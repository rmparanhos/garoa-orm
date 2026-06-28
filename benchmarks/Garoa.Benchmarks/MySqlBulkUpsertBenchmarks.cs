using System.Data;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Dapper;
using Garoa;
using MySqlConnector;

namespace Garoa.Benchmarks;

/// <summary>
/// High-volume upsert over a real MySQL connection, comparing Garoa's <c>BulkUpsert</c> API against
/// the best a Dapper user can hand-write, plus the hand-written staging floor.
/// <list type="bullet">
///   <item><c>Dapper</c> (the <see cref="BenchmarkAttribute.Baseline"/>) — a chunked multi-row
///   <c>INSERT ... VALUES (...),(...) ON DUPLICATE KEY UPDATE ...</c> executed through Dapper.</item>
///   <item><c>ManualStaging</c> — the floor: a hand-written temp staging table filled via
///   <see cref="MySqlBulkCopy"/> (from a DataTable, the idiomatic raw usage), then one set-based
///   <c>INSERT ... SELECT ... ON DUPLICATE KEY UPDATE</c>, then the staging table dropped.</item>
///   <item><c>GaroaBulk</c> — the real <c>BulkUpsert</c> API. This is what the bulk threshold gates.</item>
/// </list>
/// The table is pre-populated with half the keys, so each upsert is ~50% updates / 50% inserts.
/// <para>
/// Pinned to one invocation per iteration (<c>invocationCount: 1</c>); an <c>[IterationSetup]</c>
/// resets the table to that half-populated baseline before each measured upsert. Requires
/// <c>GAROA_MYSQL_CONN</c> with <c>AllowLoadLocalInfile=True</c>; run with <c>--filter '*SQLite*'</c>
/// to skip when no server is available.
/// </para>
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10, invocationCount: 1)]
public class MySqlBulkUpsertBenchmarks
{
    private const int ChunkSize = 1000;

    private const string UpdateSet =
        "customer = VALUES(customer), amount = VALUES(amount), " +
        "quantity = VALUES(quantity), status = VALUES(status)";

    private MySqlConnection _connection = null!;
    private BenchBulkRow[] _rows = null!;       // the full batch to upsert (ids 1..Rows)
    private BenchBulkRow[] _baseline = null!;   // the half that already exists (ids 1..Rows/2)

    [Params(1_000, 10_000)]
    public int Rows;

    [GlobalSetup]
    public void Setup()
    {
        var connStr = Environment.GetEnvironmentVariable("GAROA_MYSQL_CONN")
            ?? throw new InvalidOperationException(
                "GAROA_MYSQL_CONN is not set. Run with --filter '*SQLite*' to skip database benchmarks.");

        _connection = new MySqlConnection(connStr);
        _connection.Open();

        Exec("""
            CREATE TABLE IF NOT EXISTS bench_upsert (
                id       BIGINT PRIMARY KEY,
                customer VARCHAR(200),
                amount   DOUBLE,
                quantity BIGINT,
                status   VARCHAR(20)
            )
            """);

        _rows = BenchBulkRow.Generate(Rows);
        _baseline = _rows.Take(Rows / 2).ToArray();   // first half pre-exists -> those upserts UPDATE
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        Exec("DROP TABLE IF EXISTS bench_upsert");
        _connection.Dispose();
    }

    // Reset to the half-populated baseline before every measured upsert.
    [IterationSetup]
    public void ResetTable()
    {
        Exec("TRUNCATE bench_upsert");
        CopyInto("bench_upsert", _baseline);
    }

    // Baseline: chunked multi-row INSERT ... ON DUPLICATE KEY UPDATE, executed through Dapper.
    [Benchmark(Baseline = true)]
    public void Dapper()
    {
        for (int offset = 0; offset < _rows.Length; offset += ChunkSize)
        {
            int count = Math.Min(ChunkSize, _rows.Length - offset);

            var sql = new StringBuilder("INSERT INTO bench_upsert (id, customer, amount, quantity, status) VALUES ");
            var parameters = new DynamicParameters();

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    sql.Append(',');
                sql.Append("(@id").Append(i)
                   .Append(",@customer").Append(i)
                   .Append(",@amount").Append(i)
                   .Append(",@quantity").Append(i)
                   .Append(",@status").Append(i).Append(')');

                BenchBulkRow r = _rows[offset + i];
                parameters.Add($"id{i}", r.Id);
                parameters.Add($"customer{i}", r.Customer);
                parameters.Add($"amount{i}", r.Amount);
                parameters.Add($"quantity{i}", r.Quantity);
                parameters.Add($"status{i}", r.Status);
            }

            sql.Append(" ON DUPLICATE KEY UPDATE ").Append(UpdateSet);
            SqlMapper.Execute(_connection, sql.ToString(), parameters);
        }
    }

    // The floor: hand-written staging temp table + MySqlBulkCopy + one set-based ON DUPLICATE KEY merge.
    [Benchmark]
    public void ManualStaging()
    {
        Exec("DROP TEMPORARY TABLE IF EXISTS staging");
        Exec("CREATE TEMPORARY TABLE staging AS SELECT id, customer, amount, quantity, status FROM bench_upsert WHERE 1 = 0");

        CopyInto("staging", _rows);

        Exec(
            "INSERT INTO bench_upsert (id, customer, amount, quantity, status) " +
            "SELECT id, customer, amount, quantity, status FROM staging " +
            "ON DUPLICATE KEY UPDATE " + UpdateSet);

        Exec("DROP TEMPORARY TABLE staging");
    }

    // The real Garoa API: staging + MySqlBulkCopy + the set-based ON DUPLICATE KEY merge.
    // Default updateColumns = every written column except the conflict key, matching UpdateSet above.
    [Benchmark]
    public long GaroaBulk() => _connection.BulkUpsert("bench_upsert", _rows, conflictKeys: new[] { "id" });

    private void Exec(string sql)
    {
        using MySqlCommand cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private void CopyInto(string destination, BenchBulkRow[] rows)
    {
        var table = new DataTable();
        table.Columns.Add("id", typeof(long));
        table.Columns.Add("customer", typeof(string));
        table.Columns.Add("amount", typeof(double));
        table.Columns.Add("quantity", typeof(long));
        table.Columns.Add("status", typeof(string));

        foreach (BenchBulkRow r in rows)
            table.Rows.Add(r.Id, r.Customer, r.Amount, r.Quantity, r.Status);

        var bulkCopy = new MySqlBulkCopy(_connection) { DestinationTableName = destination };
        for (int i = 0; i < table.Columns.Count; i++)
            bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(i, table.Columns[i].ColumnName));

        bulkCopy.WriteToServer(table);
    }
}
