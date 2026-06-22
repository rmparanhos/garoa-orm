using System.Collections.Concurrent;
using System.Data.Common;

namespace Garoa.Mapping;

/// <summary>
/// Per-type cache of compiled mappers, keyed by the result set's column layout.
/// A given <typeparamref name="T"/> can be materialised from different column sets
/// (e.g. <c>SELECT *</c> vs an explicit projection); each gets its own cached mapper.
/// </summary>
/// <remarks>
/// When the source generator has emitted a mapper for <typeparamref name="T"/> (i.e. the type is
/// annotated with <see cref="GaroaMappedAttribute"/>), that generated mapper is used and only a
/// cheap per-layout slot array is built. Otherwise the runtime expression-tree mapper is compiled.
/// </remarks>
internal static class Mapper<T>
{
    private static readonly ConcurrentDictionary<MapperKey, Func<DbDataReader, T>> Cache = new();

    // Resolved once per closed generic. Null when no generated mapper exists for T.
    private static readonly IGaroaRowMapper<T>? Generated = GaroaGeneratedMappers.Get<T>();

    public static Func<DbDataReader, T> ForReader(DbDataReader reader)
    {
        int count = reader.FieldCount;
        var columns = new string[count];
        for (int i = 0; i < count; i++)
            columns[i] = reader.GetName(i);

        var key = new MapperKey(columns);
        return Cache.GetOrAdd(key, static (k, generated) => generated is null
            ? MapperFactory.Create<T>(k.Columns)
            : BuildGenerated(generated, k.Columns), Generated);
    }

    // Resolves each reader column to a member slot once, then closes over the slot array so the
    // generated mapper only does typed reads per row.
    private static Func<DbDataReader, T> BuildGenerated(IGaroaRowMapper<T> generated, string[] columns)
    {
        var slots = new int[columns.Length];
        for (int i = 0; i < columns.Length; i++)
            slots[i] = generated.ResolveSlot(TypeHelper.NormalizeName(columns[i]));

        return reader => generated.Map(reader, slots);
    }
}
