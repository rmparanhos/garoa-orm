# Benchmarks

Relative performance of Garoa vs [Dapper](https://github.com/DapperLib/Dapper), measured in the
same run with Dapper as the BenchmarkDotNet `[Baseline]`. Both libraries run the *exact same*
query over the *same* in-memory SQLite connection, so the ADO.NET overhead is identical and the
**`Ratio` column is the meaningful number** — runner noise hits both rows equally, keeping the
ratio stable even when absolute timings drift.

## Running locally

```bash
dotnet run -c Release --project benchmarks/Garoa.Benchmarks -- --filter '*'
```

Results land in `benchmarks/Garoa.Benchmarks/BenchmarkDotNet.Artifacts/results/`.

## Regression gate

CI runs these on every push to `main` (not on PRs) and fails if Garoa regresses past the
threshold (Garoa mean must stay within `1.30x` of Dapper's mean). The check is
[`check_threshold.py`](check_threshold.py); the threshold is set via the
`GAROA_BENCH_THRESHOLD` env var in `.github/workflows/benchmark.yml`. Full results are published
as a workflow artifact so the ratio can be tracked over time.

## Reference numbers

`QueryBenchmarks` — mapping N rows of a 5-column row to a POCO (one sample run on a 4-core
CI-class machine; your absolute numbers will differ, the ratios should not):

| Rows | Dapper      | Garoa       | Time ratio | Garoa allocated | Alloc ratio |
| ---- | ----------- | ----------- | ---------- | --------------- | ----------- |
| 1    | 8.07 μs     | 7.15 μs     | **0.89**   | 1.17 KB         | **0.75**    |
| 100  | 164.2 μs    | 178.1 μs    | **1.08**   | 16.76 KB        | **0.69**    |
| 1000 | 1,548 μs    | 1,727 μs    | **1.12**   | 153.9 KB        | **0.69**    |

Takeaways: Garoa is faster on single-row reads (less per-call setup), within ~12% of Dapper's
IL-based mapper on large result sets, and consistently **allocates ~25–31% less memory**.
