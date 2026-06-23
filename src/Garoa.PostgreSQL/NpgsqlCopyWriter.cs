using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Garoa.Bulk;
using Npgsql;

namespace Garoa;

/// <summary>
/// A compiled, allocation-free row writer for the PostgreSQL binary <c>COPY</c> path. For a given
/// type and column layout it builds an <see cref="Action{T1, T2}"/> that writes each member to the
/// <see cref="NpgsqlBinaryImporter"/> through the typed <c>Write&lt;T&gt;</c> overload — so value
/// types (<c>long</c>, <c>double</c>, <c>DateOnly</c>, enums…) never get boxed into an
/// <c>object[]</c> the way the generic reader path requires. Column selection is delegated to
/// <see cref="BulkColumnSet{T}.SelectColumns"/> so the emitted columns match the rest of Garoa
/// exactly. Cached by column layout, mirroring <see cref="BulkColumnSet{T}"/>.
/// </summary>
internal sealed class NpgsqlCopyWriter<T>
{
    private static readonly ConcurrentDictionary<string, NpgsqlCopyWriter<T>> Cache = new();

    // The single-argument generic NpgsqlBinaryImporter.Write<TValue>(TValue) overload.
    private static readonly MethodInfo WriteGeneric = typeof(NpgsqlBinaryImporter).GetMethods()
        .Single(m => m.Name == nameof(NpgsqlBinaryImporter.Write)
            && m.IsGenericMethodDefinition
            && m.GetParameters().Length == 1);

    private static readonly MethodInfo WriteNullMethod =
        typeof(NpgsqlBinaryImporter).GetMethod(nameof(NpgsqlBinaryImporter.WriteNull), Type.EmptyTypes)!;

    private readonly Action<NpgsqlBinaryImporter, T> _writeRow;

    /// <summary>Destination column names, in the order values are written.</summary>
    public string[] ColumnNames { get; }

    private NpgsqlCopyWriter(string[] columnNames, Action<NpgsqlBinaryImporter, T> writeRow)
    {
        ColumnNames = columnNames;
        _writeRow = writeRow;
    }

    /// <summary>Writes one row's values to <paramref name="importer"/> (call after <c>StartRow</c>).</summary>
    public void WriteRow(NpgsqlBinaryImporter importer, T row) => _writeRow(importer, row);

    public static NpgsqlCopyWriter<T> Get(IReadOnlyList<string>? columns)
    {
        string key = columns is null ? "*" : string.Join("", columns);
        return Cache.GetOrAdd(key, _ => Build(columns));
    }

    private static NpgsqlCopyWriter<T> Build(IReadOnlyList<string>? columns)
    {
        (MemberInfo Member, string Column)[] selected = BulkColumnSet<T>.SelectColumns(columns);
        if (selected.Length == 0)
            throw new InvalidOperationException($"No bulk-insertable members found on '{typeof(T).FullName}'.");

        var importer = Expression.Parameter(typeof(NpgsqlBinaryImporter), "importer");
        var row = Expression.Parameter(typeof(T), "row");

        var names = new string[selected.Length];
        var writes = new Expression[selected.Length];
        for (int i = 0; i < selected.Length; i++)
        {
            (MemberInfo member, string column) = selected[i];
            names[i] = column;
            writes[i] = BuildWrite(importer, Expression.MakeMemberAccess(row, member));
        }

        var body = Expression.Block(writes);
        Action<NpgsqlBinaryImporter, T> writeRow =
            Expression.Lambda<Action<NpgsqlBinaryImporter, T>>(body, importer, row).Compile();

        return new NpgsqlCopyWriter<T>(names, writeRow);
    }

    // Emits the typed Write / WriteNull for one member, mirroring BulkColumnSet's value handling
    // (enums as their numeric backing value; null stays null) but without boxing.
    private static Expression BuildWrite(ParameterExpression importer, Expression access)
    {
        Type memberType = access.Type;
        Type underlying = Nullable.GetUnderlyingType(memberType) ?? memberType;

        if (underlying.IsEnum)
        {
            Type numeric = Enum.GetUnderlyingType(underlying);
            if (underlying == memberType) // non-nullable enum
                return Write(importer, Expression.Convert(access, numeric), numeric);

            // Nullable<enum>: write the numeric value or NULL.
            return Expression.IfThenElse(
                Expression.Property(access, "HasValue"),
                Write(importer, Expression.Convert(Expression.Property(access, "Value"), numeric), numeric),
                WriteNull(importer));
        }

        if (Nullable.GetUnderlyingType(memberType) is { } nullableUnderlying)
        {
            // Nullable<T> value type: write the underlying value or NULL.
            return Expression.IfThenElse(
                Expression.Property(access, "HasValue"),
                Write(importer, Expression.Property(access, "Value"), nullableUnderlying),
                WriteNull(importer));
        }

        if (memberType.IsValueType)
            return Write(importer, access, memberType); // non-nullable value type: never null

        // Reference type (string, byte[], …): null stays null.
        return Expression.IfThenElse(
            Expression.ReferenceEqual(access, Expression.Constant(null, memberType)),
            WriteNull(importer),
            Write(importer, access, memberType));
    }

    private static Expression Write(ParameterExpression importer, Expression value, Type valueType)
        => Expression.Call(importer, WriteGeneric.MakeGenericMethod(valueType), value);

    private static Expression WriteNull(ParameterExpression importer)
        => Expression.Call(importer, WriteNullMethod);
}
