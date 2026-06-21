using System.Collections.Concurrent;
using System.Data.Common;

namespace Garoa.Mapping;

/// <summary>
/// Per-type cache of compiled mappers, keyed by the result set's column layout.
/// A given <typeparamref name="T"/> can be materialised from different column sets
/// (e.g. <c>SELECT *</c> vs an explicit projection); each gets its own cached mapper.
/// </summary>
internal static class Mapper<T>
{
    private static readonly ConcurrentDictionary<MapperKey, Func<DbDataReader, T>> Cache = new();

    public static Func<DbDataReader, T> ForReader(DbDataReader reader)
    {
        int count = reader.FieldCount;
        var columns = new string[count];
        for (int i = 0; i < count; i++)
            columns[i] = reader.GetName(i);

        var key = new MapperKey(columns);
        return Cache.GetOrAdd(key, static k => MapperFactory.Create<T>(k.Columns));
    }
}
