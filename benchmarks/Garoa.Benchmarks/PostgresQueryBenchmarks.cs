using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Dapper;
using Garoa;
using Npgsql;

namespace Garoa.Benchmarks;

/// <summary>
/// Garoa vs Dapper read-mapping benchmark over a real PostgreSQL connection.
/// Requires the <c>GAROA_PG_CONN</c> environment variable to point at a writable PostgreSQL
/// database. Run with <c>--filter '*SQLite*'</c> to skip this class when no server is available.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, warmupCount: 3, iterationCount: 8, launchCount: 1)]
public class PostgresQueryBenchmarks
{
    private const string Sql = "SELECT id, customer, amount, quantity, status FROM bench_orders";

    private NpgsqlConnection _connection = null!;

    [Params(1, 100, 1000)]
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
            CREATE TABLE IF NOT EXISTS bench_orders (
                id      BIGINT PRIMARY KEY,
                customer TEXT,
                amount   DOUBLE PRECISION,
                quantity BIGINT,
                status   TEXT
            )
            """);

        GaroaConnectionExtensions.Execute(_connection, "TRUNCATE bench_orders");

        using var writer = _connection.BeginBinaryImport(
            "COPY bench_orders (id, customer, amount, quantity, status) FROM STDIN (FORMAT BINARY)");

        for (int i = 1; i <= Rows; i++)
        {
            writer.StartRow();
            writer.Write((long)i);
            writer.Write($"Customer {i}");
            writer.Write(i * 1.5);
            writer.Write((long)(i % 10));
            writer.Write(i % 2 == 0 ? "Open" : "Closed");
        }

        writer.Complete();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        GaroaConnectionExtensions.Execute(_connection, "DROP TABLE IF EXISTS bench_orders");
        _connection.Dispose();
    }

    [Benchmark(Baseline = true)]
    public List<BenchOrder> Dapper() => SqlMapper.Query<BenchOrder>(_connection, Sql).AsList();

    // Runtime expression-tree mapper (BenchOrder is not [GaroaMapped]).
    [Benchmark]
    public List<BenchOrder> Garoa() => GaroaConnectionExtensions.Query<BenchOrder>(_connection, Sql);

    // Compile-time source-generated mapper (BenchOrderMapped is [GaroaMapped]).
    [Benchmark]
    public List<BenchOrderMapped> GaroaGenerated() =>
        GaroaConnectionExtensions.Query<BenchOrderMapped>(_connection, Sql);
}
