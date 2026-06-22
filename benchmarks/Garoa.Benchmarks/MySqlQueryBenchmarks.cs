using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Dapper;
using Garoa;
using MySqlConnector;

namespace Garoa.Benchmarks;

/// <summary>
/// Garoa vs Dapper read-mapping benchmark over a real MySQL connection.
/// Requires the <c>GAROA_MYSQL_CONN</c> environment variable to point at a writable MySQL
/// database. Run with <c>--filter '*SQLite*'</c> to skip this class when no server is available.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, warmupCount: 3, iterationCount: 8, launchCount: 1)]
public class MySqlQueryBenchmarks
{
    private const string Sql = "SELECT id, customer, amount, quantity, status FROM bench_orders";

    private MySqlConnection _connection = null!;

    [Params(1, 100, 1000)]
    public int Rows;

    [GlobalSetup]
    public void Setup()
    {
        var connStr = Environment.GetEnvironmentVariable("GAROA_MYSQL_CONN")
            ?? throw new InvalidOperationException(
                "GAROA_MYSQL_CONN is not set. Run with --filter '*SQLite*' to skip database benchmarks.");

        _connection = new MySqlConnection(connStr);
        _connection.Open();

        using var createCmd = _connection.CreateCommand();
        createCmd.CommandText = """
            CREATE TABLE IF NOT EXISTS bench_orders (
                id       BIGINT PRIMARY KEY,
                customer VARCHAR(200),
                amount   DOUBLE,
                quantity BIGINT,
                status   VARCHAR(20)
            )
            """;
        createCmd.ExecuteNonQuery();

        using var truncateCmd = _connection.CreateCommand();
        truncateCmd.CommandText = "TRUNCATE bench_orders";
        truncateCmd.ExecuteNonQuery();

        using var insertCmd = _connection.CreateCommand();
        insertCmd.CommandText =
            "INSERT INTO bench_orders (id, customer, amount, quantity, status) VALUES (@id, @customer, @amount, @quantity, @status)";

        var pId = insertCmd.Parameters.Add("@id", MySqlDbType.Int64);
        var pCustomer = insertCmd.Parameters.Add("@customer", MySqlDbType.VarChar);
        var pAmount = insertCmd.Parameters.Add("@amount", MySqlDbType.Double);
        var pQuantity = insertCmd.Parameters.Add("@quantity", MySqlDbType.Int64);
        var pStatus = insertCmd.Parameters.Add("@status", MySqlDbType.VarChar);

        for (int i = 1; i <= Rows; i++)
        {
            pId.Value = (long)i;
            pCustomer.Value = $"Customer {i}";
            pAmount.Value = i * 1.5;
            pQuantity.Value = (long)(i % 10);
            pStatus.Value = i % 2 == 0 ? "Open" : "Closed";
            insertCmd.ExecuteNonQuery();
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        using var dropCmd = _connection.CreateCommand();
        dropCmd.CommandText = "DROP TABLE IF EXISTS bench_orders";
        dropCmd.ExecuteNonQuery();
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
