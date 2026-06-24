using System.Data.Common;

namespace Garoa;

/// <summary>
/// A compile-time row mapper produced by the Garoa source generator for a <see cref="GaroaMappedAttribute"/>
/// type. Implementations are generated; this contract is the seam the runtime uses to prefer a
/// generated mapper over the expression-tree fallback.
/// </summary>
/// <typeparam name="T">The type each row is materialised into.</typeparam>
public interface IGaroaRowMapper<out T>
{
    /// <summary>
    /// Maps a normalised column name (lower-cased, underscores stripped) to the member slot the
    /// generator assigned it, or <c>-1</c> when no member matches. Called once per result-set
    /// layout to build the slot array passed to <see cref="Map"/>.
    /// </summary>
    /// <param name="normalizedColumn">The column name after Garoa's name normalisation.</param>
    /// <returns>The member slot index, or <c>-1</c> if the column is unmapped.</returns>
    int ResolveSlot(string normalizedColumn);

    /// <summary>
    /// Materialises a single row. <paramref name="slots"/> has one entry per reader column,
    /// mapping that column ordinal to a member slot (or <c>-1</c> to skip it).
    /// </summary>
    /// <param name="reader">The reader positioned on the row to read.</param>
    /// <param name="slots">Per-ordinal member slots, as produced by <see cref="ResolveSlot"/>.</param>
    /// <returns>The materialised instance.</returns>
    T Map(DbDataReader reader, int[] slots);
}
