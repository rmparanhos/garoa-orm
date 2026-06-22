using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Dapper;
using Garoa;
using Microsoft.Data.Sqlite;

namespace Garoa.Benchmarks;

/// <summary>
/// Relative read-mapping benchmark: Garoa vs Dapper running the exact same query over the same
/// in-memory SQLite connection. Dapper is the <see cref="BenchmarkAttribute.Baseline"/>, so the
/// reported <c>Ratio</c> is what we track — runner noise hits both rows equally, so the ratio is
/// stable even when absolute timings drift.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, warmupCount: 3, iterationCount: 8, launchCount: 1)]
public class QueryBenchmarks
{
    private const string Sql = "SELECT Id, Customer, Amount, Quantity, Status FROM orders";

    private SqliteConnection _connection = null!;

    [Params(1, 100, 1000)]
    public int Rows;

    [GlobalSetup]
    public void Setup()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        GaroaConnectionExtensions.Execute(_connection,
            "CREATE TABLE orders (Id INTEGER PRIMARY KEY, Customer TEXT, Amount REAL, Quantity INTEGER, Status TEXT);");

        using var tx = _connection.BeginTransaction();
        using SqliteCommand insert = _connection.CreateCommand();
        insert.CommandText =
            "INSERT INTO orders (Id, Customer, Amount, Quantity, Status) VALUES ($id, $customer, $amount, $quantity, $status);";
        SqliteParameter id = insert.Parameters.Add("$id", SqliteType.Integer);
        SqliteParameter customer = insert.Parameters.Add("$customer", SqliteType.Text);
        SqliteParameter amount = insert.Parameters.Add("$amount", SqliteType.Real);
        SqliteParameter quantity = insert.Parameters.Add("$quantity", SqliteType.Integer);
        SqliteParameter status = insert.Parameters.Add("$status", SqliteType.Text);

        for (int i = 1; i <= Rows; i++)
        {
            id.Value = i;
            customer.Value = $"Customer {i}";
            amount.Value = i * 1.5;
            quantity.Value = i % 10;
            status.Value = i % 2 == 0 ? "Open" : "Closed";
            insert.ExecuteNonQuery();
        }

        tx.Commit();
    }

    [GlobalCleanup]
    public void Cleanup() => _connection.Dispose();

    // Called statically because both libraries expose a Query<T> extension method.
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
