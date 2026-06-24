namespace Garoa;

/// <summary>
/// How <c>BulkInsert</c> derives a destination column name from a member when the member has no
/// <see cref="ColumnAttribute"/> and the call does not pass an explicit column list. The read side
/// already matches column names underscore- and case-insensitively, so the default mirrors that on
/// the write side for the snake_case tables that PostgreSQL and MySQL conventionally use.
/// </summary>
public enum BulkNamingConvention
{
    /// <summary>
    /// Convert the member name to <c>snake_case</c> (e.g. <c>BirthDate</c> → <c>birth_date</c>).
    /// The default, matching the prevailing PostgreSQL/MySQL column-naming convention.
    /// </summary>
    SnakeCase = 0,

    /// <summary>
    /// Emit the member name verbatim (e.g. <c>BirthDate</c> → <c>BirthDate</c>). Use this when the
    /// table's columns are named exactly like the CLR members.
    /// </summary>
    MemberName = 1,
}
