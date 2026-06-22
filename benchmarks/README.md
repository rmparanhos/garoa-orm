# Benchmarks

Relative performance of Garoa vs [Dapper](https://github.com/DapperLib/Dapper), measured in the
same run with Dapper as the BenchmarkDotNet `[Baseline]`. Both libraries run the *exact same*
query over the *same* connection, so the ADO.NET overhead is identical and the
**`Ratio` column is the meaningful number** — runner noise hits both rows equally, keeping the
ratio stable even when absolute timings drift.

## Benchmark classes

| Class                   | Connection     | Env var required   |
| ----------------------- | -------------- | ------------------ |
| `QueryBenchmarks`       | In-memory SQLite | *(none)*          |
| `PostgresQueryBenchmarks` | PostgreSQL   | `GAROA_PG_CONN`    |
| `MySqlQueryBenchmarks`  | MySQL          | `GAROA_MYSQL_CONN` |

All three classes benchmark the same scenario for `N ∈ {1, 100, 1000}` rows of a 5-column POCO,
with three methods (Dapper is the `[Baseline]`):

| Method           | What it measures                                                            |
| ---------------- | --------------------------------------------------------------------------- |
| `Dapper`         | Dapper's IL-based mapper (baseline).                                         |
| `Garoa`          | Garoa's **runtime** expression-tree mapper (`BenchOrder`, not annotated).   |
| `GaroaGenerated` | Garoa's **compile-time** source-generated mapper (`BenchOrderMapped`, `[GaroaMapped]`). |

The `GaroaGenerated` row isolates the effect of the source generator: it uses typed reader getters
instead of the generic `GetFieldValue<T>` dispatch, so comparing it against `Garoa` shows how much
of the gap versus Dapper the generator closes.

## Running locally

```bash
# All benchmarks (requires GAROA_PG_CONN and GAROA_MYSQL_CONN to be set):
dotnet run -c Release --project benchmarks/Garoa.Benchmarks -- --filter '*'

# SQLite only (no database server needed):
dotnet run -c Release --project benchmarks/Garoa.Benchmarks -- --filter '*SQLite*'

# PostgreSQL only:
GAROA_PG_CONN="Host=localhost;Username=postgres;Password=...;Database=garoa_bench" \
  dotnet run -c Release --project benchmarks/Garoa.Benchmarks -- --filter '*Postgres*'
```

Results land in `benchmarks/Garoa.Benchmarks/BenchmarkDotNet.Artifacts/results/`.

## Regression gate

CI runs these on every push to `main` and on every PR targeting `main`, and fails if Garoa
regresses past the threshold (Garoa mean must stay within `1.30x` of Dapper's mean). The check
is [`check_threshold.py`](check_threshold.py); the threshold is set via the
`GAROA_BENCH_THRESHOLD` env var in `.github/workflows/benchmark.yml`.

Results are also:
- Published as a **workflow artifact** on every run.
- **Committed** to the `benchmark-results` branch (one directory per run, timestamped) for
  long-term tracking.
- **Posted as a PR comment** when the workflow runs on a pull request.

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
