using Garoa;
using MySqlConnector;
using Xunit;

namespace Garoa.IntegrationTests;

/// <summary>
/// Live MySQL coverage for <c>IN @list</c> expansion. MySQL has no <c>= ANY(array)</c> escape hatch,
/// so the text expansion is the only path there — this confirms it (and the empty-list false
/// predicate) works against a real server. Skipped unless <c>GAROA_MYSQL_CONN</c> is set.
/// </summary>
public class MySqlInExpansionTests
{
    private sealed class Widget
    {
        [Column("id")] public int Id { get; set; }
        [Column("name")] public string? Name { get; set; }
    }

    private static string? ConnectionString => Environment.GetEnvironmentVariable("GAROA_MYSQL_CONN");

    private static MySqlConnection Open()
    {
        Skip.If(string.IsNullOrWhiteSpace(ConnectionString), "Set GAROA_MYSQL_CONN to run MySQL integration tests.");
        var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    private static void Seed(MySqlConnection db)
    {
        db.Execute("DROP TABLE IF EXISTS widgets;");
        db.Execute("CREATE TABLE widgets (id int PRIMARY KEY, name varchar(50));");
        db.Execute("INSERT INTO widgets (id, name) VALUES (1,'Bolt'),(2,'Nut'),(3,'Screw'),(4,'Washer');");
    }

    [SkippableFact]
    public void Expands_a_list_into_an_IN_clause()
    {
        using MySqlConnection db = Open();
        Seed(db);

        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE id IN @ids ORDER BY id;",
            new { ids = new[] { 1, 3 } });

        Assert.Equal(new[] { 1, 3 }, rows.Select(w => w.Id));
    }

    [SkippableFact]
    public void Empty_list_returns_no_rows_without_a_syntax_error()
    {
        using MySqlConnection db = Open();
        Seed(db);

        // IN () is a syntax error in MySQL; the binder must rewrite to a false predicate instead.
        List<Widget> rows = db.Query<Widget>(
            "SELECT id, name FROM widgets WHERE id IN @ids;",
            new { ids = Array.Empty<int>() });

        Assert.Empty(rows);
    }
}
