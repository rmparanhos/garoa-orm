using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Dapper;
using Garoa;
using Npgsql;

namespace Garoa.Benchmarks;

/// <summary>
/// Bulk insert over a real PostgreSQL connection, comparing the two ways to write the same rows to
/// the same table:
/// <list type="bullet">
///   <item><c>Dapper</c> (the <see cref="BenchmarkAttribute.Baseline"/>) — a chunked multi-row
///   <c>INSERT ... VALUES (...),(...)</c> executed through Dapper's <c>Execute</c>. This is the
///   fair "Dapper insert": Dapper is just the executor, so hand-writing the multi-row statement is
///   the best a Dapper user can do without a bulk API (a per-row <c>Execute</c> loop is an
///   anti-pattern — one round-trip and one commit per row — and is deliberately not measured here).</item>
///   <item><c>GaroaBulk</c> — Garoa's streaming binary <c>COPY</c>.</item>
/// </list>
/// COPY beats the multi-row INSERT because it skips per-statement SQL parsing, streams a binary
/// payload, and never materialises a giant statement — so it is both faster and far lighter on
/// allocations. The CI regression gate tracks <c>GaroaBulk</c> against this <c>Dapper</c> baseline.
/// <para>
/// Pinned to one invocation per iteration (<c>invocationCount: 1</c>) with an
/// <c>[IterationSetup]</c> that truncates the table — so each measured iteration is exactly one
/// full bulk load against an empty table, with no primary-key clashes. (An <c>[IterationSetup]</c>
/// also makes BenchmarkDotNet set <c>UnrollFactor</c> to 1.) Requires <c>GAROA_PG_CONN</c>; run
/// with <c>--filter '*SQLite*'</c> to skip when no server is available.
/// </para>
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10, invocationCount: 1)]
public class PostgresBulkInsertBenchmarks
{
    // PostgreSQL caps a statement at 65535 parameters; chunking the multi-row INSERT keeps it well
    // under that and mirrors what real chunking helpers do.
    private const int ChunkSize = 1000;

    private NpgsqlConnection _connection = null!;
    private BenchBulkRow[] _rows = null!;

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

        GaroaConnectionExtensions.Execute(_connection, """
            CREATE TABLE IF NOT EXISTS bench_bulk (
                id       BIGINT PRIMARY KEY,
                customer TEXT,
                amount   DOUBLE PRECISION,
                quantity BIGINT,
                status   TEXT
            )
            """);

        _rows = BenchBulkRow.Generate(Rows);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        GaroaConnectionExtensions.Execute(_connection, "DROP TABLE IF EXISTS bench_bulk");
        _connection.Dispose();
    }

    // Empty table before every measured iteration so neither method hits a primary-key clash.
    [IterationSetup]
    public void ResetTable() => GaroaConnectionExtensions.Execute(_connection, "TRUNCATE bench_bulk");

    // The fair Dapper insert: a chunked multi-row INSERT VALUES executed through Dapper.
    [Benchmark(Baseline = true)]
    public void Dapper()
    {
        for (int offset = 0; offset < _rows.Length; offset += ChunkSize)
        {
            int count = Math.Min(ChunkSize, _rows.Length - offset);

            var sql = new StringBuilder("INSERT INTO bench_bulk (id, customer, amount, quantity, status) VALUES ");
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

            SqlMapper.Execute(_connection, sql.ToString(), parameters);
        }
    }

    [Benchmark]
    public ulong GaroaBulk() => _connection.BulkInsert("bench_bulk", _rows);
}
