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
