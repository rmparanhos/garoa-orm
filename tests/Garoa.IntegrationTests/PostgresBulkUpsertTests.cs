using Garoa;
using Npgsql;
using Xunit;

namespace Garoa.IntegrationTests;

/// <summary>
/// Live PostgreSQL coverage for streaming bulk upsert (staging + COPY + set-based
/// <c>ON CONFLICT</c>). Skipped unless <c>GAROA_PG_CONN</c> is set.
/// </summary>
public class PostgresBulkUpsertTests
{
    private sealed class Account
    {
        [Column("id")] public long Id { get; set; }
        [Column("name")] public string? Name { get; set; }
        [Column("balance")] public double Balance { get; set; }
    }

    private static string? ConnectionString => Environment.GetEnvironmentVariable("GAROA_PG_CONN");

    private static NpgsqlConnection Open()
    {
        Skip.If(string.IsNullOrWhiteSpace(ConnectionString), "Set GAROA_PG_CONN to run PostgreSQL integration tests.");
        var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    private static void CreateSeeded(NpgsqlConnection db)
    {
        db.Execute("DROP TABLE IF EXISTS accounts;");
        db.Execute("CREATE TABLE accounts (id bigint PRIMARY KEY, name text, balance double precision);");
        db.Execute("INSERT INTO accounts (id, name, balance) VALUES (1, 'old-1', 10), (2, 'old-2', 20);");
    }

    [SkippableFact]
    public void BulkUpsert_inserts_new_rows_and_updates_existing_ones()
    {
        using NpgsqlConnection db = Open();
        CreateSeeded(db);

        var rows = new[]
        {
            new Account { Id = 1, Name = "new-1", Balance = 100 },   // conflict -> update
            new Account { Id = 2, Name = "new-2", Balance = 200 },   // conflict -> update
            new Account { Id = 3, Name = "new-3", Balance = 300 },   // no conflict -> insert
        };

        ulong written = db.BulkUpsert("accounts", rows, conflictKeys: new[] { "id" });
        Assert.Equal(3UL, written);

        List<Account> back = db.Query<Account>("SELECT id, name, balance FROM accounts ORDER BY id;");
        Assert.Equal(3, back.Count);
        Assert.Equal("new-1", back[0].Name);
        Assert.Equal(100, back[0].Balance);
        Assert.Equal("new-2", back[1].Name);
        Assert.Equal("new-3", back[2].Name);
        Assert.Equal(300, back[2].Balance);
    }

    [SkippableFact]
    public void BulkUpsert_updates_only_the_listed_columns_on_conflict()
    {
        using NpgsqlConnection db = Open();
        CreateSeeded(db);

        var rows = new[] { new Account { Id = 1, Name = "renamed", Balance = 9999 } };

        // On conflict, overwrite name only — balance must keep its existing value.
        db.BulkUpsert("accounts", rows, conflictKeys: new[] { "id" }, updateColumns: new[] { "name" });

        Account a = db.QueryFirst<Account>("SELECT id, name, balance FROM accounts WHERE id = 1;");
        Assert.Equal("renamed", a.Name);
        Assert.Equal(10, a.Balance);   // unchanged
    }

    [SkippableFact]
    public async Task BulkUpsertAsync_inserts_and_updates()
    {
        await using NpgsqlConnection db = Open();
        CreateSeeded(db);

        var rows = new[]
        {
            new Account { Id = 2, Name = "async-2", Balance = 222 },   // update
            new Account { Id = 4, Name = "async-4", Balance = 444 },   // insert
        };

        ulong written = await db.BulkUpsertAsync("accounts", rows, conflictKeys: new[] { "id" });
        Assert.Equal(2UL, written);

        Account two = db.QueryFirst<Account>("SELECT id, name, balance FROM accounts WHERE id = 2;");
        Assert.Equal("async-2", two.Name);
        Assert.Equal(222, two.Balance);

        Account? four = db.QueryFirstOrDefault<Account>("SELECT id, name, balance FROM accounts WHERE id = 4;");
        Assert.NotNull(four);
        Assert.Equal("async-4", four!.Name);
    }
}
