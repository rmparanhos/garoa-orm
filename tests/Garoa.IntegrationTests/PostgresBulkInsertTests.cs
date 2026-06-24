using Garoa;
using Npgsql;
using Xunit;

namespace Garoa.IntegrationTests;

/// <summary>
/// Live PostgreSQL coverage for streaming bulk insert. Skipped unless <c>GAROA_PG_CONN</c> is
/// set (CI provides it via a Postgres service container); set it locally to run against your own.
/// </summary>
public class PostgresBulkInsertTests
{
    private sealed class Person
    {
        [Column("id")] public int Id { get; set; }
        [Column("name")] public string? Name { get; set; }
        [Column("birth_date")] public DateOnly BirthDate { get; set; }
        [Column("manager_id")] public int? ManagerId { get; set; }
    }

    private static string? ConnectionString => Environment.GetEnvironmentVariable("GAROA_PG_CONN");

    private static NpgsqlConnection Open()
    {
        Skip.If(string.IsNullOrWhiteSpace(ConnectionString), "Set GAROA_PG_CONN to run PostgreSQL integration tests.");
        var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    [SkippableFact]
    public async Task BulkInsert_streams_rows_and_roundtrips_dateonly()
    {
        await using NpgsqlConnection db = Open();
        db.Execute("DROP TABLE IF EXISTS people;");
        db.Execute(
            "CREATE TABLE people (id int PRIMARY KEY, name text, birth_date date, manager_id int);");

        IEnumerable<Person> rows = Enumerable.Range(1, 1000).Select(i => new Person
        {
            Id = i,
            Name = $"Person {i}",
            BirthDate = new DateOnly(1990, 1, 1).AddDays(i),
            ManagerId = i == 1 ? null : 1,
        });

        // Exercises the COPY timeout path (NpgsqlBinaryImporter.Timeout).
        ulong written = await db.BulkInsertAsync("people", rows, commandTimeout: 30);
        Assert.Equal(1000UL, written);

        List<long> count = db.Query<long>("SELECT count(*) FROM people;");
        Assert.Equal(1000, count[0]);

        // DateOnly must round-trip natively — the original Dapper pain point.
        List<Person> first = db.Query<Person>(
            "SELECT id, name, birth_date, manager_id FROM people WHERE id = @Id", new { Id = 1 });
        Assert.Equal(new DateOnly(1990, 1, 2), first[0].BirthDate);
        Assert.Null(first[0].ManagerId);
    }

    [SkippableFact]
    public void BulkInsert_with_explicit_columns_skips_unlisted_columns()
    {
        using NpgsqlConnection db = Open();
        db.Execute("DROP TABLE IF EXISTS people;");
        db.Execute(
            "CREATE TABLE people (id serial PRIMARY KEY, name text, birth_date date, manager_id int);");

        var rows = new[]
        {
            new Person { Name = "Ada", BirthDate = new DateOnly(1815, 12, 10) },
            new Person { Name = "Alan", BirthDate = new DateOnly(1912, 6, 23) },
        };

        // Let the database assign id via serial: only write name + birth_date.
        ulong written = db.BulkInsert("people", rows, new[] { "name", "birth_date" }, commandTimeout: 30);
        Assert.Equal(2UL, written);

        List<string> names = db.Query<string>("SELECT name FROM people ORDER BY id;");
        Assert.Equal(new[] { "Ada", "Alan" }, names);
    }

    // No [Column] attributes: relies on the default snake_case naming convention.
    private sealed class Customer
    {
        public long Id { get; set; }
        public string? FullName { get; set; }
        public DateOnly SignupDate { get; set; }
    }

    [SkippableFact]
    public void BulkInsert_maps_pascalcase_members_to_snake_case_columns_by_default()
    {
        using NpgsqlConnection db = Open();
        db.Execute("DROP TABLE IF EXISTS customers;");
        db.Execute("CREATE TABLE customers (id bigint PRIMARY KEY, full_name text, signup_date date);");

        var rows = new[]
        {
            new Customer { Id = 1, FullName = "Ada Lovelace", SignupDate = new DateOnly(2020, 5, 1) },
            new Customer { Id = 2, FullName = "Alan Turing", SignupDate = new DateOnly(2021, 6, 23) },
        };

        // FullName -> full_name, SignupDate -> signup_date, with no annotations.
        ulong written = db.BulkInsert("customers", rows);
        Assert.Equal(2UL, written);

        List<Customer> back = db.Query<Customer>("SELECT id, full_name, signup_date FROM customers ORDER BY id;");
        Assert.Equal("Ada Lovelace", back[0].FullName);
        Assert.Equal(new DateOnly(2021, 6, 23), back[1].SignupDate);
    }

    private enum Priority : short { Low = 1, High = 2 }

    private sealed class Job
    {
        [Column("id")] public int Id { get; set; }
        [Column("priority")] public Priority Priority { get; set; }
        [Column("due")] public DateOnly? Due { get; set; }
    }

    [SkippableFact]
    public void BulkInsert_writes_enum_as_numeric_and_handles_nullable_value_type()
    {
        using NpgsqlConnection db = Open();
        db.Execute("DROP TABLE IF EXISTS jobs;");
        db.Execute("CREATE TABLE jobs (id int PRIMARY KEY, priority smallint, due date);");

        var rows = new[]
        {
            new Job { Id = 1, Priority = Priority.High, Due = new DateOnly(2030, 1, 1) },
            new Job { Id = 2, Priority = Priority.Low, Due = null },
        };

        ulong written = db.BulkInsert("jobs", rows);
        Assert.Equal(2UL, written);

        // Enum is written as its numeric backing value (smallint), via the typed COPY writer.
        List<short> priorities = db.Query<short>("SELECT priority FROM jobs ORDER BY id;");
        Assert.Equal(new short[] { 2, 1 }, priorities);

        // Nullable DateOnly round-trips, including the null.
        List<Job> back = db.Query<Job>("SELECT id, priority, due FROM jobs ORDER BY id;");
        Assert.Equal(Priority.High, back[0].Priority);
        Assert.Equal(new DateOnly(2030, 1, 1), back[0].Due);
        Assert.Null(back[1].Due);
    }
}
