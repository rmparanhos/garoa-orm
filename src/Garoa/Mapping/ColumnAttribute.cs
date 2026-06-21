namespace Garoa;

/// <summary>
/// Maps a property or field to a result-set column whose name differs from the member name.
/// Matching is otherwise case-insensitive and ignores underscores (so <c>user_id</c> binds to <c>UserId</c>).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ColumnAttribute : Attribute
{
    /// <summary>The column name this member binds to.</summary>
    public string Name { get; }

    /// <param name="name">The column name as returned by the query.</param>
    public ColumnAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Column name must not be empty.", nameof(name));
        Name = name;
    }
}
