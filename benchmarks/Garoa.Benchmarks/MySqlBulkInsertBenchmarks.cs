using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Garoa;
using MySqlConnector;

namespace Garoa.Benchmarks;

/// <summary>
/// Bulk insert over a real MySQL connection: Garoa's streaming <see cref="MySqlBulkCopy"/>
/// (<c>GaroaBulk</c>) vs the naive multi-row parameterised <c>INSERT</c> a developer would
/// otherwise hand-write (<c>NaiveInsert</c>, the <see cref="BenchmarkAttribute.Baseline"/>). Both
/// write the same rows to the same table, so the reported <c>Ratio</c> (GaroaBulk / NaiveInsert) is
/// what we track.
/// <para>
/// Pinned to one invocation per iteration (<c>invocationCount: 1</c>) with an
/// <c>[IterationSetup]</c> that truncates the table — so each measured iteration is exactly one
/// full bulk load against an empty table, with no primary-key clashes. (An <c>[IterationSetup]</c>
/// also makes BenchmarkDotNet set <c>UnrollFactor</c> to 1.) Requires <c>GAROA_MYSQL_CONN</c> with
/// <c>AllowLoadLocalInfile=True</c> (MySqlBulkCopy uses <c>LOAD DATA LOCAL INFILE</c>); run with
/// <c>--filter '*SQLite*'</c> to skip when no server is available.
/// </para>
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, warmupCount: 2, iterationCount: 10, invocationCount: 1)]
public class MySqlBulkInsertBenchmarks
{
    private const int ChunkSize = 1000;

    private MySqlConnection _connection = null!;
    private BenchBulkRow[] _rows = null!;

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

        using (MySqlCommand create = _connection.CreateCommand())
        {
            create.CommandText = """
                CREATE TABLE IF NOT EXISTS bench_bulk (
                    id       BIGINT PRIMARY KEY,
                    customer VARCHAR(200),
                    amount   DOUBLE,
                    quantity BIGINT,
                    status   VARCHAR(20)
                )
                """;
            create.ExecuteNonQuery();
        }

        _rows = BenchBulkRow.Generate(Rows);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        using MySqlCommand drop = _connection.CreateCommand();
        drop.CommandText = "DROP TABLE IF EXISTS bench_bulk";
        drop.ExecuteNonQuery();
        _connection.Dispose();
    }

    // Empty table before every measured iteration so neither method hits a primary-key clash.
    [IterationSetup]
    public void ResetTable()
    {
        using MySqlCommand truncate = _connection.CreateCommand();
        truncate.CommandText = "TRUNCATE bench_bulk";
        truncate.ExecuteNonQuery();
    }

    [Benchmark(Baseline = true)]
    public void NaiveInsert()
    {
        for (int offset = 0; offset < _rows.Length; offset += ChunkSize)
        {
            int count = Math.Min(ChunkSize, _rows.Length - offset);

            using MySqlCommand cmd = _connection.CreateCommand();
            var sql = new StringBuilder("INSERT INTO bench_bulk (id, customer, amount, quantity, status) VALUES ");

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
                cmd.Parameters.AddWithValue($"@id{i}", r.Id);
                cmd.Parameters.AddWithValue($"@customer{i}", (object?)r.Customer ?? DBNull.Value);
                cmd.Parameters.AddWithValue($"@amount{i}", r.Amount);
                cmd.Parameters.AddWithValue($"@quantity{i}", r.Quantity);
                cmd.Parameters.AddWithValue($"@status{i}", (object?)r.Status ?? DBNull.Value);
            }

            cmd.CommandText = sql.ToString();
            cmd.ExecuteNonQuery();
        }
    }

    [Benchmark]
    public long GaroaBulk() => _connection.BulkInsert("bench_bulk", _rows);
}
