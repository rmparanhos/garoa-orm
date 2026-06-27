using Microsoft.Data.Sqlite;
using Xunit;

namespace Garoa.Tests;

/// <summary>
/// <c>IN @list</c> expansion in <see cref="ParameterBinder"/>, exercised over in-memory SQLite (the
/// logic is provider-agnostic). Covers list expansion, single/empty lists, the empty-list false
/// predicate, scalars that must not be expanded (string/byte[]), and token-boundary safety.
/// </summary>
public class InExpansionTests
{
    private sealed class Widget
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }

    private static SqliteConnection NewDatabase()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        connection.Execute("CREATE TABLE widgets (id INTEGER PRIMARY KEY, name TEXT);");
        connection.Execute(
            "INSERT INTO widgets (id, name) VALUES (1, 'Bolt'), (2, 'Nut'), (3, 'Screw'), (4, 'Washer');");
        return connection;
    }

    [Fact]
    public void Expands_a_list_into_an_IN_clause()
    {
        using SqliteConnection db = NewDatabase();

        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE id IN @ids ORDER BY id;",
            new { ids = new[] { 1, 3 } });

        Assert.Equal(new long[] { 1, 3 }, rows.Select(w => w.Id));
    }

    [Fact]
    public void Expands_a_single_element_list()
    {
        using SqliteConnection db = NewDatabase();

        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE id IN @ids;",
            new { ids = new[] { 2 } });

        Assert.Equal(2, Assert.Single(rows).Id);
    }

    [Fact]
    public void Empty_list_returns_no_rows_without_a_syntax_error()
    {
        using SqliteConnection db = NewDatabase();

        // Must rewrite to a false predicate, never emit IN () (a syntax error on every provider).
        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE id IN @ids;",
            new { ids = Array.Empty<int>() });

        Assert.Empty(rows);
    }

    [Fact]
    public void Works_with_a_List_not_just_an_array()
    {
        using SqliteConnection db = NewDatabase();

        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE id IN @ids ORDER BY id;",
            new { ids = new List<int> { 2, 4 } });

        Assert.Equal(new long[] { 2, 4 }, rows.Select(w => w.Id));
    }

    [Fact]
    public void Expands_a_string_list_with_quoting_handled_by_parameters()
    {
        using SqliteConnection db = NewDatabase();

        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE name IN @names ORDER BY id;",
            new { names = new[] { "Bolt", "Screw" } });

        Assert.Equal(new[] { "Bolt", "Screw" }, rows.Select(w => w.Name));
    }

    [Fact]
    public void A_string_value_is_a_scalar_not_a_char_list()
    {
        using SqliteConnection db = NewDatabase();

        // A string is IEnumerable<char> but must bind as a single scalar, not expand to ('B','o',…).
        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE name = @name;",
            new { name = "Bolt" });

        Assert.Equal("Bolt", Assert.Single(rows).Name);
    }

    [Fact]
    public void Mixes_a_scalar_and_an_expanded_list()
    {
        using SqliteConnection db = NewDatabase();

        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE id IN @ids AND name <> @name ORDER BY id;",
            new { ids = new[] { 1, 2, 3 }, name = "Nut" });

        Assert.Equal(new long[] { 1, 3 }, rows.Select(w => w.Id));
    }

    [Fact]
    public void A_list_token_is_not_confused_with_a_longer_named_parameter()
    {
        using SqliteConnection db = NewDatabase();

        // @id (scalar) and @ids (list) coexist; expanding @ids must not touch @id.
        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE id IN @ids OR id = @id ORDER BY id;",
            new { ids = new[] { 1, 2 }, id = 4 });

        Assert.Equal(new long[] { 1, 2, 4 }, rows.Select(w => w.Id));
    }

    [Fact]
    public void The_same_list_token_used_twice_expands_both_occurrences()
    {
        using SqliteConnection db = NewDatabase();

        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE id IN @ids OR id IN @ids ORDER BY id;",
            new { ids = new[] { 2, 4 } });

        Assert.Equal(new long[] { 2, 4 }, rows.Select(w => w.Id));
    }
}
