using Microsoft.Data.Sqlite;
using Xunit;

namespace Garoa.Tests;

/// <summary>
/// Behaviour of the single-row strict read family (<c>QuerySingle</c> /
/// <c>QuerySingleOrDefault</c>, sync + async) over in-memory SQLite. The matrix: returns the row when
/// exactly one matches, throws vs returns <c>default</c> when none, and always throws on more than one.
/// </summary>
public class QuerySingleTests
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
        connection.Execute("INSERT INTO widgets (id, name) VALUES (1, 'Bolt'), (2, 'Nut');");
        return connection;
    }

    [Fact]
    public void QuerySingle_returns_the_row_when_exactly_one_matches()
    {
        using SqliteConnection db = NewDatabase();

        Widget w = db.QuerySingle<Widget>("SELECT id, name FROM widgets WHERE id = 1;");

        Assert.Equal(1, w.Id);
        Assert.Equal("Bolt", w.Name);
    }

    [Fact]
    public void QuerySingle_throws_when_there_are_no_rows()
    {
        using SqliteConnection db = NewDatabase();

        Assert.Throws<InvalidOperationException>(
            () => db.QuerySingle<Widget>("SELECT id, name FROM widgets WHERE id = 99;"));
    }

    [Fact]
    public void QuerySingle_throws_when_more_than_one_row_matches()
    {
        using SqliteConnection db = NewDatabase();

        Assert.Throws<InvalidOperationException>(
            () => db.QuerySingle<Widget>("SELECT id, name FROM widgets;"));
    }

    [Fact]
    public void QuerySingleOrDefault_returns_the_row_when_exactly_one_matches()
    {
        using SqliteConnection db = NewDatabase();

        Widget? w = db.QuerySingleOrDefault<Widget>("SELECT id, name FROM widgets WHERE id = 2;");

        Assert.NotNull(w);
        Assert.Equal(2, w!.Id);
    }

    [Fact]
    public void QuerySingleOrDefault_returns_default_when_there_are_no_rows()
    {
        using SqliteConnection db = NewDatabase();

        Widget? w = db.QuerySingleOrDefault<Widget>("SELECT id, name FROM widgets WHERE id = 99;");

        Assert.Null(w);
    }

    [Fact]
    public void QuerySingleOrDefault_throws_when_more_than_one_row_matches()
    {
        using SqliteConnection db = NewDatabase();

        // OrDefault forgives zero rows, but more-than-one is still an error.
        Assert.Throws<InvalidOperationException>(
            () => db.QuerySingleOrDefault<Widget>("SELECT id, name FROM widgets;"));
    }

    [Fact]
    public async Task QuerySingleAsync_returns_the_row_when_exactly_one_matches()
    {
        await using SqliteConnection db = NewDatabase();

        Widget w = await db.QuerySingleAsync<Widget>("SELECT id, name FROM widgets WHERE id = 1;");

        Assert.Equal(1, w.Id);
    }

    [Fact]
    public async Task QuerySingleAsync_throws_when_more_than_one_row_matches()
    {
        await using SqliteConnection db = NewDatabase();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => db.QuerySingleAsync<Widget>("SELECT id, name FROM widgets;"));
    }

    [Fact]
    public async Task QuerySingleOrDefaultAsync_returns_default_when_there_are_no_rows()
    {
        await using SqliteConnection db = NewDatabase();

        Widget? w = await db.QuerySingleOrDefaultAsync<Widget>("SELECT id, name FROM widgets WHERE id = 99;");

        Assert.Null(w);
    }

    [Fact]
    public async Task QuerySingleOrDefaultAsync_throws_when_more_than_one_row_matches()
    {
        await using SqliteConnection db = NewDatabase();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => db.QuerySingleOrDefaultAsync<Widget>("SELECT id, name FROM widgets;"));
    }
}
