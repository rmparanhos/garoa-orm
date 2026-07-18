using Garoa.Mapping;

namespace Garoa.Bulk;

/// <summary>
/// Resolves user-supplied conflict-key names against the columns a bulk upsert actually writes,
/// using the same case- and underscore-insensitive matching as the explicit <c>columns</c> argument —
/// so <c>conflictKeys: ["UserId"]</c> finds the written column <c>user_id</c>. Shared by the
/// provider upserts so both stay consistent with the rest of Garoa's name matching.
/// </summary>
internal static class ConflictKeys
{
    /// <summary>
    /// Maps each conflict key to the matching written column name (which is what must be emitted
    /// into the SQL), throwing when a key matches none of the written columns.
    /// </summary>
    public static string[] Resolve(IReadOnlyList<string> writtenColumns, IReadOnlyList<string> conflictKeys)
    {
        var byNormalized = new Dictionary<string, string>(writtenColumns.Count, StringComparer.Ordinal);
        foreach (string column in writtenColumns)
            byNormalized.TryAdd(TypeHelper.NormalizeName(column), column);

        var resolved = new string[conflictKeys.Count];
        for (int i = 0; i < conflictKeys.Count; i++)
        {
            if (!byNormalized.TryGetValue(TypeHelper.NormalizeName(conflictKeys[i]), out string? column))
                throw new GaroaMappingException(
                    $"Conflict key '{conflictKeys[i]}' does not match any written column " +
                    $"({string.Join(", ", writtenColumns)}).");
            resolved[i] = column;
        }

        return resolved;
    }
}
