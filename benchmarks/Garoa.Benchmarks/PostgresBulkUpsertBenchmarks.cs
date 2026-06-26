using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Dapper;
using Npgsql;

namespace Garoa.Benchmarks;

/// <summary>
/// High-volume upsert over a real PostgreSQL connection, comparing Garoa's <c>BulkUpsert</c> API
/// against the best a Dapper user can hand-write, plus the hand-written staging floor.
/// <list type="bullet">
///   <item><c>Dapper</c> (the <see cref="BenchmarkAttribute.Baseline"/>) — a chunked multi-row
///   <c>INSERT ... VALUES (...),(...) ON CONFLICT (id) DO UPDATE SET ...</c> executed through Dapper.
///   The best a Dapper user can hand-write without a bulk path.</item>
///   <item><c>ManualStaging</c> — the floor: a hand-written temp staging table, the rows streamed in
///   via binary <c>COPY</c>, then one set-based
///   <c>INSERT INTO target SELECT ... FROM staging ON CONFLICT (id) DO UPDATE</c>, then the staging
///   table dropped. This is the shape <c>BulkUpsert</c> wraps, so <c>GaroaBulk</c> vs
///   <c>ManualStaging</c> exposes the cost of Garoa's typed COPY writer and column reflection.</item>
///   <item><c>GaroaBulk</c> — the real <c>BulkUpsert</c> API. This is what the bulk threshold gates.</item>
/// </list>
/// The table is pre-populated with half the keys, so each upsert is ~50% updates / 50% inserts.
/// <para>
/// Pinned to one invocation per iteration (<c>invocationCount: 1</c>); an <c>[IterationSetup]</c>
/// resets the table to that half-populated baseline before each measured upsert. Requires
/// <c>GAROA_PG_CONN</c>; run with <c>--filter '*SQLite*'</c> to skip when no server is available.
/// </para>
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10, invocationCount: 1)]
public class PostgresBulkUpsertBenchmarks
{
    private const int ChunkSize = 1000;

    private const string UpdateSet =
        "customer = EXCLUDED.customer, amount = EXCLUDED.amount, " +
        "quantity = EXCLUDED.quantity, status = EXCLUDED.status";

    private NpgsqlConnection _connection = null!;
    private BenchBulkRow[] _rows = null!;       // the full batch to upsert (ids 1..Rows)
    private BenchBulkRow[] _baseline = null!;   // the half that already exists (ids 1..Rows/2)

    [Params(1_000, 10_000)]
    public int Rows;

    [GlobalSetup]
    public void Setup()
    {
        var connStr = Environment.GetEnvironmentVariable("GAROA_PG_CONN")
            ?? throw new InvalidOperationException(
                "GAROA_PG_CONN is not set. Run with --filter '*SQLite*' to skip database benchmarks.");

        _connection = new NpgsqlConnection(connStr);
        _connection.Open();

        Exec("""
            CREATE TABLE IF NOT EXISTS bench_upsert (
                id       BIGINT PRIMARY KEY,
                customer TEXT,
                amount   DOUBLE PRECISION,
                quantity BIGINT,
                status   TEXT
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
        CopyInto("COPY bench_upsert (id, customer, amount, quantity, status) FROM STDIN (FORMAT BINARY)", _baseline);
    }

    // Baseline: chunked multi-row INSERT ... ON CONFLICT DO UPDATE, executed through Dapper.
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

            sql.Append(" ON CONFLICT (id) DO UPDATE SET ").Append(UpdateSet);
            SqlMapper.Execute(_connection, sql.ToString(), parameters);
        }
    }

    // The floor: hand-written staging temp table + COPY + one set-based ON CONFLICT merge. This is
    // the shape BulkUpsert wraps, so GaroaBulk vs ManualStaging exposes Garoa's per-row overhead.
    [Benchmark]
    public void ManualStaging()
    {
        Exec("DROP TABLE IF EXISTS staging");
        Exec("CREATE TEMP TABLE staging (LIKE bench_upsert)");

        CopyInto("COPY staging (id, customer, amount, quantity, status) FROM STDIN (FORMAT BINARY)", _rows);

        Exec(
            "INSERT INTO bench_upsert (id, customer, amount, quantity, status) " +
            "SELECT id, customer, amount, quantity, status FROM staging " +
            "ON CONFLICT (id) DO UPDATE SET " + UpdateSet);

        Exec("DROP TABLE staging");
    }

    // The real Garoa API: typed COPY into its own staging table, then the set-based ON CONFLICT merge.
    // Default updateColumns = every written column except the conflict key, matching UpdateSet above.
    [Benchmark]
    public ulong GaroaBulk() => _connection.BulkUpsert("bench_upsert", _rows, conflictKeys: new[] { "id" });

    private void Exec(string sql)
    {
        using NpgsqlCommand cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private void CopyInto(string copyCommand, BenchBulkRow[] rows)
    {
        using NpgsqlBinaryImporter writer = _connection.BeginBinaryImport(copyCommand);
        foreach (BenchBulkRow r in rows)
        {
            writer.StartRow();
            writer.Write(r.Id);
            writer.Write(r.Customer!);
            writer.Write(r.Amount);
            writer.Write(r.Quantity);
            writer.Write(r.Status!);
        }

        writer.Complete();
    }
}
