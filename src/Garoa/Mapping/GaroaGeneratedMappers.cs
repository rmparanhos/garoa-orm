using System.Collections.Concurrent;
using System.Data.Common;

namespace Garoa;

/// <summary>
/// Registry of source-generated <see cref="IGaroaRowMapper{T}"/> instances. Generated code
/// self-registers through a module initializer, so by the time user code issues its first query the
/// mapper for each <see cref="GaroaMappedAttribute"/> type is already present.
/// </summary>
public static class GaroaGeneratedMappers
{
    private static readonly ConcurrentDictionary<Type, object> Mappers = new();

    /// <summary>Registers the generated mapper for <typeparamref name="T"/>. Called by generated code.</summary>
    /// <typeparam name="T">The mapped type.</typeparam>
    /// <param name="mapper">The generated mapper instance.</param>
    public static void Register<T>(IGaroaRowMapper<T> mapper) => Mappers[typeof(T)] = mapper;

    /// <summary>Returns the generated mapper for <typeparamref name="T"/>, or <c>null</c> if none was generated.</summary>
    internal static IGaroaRowMapper<T>? Get<T>() =>
        Mappers.TryGetValue(typeof(T), out object? mapper) ? (IGaroaRowMapper<T>)mapper : null;

    /// <summary>
    /// Builds a column-accurate mapping exception. Generated mappers call this from their catch
    /// block so a failed conversion names the offending column by name and ordinal — the same
    /// diagnostic quality as the runtime mapper. This is public because generated code lives in the
    /// consumer's assembly.
    /// </summary>
    /// <param name="reader">The reader being mapped.</param>
    /// <param name="ordinal">The reader ordinal that failed.</param>
    /// <param name="target">The type being mapped to.</param>
    /// <param name="inner">The original exception.</param>
    /// <returns>A <see cref="GaroaMappingException"/> describing the failure.</returns>
    public static Exception MappingError(DbDataReader reader, int ordinal, Type target, Exception inner)
    {
        if (inner is GaroaMappingException existing)
            return existing;

        string column;
        string providerType;
        try
        {
            column = reader.GetName(ordinal);
            providerType = reader.GetFieldType(ordinal)?.Name ?? "unknown";
        }
        catch
        {
            column = $"#{ordinal}";
            providerType = "unknown";
        }

        return new GaroaMappingException(
            $"Error mapping column '{column}' (ordinal {ordinal}, provider type '{providerType}') to '{target.Name}': {inner.Message}",
            inner);
    }
}
