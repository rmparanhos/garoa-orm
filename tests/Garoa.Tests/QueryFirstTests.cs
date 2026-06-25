using Microsoft.Data.Sqlite;
using Xunit;

namespace Garoa.Tests;

/// <summary>
/// Behaviour of the single-row read family (<c>QueryFirst</c> / <c>QueryFirstOrDefault</c>, sync +
/// async) over in-memory SQLite. The matrix: returns the first row, throws vs returns
/// <c>default</c> when empty, and ignores extra rows.
/// </summary>
public class QueryFirstTests
{
    private sealed class Widget
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }

    private static SqliteConnection NewDatabase(bool seed = true)
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        connection.Execute("CREATE TABLE widgets (id INTEGER PRIMARY KEY, name TEXT);");
        if (seed)
            connection.Execute("INSERT INTO widgets (id, name) VALUES (1, 'Bolt'), (2, 'Nut');");
        return connection;
    }

    [Fact]
    public void QueryFirst_returns_the_first_row()
    {
        using SqliteConnection db = NewDatabase();

        Widget w = db.QueryFirst<Widget>("SELECT id, name FROM widgets ORDER BY id;");

        Assert.Equal(1, w.Id);
        Assert.Equal("Bolt", w.Name);
    }

    [Fact]
    public void QueryFirst_ignores_rows_after_the_first()
    {
        using SqliteConnection db = NewDatabase();

        // Two rows match; only the first (by ORDER BY) comes back.
        Widget w = db.QueryFirst<Widget>("SELECT id, name FROM widgets ORDER BY id DESC;");

        Assert.Equal(2, w.Id);
    }

    [Fact]
    public void QueryFirst_throws_when_there_are_no_rows()
    {
        using SqliteConnection db = NewDatabase();

        Assert.Throws<InvalidOperationException>(
            () => db.QueryFirst<Widget>("SELECT id, name FROM widgets WHERE id = 999;"));
    }

    [Fact]
    public void QueryFirstOrDefault_returns_the_first_row()
    {
        using SqliteConnection db = NewDatabase();

        Widget? w = db.QueryFirstOrDefault<Widget>("SELECT id, name FROM widgets ORDER BY id;");

        Assert.NotNull(w);
        Assert.Equal(1, w!.Id);
    }

    [Fact]
    public void QueryFirstOrDefault_returns_null_for_a_reference_type_when_empty()
    {
        using SqliteConnection db = NewDatabase();

        Widget? w = db.QueryFirstOrDefault<Widget>("SELECT id, name FROM widgets WHERE id = 999;");

        Assert.Null(w);
    }

    [Fact]
    public void QueryFirstOrDefault_returns_zero_for_a_value_type_scalar_when_empty()
    {
        using SqliteConnection db = NewDatabase();

        long count = db.QueryFirstOrDefault<long>("SELECT id FROM widgets WHERE id = 999;");

        Assert.Equal(0, count);
    }

    [Fact]
    public void QueryFirst_binds_parameters_and_maps_scalars()
    {
        using SqliteConnection db = NewDatabase();

        string name = db.QueryFirst<string>("SELECT name FROM widgets WHERE id = @Id;", new { Id = 2 });

        Assert.Equal("Nut", name);
    }

    [Fact]
    public async Task QueryFirstAsync_returns_the_first_row()
    {
        await using SqliteConnection db = NewDatabase();

        Widget w = await db.QueryFirstAsync<Widget>("SELECT id, name FROM widgets ORDER BY id;");

        Assert.Equal(1, w.Id);
    }

    [Fact]
    public async Task QueryFirstAsync_throws_when_there_are_no_rows()
    {
        await using SqliteConnection db = NewDatabase();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => db.QueryFirstAsync<Widget>("SELECT id, name FROM widgets WHERE id = 999;"));
    }

    [Fact]
    public async Task QueryFirstOrDefaultAsync_returns_null_when_empty()
    {
        await using SqliteConnection db = NewDatabase();

        Widget? w = await db.QueryFirstOrDefaultAsync<Widget>("SELECT id, name FROM widgets WHERE id = 999;");

        Assert.Null(w);
    }
}
