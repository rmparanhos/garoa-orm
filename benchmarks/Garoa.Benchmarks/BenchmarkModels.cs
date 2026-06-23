using System.Data.Common;

namespace Garoa.Benchmarks;

// Two identically-shaped POCOs so the benchmark can compare the two Garoa mapping paths over the
// same query: BenchOrder goes through the runtime expression-tree mapper, BenchOrderMapped through
// the compile-time source-generated mapper. Both must be public so the generated mapper (emitted
// into the Garoa.Generated namespace) can reference BenchOrderMapped.

public sealed class BenchOrder
{
    public long Id { get; set; }
    public string? Customer { get; set; }
    public double Amount { get; set; }
    public long Quantity { get; set; }
    public string? Status { get; set; }
}

[GaroaMapped]
public sealed class BenchOrderMapped
{
    public long Id { get; set; }
    public string? Customer { get; set; }
    public double Amount { get; set; }
    public long Quantity { get; set; }
    public string? Status { get; set; }
}

// Row written by the bulk-insert benchmarks. The [Column] attributes map the PascalCase members to
// the snake_case-free lowercase table columns so the COPY/bulk-copy column list and the multi-row
// INSERT both target the same physical layout. (Today the write side emits member/[Column] names
// verbatim, so the attribute is required here.)
public sealed class BenchBulkRow
{
    [Column("id")] public long Id { get; set; }
    [Column("customer")] public string? Customer { get; set; }
    [Column("amount")] public double Amount { get; set; }
    [Column("quantity")] public long Quantity { get; set; }
    [Column("status")] public string? Status { get; set; }

    /// <summary>Generates <paramref name="count"/> deterministic rows with ids 1..count.</summary>
    public static BenchBulkRow[] Generate(int count)
    {
        var rows = new BenchBulkRow[count];
        for (int i = 0; i < count; i++)
        {
            int n = i + 1;
            rows[i] = new BenchBulkRow
            {
                Id = n,
                Customer = $"Customer {n}",
                Amount = n * 1.5,
                Quantity = n % 10,
                Status = n % 2 == 0 ? "Open" : "Closed",
            };
        }

        return rows;
    }
}

// The hand-written "floor": maps the 5-column row to BenchOrder with constant ordinals and typed
// getters, mirroring exactly what the source generator emits inline
// (reader.IsDBNull(i) ? default : reader.GetX(i) per column). Comparing the Manual benchmark to
// GaroaGenerated isolates the framework's per-row machinery from the raw reader work: if they tie,
// the generated mapper is at the floor; the Manual-vs-Dapper ratio shows how much of any gap is the
// driver itself rather than Garoa.
internal static class ManualMapper
{
    public static List<BenchOrder> Read(DbConnection connection, string sql)
    {
        using DbCommand command = connection.CreateCommand();
        command.CommandText = sql;

        using DbDataReader reader = command.ExecuteReader();
        var results = new List<BenchOrder>();
        while (reader.Read())
        {
            results.Add(new BenchOrder
            {
                Id = reader.IsDBNull(0) ? default : reader.GetInt64(0),
                Customer = reader.IsDBNull(1) ? default : reader.GetString(1),
                Amount = reader.IsDBNull(2) ? default : reader.GetDouble(2),
                Quantity = reader.IsDBNull(3) ? default : reader.GetInt64(3),
                Status = reader.IsDBNull(4) ? default : reader.GetString(4),
            });
        }

        return results;
    }
}
