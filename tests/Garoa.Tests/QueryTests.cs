using Microsoft.Data.Sqlite;
using Xunit;

namespace Garoa.Tests;

/// <summary>
/// End-to-end coverage over a real ADO.NET provider (in-memory SQLite), exercising the full
/// path: command creation, parameter binding, execution and mapping.
/// </summary>
public class QueryTests
{
    private sealed class Widget
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
    }

    private static SqliteConnection NewDatabase()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        connection.Execute(
            "CREATE TABLE widgets (id INTEGER PRIMARY KEY, name TEXT, price REAL);");
        return connection;
    }

    [Fact]
    public void Execute_returns_rows_affected_and_binds_parameters()
    {
        using SqliteConnection db = NewDatabase();

        int affected = db.Execute(
            "INSERT INTO widgets (id, name, price) VALUES (@Id, @Name, @Price);",
            new { Id = 1L, Name = "Bolt", Price = 0.5 });

        Assert.Equal(1, affected);
    }

    [Fact]
    public void Query_materialises_rows()
    {
        using SqliteConnection db = NewDatabase();
        db.Execute("INSERT INTO widgets (id, name, price) VALUES (1, 'Bolt', 0.5), (2, 'Nut', 0.25);");

        List<Widget> widgets = db.Query<Widget>("SELECT id, name, price FROM widgets ORDER BY id;");

        Assert.Equal(2, widgets.Count);
        Assert.Equal("Bolt", widgets[0].Name);
        Assert.Equal(0.25, widgets[1].Price);
    }

    [Fact]
    public void Query_with_parameter_filters_rows()
    {
        using SqliteConnection db = NewDatabase();
        db.Execute("INSERT INTO widgets (id, name, price) VALUES (1, 'Bolt', 0.5), (2, 'Nut', 0.25);");

        List<Widget> cheap = db.Query<Widget>(
            "SELECT id, name, price FROM widgets WHERE price < @Max;",
            new { Max = 0.3 });

        Widget only = Assert.Single(cheap);
        Assert.Equal("Nut", only.Name);
    }

    [Fact]
    public void Query_scalar_returns_values()
    {
        using SqliteConnection db = NewDatabase();
        db.Execute("INSERT INTO widgets (id, name, price) VALUES (1, 'Bolt', 0.5), (2, 'Nut', 0.25);");

        List<long> ids = db.Query<long>("SELECT id FROM widgets ORDER BY id;");

        Assert.Equal(new long[] { 1, 2 }, ids);
    }

    [Fact]
    public async Task QueryAsync_and_ExecuteAsync_work()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        await db.ExecuteAsync("CREATE TABLE widgets (id INTEGER PRIMARY KEY, name TEXT, price REAL);");

        int affected = await db.ExecuteAsync(
            "INSERT INTO widgets (id, name, price) VALUES (@Id, @Name, @Price);",
            new { Id = 1L, Name = "Bolt", Price = 0.5 });
        Assert.Equal(1, affected);

        List<Widget> widgets = await db.QueryAsync<Widget>("SELECT id, name, price FROM widgets;");
        Assert.Single(widgets);
    }

    [Fact]
    public void Query_opens_and_closes_a_closed_connection()
    {
        // A never-opened connection: Query must open it, run, then leave it closed again.
        using var db = new SqliteConnection("Data Source=:memory:");
        Assert.Equal(System.Data.ConnectionState.Closed, db.State);

        List<long> values = db.Query<long>("SELECT 1;");

        Assert.Equal(new long[] { 1 }, values);
        Assert.Equal(System.Data.ConnectionState.Closed, db.State);
    }
}
