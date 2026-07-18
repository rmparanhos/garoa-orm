using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace Garoa.Mapping;

/// <summary>
/// Builds compiled <c>Func&lt;DbDataReader, T&gt;</c> mappers from a result-set's column layout.
/// Mappers are produced with expression trees (no IL emission). BCL types read through the typed
/// reader getters (<c>GetInt32</c>, <c>GetString</c>, …), which the JIT inlines; provider-resolved
/// types (DateOnly, TimeOnly, …) read through <see cref="DbDataReader.GetFieldValue{T}(int)"/>,
/// delegating type handling to the provider — the same strategy as the source-generated mapper.
/// </summary>
internal static class MapperFactory
{
    private static readonly MethodInfo GetFieldValueMethod =
        typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetFieldValue))!;

    private static readonly MethodInfo IsDbNullMethod =
        typeof(DbDataReader).GetMethod(nameof(DbDataReader.IsDBNull), new[] { typeof(int) })!;

    // Typed reader getters for the types the BCL exposes one for — the same list the source
    // generator uses. The JIT inlines these, unlike the generic GetFieldValue<T> dispatch, which is
    // where the runtime mapper used to trail Dapper's IL mapper on large result sets. Everything
    // else (DateOnly, TimeOnly, DateTimeOffset, byte[], custom types) still goes through
    // GetFieldValue<T> so the provider resolves it — the delegation that motivates Garoa.
    private static readonly Dictionary<Type, MethodInfo> TypedGetters = new()
    {
        [typeof(bool)] = Getter(nameof(DbDataReader.GetBoolean)),
        [typeof(byte)] = Getter(nameof(DbDataReader.GetByte)),
        [typeof(char)] = Getter(nameof(DbDataReader.GetChar)),
        [typeof(short)] = Getter(nameof(DbDataReader.GetInt16)),
        [typeof(int)] = Getter(nameof(DbDataReader.GetInt32)),
        [typeof(long)] = Getter(nameof(DbDataReader.GetInt64)),
        [typeof(float)] = Getter(nameof(DbDataReader.GetFloat)),
        [typeof(double)] = Getter(nameof(DbDataReader.GetDouble)),
        [typeof(decimal)] = Getter(nameof(DbDataReader.GetDecimal)),
        [typeof(string)] = Getter(nameof(DbDataReader.GetString)),
        [typeof(DateTime)] = Getter(nameof(DbDataReader.GetDateTime)),
        [typeof(Guid)] = Getter(nameof(DbDataReader.GetGuid)),
    };

    private static MethodInfo Getter(string name) =>
        typeof(DbDataReader).GetMethod(name, new[] { typeof(int) })!;

    private static readonly MethodInfo CreateExceptionMethod =
        typeof(MapperFactory).GetMethod(nameof(CreateMappingException), BindingFlags.NonPublic | BindingFlags.Static)!;

    public static Func<DbDataReader, T> Create<T>(string[] columns)
    {
        Type target = typeof(T);

        return TypeHelper.IsSimpleType(target)
            ? CreateScalarMapper<T>(target)
            : CreateClassMapper<T>(target, columns);
    }

    private static Func<DbDataReader, T> CreateScalarMapper<T>(Type target)
    {
        var reader = Expression.Parameter(typeof(DbDataReader), "reader");
        var index = Expression.Variable(typeof(int), "index");

        var body = Expression.Block(
            target,
            new[] { index },
            Wrap(reader, index, target, ReadColumn(reader, 0, target)));

        return Expression.Lambda<Func<DbDataReader, T>>(body, reader).Compile();
    }

    private static Func<DbDataReader, T> CreateClassMapper<T>(Type target, string[] columns)
    {
        ConstructorInfo ctor = target.GetConstructor(Type.EmptyTypes)
            ?? throw new GaroaMappingException(
                $"Cannot map to '{target.FullName}': a public parameterless constructor is required.");

        var reader = Expression.Parameter(typeof(DbDataReader), "reader");
        var instance = Expression.Variable(target, "instance");
        var index = Expression.Variable(typeof(int), "index");

        Dictionary<string, MemberInfo> members = BuildMemberLookup(target);

        var statements = new List<Expression> { Expression.Assign(instance, Expression.New(ctor)) };

        for (int i = 0; i < columns.Length; i++)
        {
            if (!members.TryGetValue(TypeHelper.NormalizeName(columns[i]), out MemberInfo? member))
                continue;

            Type memberType = MemberType(member);
            statements.Add(Expression.Assign(index, Expression.Constant(i)));
            statements.Add(Expression.Assign(
                Expression.MakeMemberAccess(instance, member),
                ReadColumn(reader, i, memberType)));
        }

        statements.Add(instance);

        Expression tryBody = Expression.Block(target, statements);
        Expression guarded = WrapBlock(reader, index, target, tryBody);

        var outer = Expression.Block(target, new[] { instance, index }, guarded);
        return Expression.Lambda<Func<DbDataReader, T>>(outer, reader).Compile();
    }

    /// <summary>
    /// Emits <c>reader.IsDBNull(i) ? default : reader.GetFieldValue&lt;TUnderlying&gt;(i)</c>,
    /// unwrapping <see cref="Nullable{T}"/> and reading enums via their numeric backing type.
    /// </summary>
    private static Expression ReadColumn(ParameterExpression reader, int ordinal, Type memberType)
    {
        Type underlying = Nullable.GetUnderlyingType(memberType) ?? memberType;
        ConstantExpression ord = Expression.Constant(ordinal);

        Expression value;
        if (underlying.IsEnum)
        {
            Type numeric = Enum.GetUnderlyingType(underlying);
            Expression raw = Expression.Call(reader, GetFieldValueMethod.MakeGenericMethod(numeric), ord);
            value = Expression.Convert(raw, underlying);
        }
        else if (TypedGetters.TryGetValue(underlying, out MethodInfo? typedGetter))
        {
            value = Expression.Call(reader, typedGetter, ord);
        }
        else
        {
            value = Expression.Call(reader, GetFieldValueMethod.MakeGenericMethod(underlying), ord);
        }

        if (underlying != memberType)
            value = Expression.Convert(value, memberType); // T -> T?

        Expression isNull = Expression.Call(reader, IsDbNullMethod, ord);
        return Expression.Condition(isNull, Expression.Default(memberType), value);
    }

    /// <summary>Wraps a scalar read: assign the ordinal then guard with a typed try/catch.</summary>
    private static Expression Wrap(ParameterExpression reader, ParameterExpression index, Type target, Expression read)
    {
        Expression body = Expression.Block(target, Expression.Assign(index, Expression.Constant(0)), read);
        return WrapBlock(reader, index, target, body);
    }

    /// <summary>Wraps a mapper body in a try/catch that rethrows a column-accurate mapping error.</summary>
    private static Expression WrapBlock(ParameterExpression reader, ParameterExpression index, Type target, Expression body)
    {
        var ex = Expression.Parameter(typeof(Exception), "ex");
        Expression throwExpr = Expression.Throw(
            Expression.Call(CreateExceptionMethod, reader, index, Expression.Constant(target, typeof(Type)), ex),
            target);

        return Expression.TryCatch(body, Expression.Catch(ex, throwExpr));
    }

    private static Dictionary<string, MemberInfo> BuildMemberLookup(Type target)
    {
        var lookup = new Dictionary<string, MemberInfo>(StringComparer.Ordinal);

        foreach (PropertyInfo prop in target.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.SetMethod is null || prop.GetIndexParameters().Length > 0)
                continue;
            Add(lookup, prop, prop.Name);
        }

        foreach (FieldInfo field in target.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.IsInitOnly)
                continue;
            Add(lookup, field, field.Name);
        }

        return lookup;
    }

    private static void Add(Dictionary<string, MemberInfo> lookup, MemberInfo member, string memberName)
    {
        string name = member.GetCustomAttribute<ColumnAttribute>()?.Name ?? memberName;
        // First declaration wins (properties are registered before fields).
        lookup.TryAdd(TypeHelper.NormalizeName(name), member);
    }

    private static Type MemberType(MemberInfo member) => member switch
    {
        PropertyInfo p => p.PropertyType,
        FieldInfo f => f.FieldType,
        _ => throw new InvalidOperationException($"Unsupported member kind: {member.MemberType}."),
    };

    // Invoked from compiled mappers. Defensive: never let diagnostics throw over the real error.
    private static Exception CreateMappingException(DbDataReader reader, int index, Type target, Exception inner)
    {
        if (inner is GaroaMappingException existing)
            return existing;

        string column;
        string providerType;
        try
        {
            column = reader.GetName(index);
            providerType = reader.GetFieldType(index)?.Name ?? "unknown";
        }
        catch
        {
            column = $"#{index}";
            providerType = "unknown";
        }

        return new GaroaMappingException(
            $"Error mapping column '{column}' (ordinal {index}, provider type '{providerType}') to '{target.Name}': {inner.Message}",
            inner);
    }
}
