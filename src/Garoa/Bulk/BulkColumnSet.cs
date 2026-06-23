using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Garoa.Mapping;

namespace Garoa.Bulk;

/// <summary>
/// The ordered set of columns to bulk-insert for <typeparamref name="T"/>, plus a compiled
/// delegate that extracts each row's values into a caller-supplied buffer. Building the
/// extractor once (cached by column layout) keeps the per-row cost to member access + boxing,
/// with no reflection on the hot path.
/// </summary>
/// <remarks>
/// Enum members are written as their numeric backing value so providers don't need enum
/// awareness. <see cref="Nullable{T}"/> is unwrapped; a null stays null (becomes
/// <see cref="DBNull"/> at the reader boundary).
/// </remarks>
internal sealed class BulkColumnSet<T>
{
    private static readonly ConcurrentDictionary<string, BulkColumnSet<T>> Cache = new();

    private readonly Action<T, object?[]> _fill;

    /// <summary>Destination column names, in the order values are produced.</summary>
    public string[] ColumnNames { get; }

    /// <summary>CLR type of each column (nullable unwrapped; enums as their numeric type).</summary>
    public Type[] ColumnTypes { get; }

    public int Count => ColumnNames.Length;

    private BulkColumnSet(string[] columnNames, Type[] columnTypes, Action<T, object?[]> fill)
    {
        ColumnNames = columnNames;
        ColumnTypes = columnTypes;
        _fill = fill;
    }

    /// <summary>Fills <paramref name="buffer"/> (length <see cref="Count"/>) with the row's values.</summary>
    public void Fill(T item, object?[] buffer) => _fill(item, buffer);

    public static BulkColumnSet<T> Get(IReadOnlyList<string>? columns)
    {
        string key = columns is null ? "*" : string.Join("", columns);
        return Cache.GetOrAdd(key, _ => Build(columns));
    }

    private static BulkColumnSet<T> Build(IReadOnlyList<string>? columns)
    {
        (MemberInfo Member, string Column)[] selected = SelectColumns(columns);
        if (selected.Length == 0)
            throw new GaroaMappingException($"No bulk-insertable members found on '{typeof(T).FullName}'.");

        var item = Expression.Parameter(typeof(T), "item");
        var buffer = Expression.Parameter(typeof(object[]), "buffer");

        var names = new string[selected.Length];
        var types = new Type[selected.Length];
        var assignments = new Expression[selected.Length];

        for (int i = 0; i < selected.Length; i++)
        {
            (MemberInfo member, string column) = selected[i];
            names[i] = column;

            Type memberType = MemberType(member);
            types[i] = ColumnType(memberType);

            Expression access = Expression.MakeMemberAccess(item, member);
            Expression boxed = BoxValue(access, memberType);
            assignments[i] = Expression.Assign(Expression.ArrayAccess(buffer, Expression.Constant(i)), boxed);
        }

        var body = Expression.Block(assignments);
        var fill = Expression.Lambda<Action<T, object?[]>>(body, item, buffer).Compile();

        return new BulkColumnSet<T>(names, types, fill);
    }

    /// <summary>
    /// The ordered (member, destination column) pairs this type bulk-inserts, applying the same
    /// member discovery, <c>[Column]</c> resolution and explicit-column matching the runtime fill
    /// uses. Exposed so provider writers (e.g. the PostgreSQL typed COPY writer) emit the exact same
    /// columns without duplicating the selection rules.
    /// </summary>
    internal static (MemberInfo Member, string Column)[] SelectColumns(IReadOnlyList<string>? columns)
    {
        List<(MemberInfo Member, string Column)> all = DiscoverMembers();

        if (columns is null)
            return all.ToArray();

        var byNormalized = new Dictionary<string, MemberInfo>(StringComparer.Ordinal);
        foreach ((MemberInfo member, string column) in all)
            byNormalized.TryAdd(TypeHelper.NormalizeName(column), member);

        var result = new (MemberInfo, string)[columns.Count];
        for (int i = 0; i < columns.Count; i++)
        {
            if (!byNormalized.TryGetValue(TypeHelper.NormalizeName(columns[i]), out MemberInfo? member))
                throw new GaroaMappingException(
                    $"Column '{columns[i]}' has no matching member on '{typeof(T).FullName}'.");
            result[i] = (member, columns[i]);
        }

        return result;
    }

    private static List<(MemberInfo Member, string Column)> DiscoverMembers()
    {
        var members = new List<(MemberInfo, string)>();

        foreach (PropertyInfo prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.GetMethod is null || prop.GetIndexParameters().Length > 0)
                continue;
            members.Add((prop, ColumnName(prop, prop.Name)));
        }

        foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
            members.Add((field, ColumnName(field, field.Name)));

        return members;
    }

    private static string ColumnName(MemberInfo member, string fallback)
        => member.GetCustomAttribute<ColumnAttribute>()?.Name ?? fallback;

    private static Type MemberType(MemberInfo member) => member switch
    {
        PropertyInfo p => p.PropertyType,
        FieldInfo f => f.FieldType,
        _ => throw new InvalidOperationException($"Unsupported member kind: {member.MemberType}."),
    };

    private static Type ColumnType(Type memberType)
    {
        Type underlying = Nullable.GetUnderlyingType(memberType) ?? memberType;
        return underlying.IsEnum ? Enum.GetUnderlyingType(underlying) : underlying;
    }

    /// <summary>Boxes a member value to <see cref="object"/>, mapping enums to their numeric value.</summary>
    private static Expression BoxValue(Expression access, Type memberType)
    {
        Type underlying = Nullable.GetUnderlyingType(memberType) ?? memberType;

        if (!underlying.IsEnum)
            return Expression.Convert(access, typeof(object));

        Type numeric = Enum.GetUnderlyingType(underlying);

        if (underlying == memberType) // non-nullable enum
            return Expression.Convert(Expression.Convert(access, numeric), typeof(object));

        // Nullable<enum>: null stays null, otherwise box the numeric value.
        Expression hasValue = Expression.Property(access, "HasValue");
        Expression value = Expression.Property(access, "Value");
        Expression boxedNumeric = Expression.Convert(Expression.Convert(value, numeric), typeof(object));
        return Expression.Condition(hasValue, boxedNumeric, Expression.Constant(null, typeof(object)));
    }
}
