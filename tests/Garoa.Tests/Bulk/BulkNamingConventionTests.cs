using Garoa.Bulk;
using Garoa.Mapping;
using Xunit;

namespace Garoa.Tests.Bulk;

/// <summary>
/// Covers the write-side naming convention: how <c>BulkInsert</c> derives a column name from a
/// member. <see cref="GaroaDefaults.BulkNamingConvention"/> is process-wide, so each test saves and
/// restores it.
/// </summary>
public sealed class BulkNamingConventionTests : IDisposable
{
    private readonly BulkNamingConvention _original = GaroaDefaults.BulkNamingConvention;

    public void Dispose() => GaroaDefaults.BulkNamingConvention = _original;

    private sealed class Account
    {
        public long Id { get; set; }
        public string? FullName { get; set; }
        public DateOnly BirthDate { get; set; }

        [Column("years_at_company")]
        public int Tenure { get; set; }
    }

    [Theory]
    [InlineData("BirthDate", "birth_date")]
    [InlineData("ManagerId", "manager_id")]
    [InlineData("Id", "id")]
    [InlineData("Name", "name")]
    [InlineData("HTTPStatus", "http_status")]
    [InlineData("already_snake", "already_snake")]
    public void ToSnakeCase_converts_pascal_and_camel_case(string input, string expected)
        => Assert.Equal(expected, TypeHelper.ToSnakeCase(input));

    [Fact]
    public void SnakeCase_is_the_default_for_unannotated_members()
    {
        GaroaDefaults.BulkNamingConvention = BulkNamingConvention.SnakeCase;

        BulkColumnSet<Account> set = BulkColumnSet<Account>.Get(null);

        // [Column] still wins for Tenure; the rest go through snake_case.
        Assert.Equal(new[] { "id", "full_name", "birth_date", "years_at_company" }, set.ColumnNames);
    }

    [Fact]
    public void MemberName_convention_emits_members_verbatim_but_Column_still_wins()
    {
        GaroaDefaults.BulkNamingConvention = BulkNamingConvention.MemberName;

        BulkColumnSet<Account> set = BulkColumnSet<Account>.Get(null);

        Assert.Equal(new[] { "Id", "FullName", "BirthDate", "years_at_company" }, set.ColumnNames);
    }

    [Fact]
    public void Explicit_columns_are_used_verbatim_regardless_of_convention()
    {
        GaroaDefaults.BulkNamingConvention = BulkNamingConvention.SnakeCase;

        // Explicit names match members underscore-insensitively and are emitted exactly as given.
        BulkColumnSet<Account> set = BulkColumnSet<Account>.Get(new[] { "FullName", "birth_date" });

        Assert.Equal(new[] { "FullName", "birth_date" }, set.ColumnNames);
    }
}
