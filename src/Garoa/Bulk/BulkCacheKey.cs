namespace Garoa.Bulk;

/// <summary>
/// Cache key for a bulk column layout: the explicit column list (or null for "all members") plus the
/// naming convention in force. Structural, like <c>MapperKey</c> on the read side — two different
/// column lists can never collide the way a separator-less string concatenation could
/// (<c>["user","name"]</c> vs <c>["username"]</c>).
/// </summary>
internal readonly struct BulkCacheKey : IEquatable<BulkCacheKey>
{
    // A private copy: the caller's list must not be able to mutate a cached key.
    private readonly string[]? _columns;
    private readonly int _convention;
    private readonly int _hash;

    public BulkCacheKey(IReadOnlyList<string>? columns, BulkNamingConvention convention)
    {
        _convention = (int)convention;

        var hash = new HashCode();
        hash.Add(_convention);

        if (columns is null)
        {
            _columns = null;
        }
        else
        {
            var copy = new string[columns.Count];
            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = columns[i];
                hash.Add(copy[i], StringComparer.Ordinal);
            }
            _columns = copy;
        }

        _hash = hash.ToHashCode();
    }

    public bool Equals(BulkCacheKey other)
    {
        if (_hash != other._hash || _convention != other._convention)
            return false;
        if (_columns is null || other._columns is null)
            return _columns is null && other._columns is null;
        if (_columns.Length != other._columns.Length)
            return false;

        for (int i = 0; i < _columns.Length; i++)
        {
            if (!string.Equals(_columns[i], other._columns[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is BulkCacheKey other && Equals(other);

    public override int GetHashCode() => _hash;
}
