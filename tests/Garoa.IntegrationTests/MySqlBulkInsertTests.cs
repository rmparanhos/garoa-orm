using Garoa;
using MySqlConnector;
using Xunit;

namespace Garoa.IntegrationTests;

/// <summary>
/// Live MySQL coverage for streaming bulk insert. Skipped unless <c>GAROA_MYSQL_CONN</c> is set
/// (CI provides it via a MySQL service container). The connection string must include
/// <c>AllowLoadLocalInfile=True</c>, which <see cref="MySqlBulkCopy"/> relies on.
/// </summary>
public class MySqlBulkInsertTests
{
    private sealed class Person
    {
        [Column("id")] public int Id { get; set; }
        [Column("name")] public string? Name { get; set; }
        [Column("birth_date")] public DateOnly BirthDate { get; set; }
        [Column("manager_id")] public int? ManagerId { get; set; }
    }

    private static string? ConnectionString => Environment.GetEnvironmentVariable("GAROA_MYSQL_CONN");

    private static MySqlConnection Open()
    {
        Skip.If(string.IsNullOrWhiteSpace(ConnectionString), "Set GAROA_MYSQL_CONN to run MySQL integration tests.");
        var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    [SkippableFact]
    public async Task BulkInsert_streams_rows()
    {
        await using MySqlConnection db = Open();
        db.Execute("DROP TABLE IF EXISTS people;");
        db.Execute(
            "CREATE TABLE people (id int PRIMARY KEY, name varchar(100), birth_date date, manager_id int NULL);");

        IEnumerable<Person> rows = Enumerable.Range(1, 1000).Select(i => new Person
        {
            Id = i,
            Name = $"Person {i}",
            BirthDate = new DateOnly(1990, 1, 1).AddDays(i),
            ManagerId = i == 1 ? null : 1,
        });

        // Exercises the bulk-copy timeout path (MySqlBulkCopy.BulkCopyTimeout).
        long written = await db.BulkInsertAsync("people", rows, commandTimeout: 30);
        Assert.Equal(1000, written);

        List<long> count = db.Query<long>("SELECT count(*) FROM people;");
        Assert.Equal(1000, count[0]);

        List<Person> first = db.Query<Person>(
            "SELECT id, name, birth_date, manager_id FROM people WHERE id = @Id", new { Id = 1 });
        Assert.Equal(new DateOnly(1990, 1, 2), first[0].BirthDate);
        Assert.Null(first[0].ManagerId);
    }
}
