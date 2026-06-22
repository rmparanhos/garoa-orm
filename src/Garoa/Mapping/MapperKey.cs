namespace Garoa.Mapping;

/// <summary>
/// Cache key for a compiled mapper: the exact ordered column names of a result set.
/// The target type is implied by the per-type cache that holds the key, so it is not stored here.
/// </summary>
internal readonly struct MapperKey : IEquatable<MapperKey>
{
    public readonly string[] Columns;
    private readonly int _hash;

    public MapperKey(string[] columns)
    {
        Columns = columns;

        var hash = new HashCode();
        foreach (string column in columns)
            hash.Add(column, StringComparer.Ordinal);
        _hash = hash.ToHashCode();
    }

    public bool Equals(MapperKey other)
    {
        if (_hash != other._hash || Columns.Length != other.Columns.Length)
            return false;

        for (int i = 0; i < Columns.Length; i++)
        {
            if (!string.Equals(Columns[i], other.Columns[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is MapperKey other && Equals(other);

    public override int GetHashCode() => _hash;
}
