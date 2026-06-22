# Benchmarks

Relative performance of Garoa vs [Dapper](https://github.com/DapperLib/Dapper), measured in the
same run with Dapper as the BenchmarkDotNet `[Baseline]`. Both libraries run the *exact same*
query over the *same* connection, so the ADO.NET overhead is identical and the
**`Ratio` column is the meaningful number** — runner noise hits both rows equally, keeping the
ratio stable even when absolute timings drift.

## Read-mapping benchmarks

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

## Bulk-insert benchmarks

| Class                          | Connection  | Env var required   |
| ------------------------------ | ----------- | ------------------ |
| `PostgresBulkInsertBenchmarks` | PostgreSQL  | `GAROA_PG_CONN`    |
| `MySqlBulkInsertBenchmarks`    | MySQL       | `GAROA_MYSQL_CONN` (with `AllowLoadLocalInfile=True`) |

These measure writing `N ∈ {1 000, 10 000}` rows two ways (Dapper is the `[Baseline]`, as in the
read-mapping benchmarks):

| Method      | What it measures                                                                  |
| ----------- | --------------------------------------------------------------------------------- |
| `Dapper`    | A chunked multi-row `INSERT ... VALUES (...),(...)` executed through Dapper's `Execute` (baseline). Dapper has no bulk API, so this hand-written multi-row statement is the best a Dapper user can do. |
| `GaroaBulk` | Garoa's streaming `BulkInsert` — PostgreSQL binary `COPY` / MySQL `MySqlBulkCopy`. |

The fair comparison here is **`GaroaBulk` vs a competent Dapper multi-row insert**, not vs a per-row
`Execute` loop. A row-by-row loop (one round-trip and one commit per row) is a common anti-pattern
that is ~50–150× slower than either approach, but that gap is about row-by-row execution, not the
library — so it is deliberately *not* the baseline. The `Ratio` column is therefore "vs the Dapper
multi-row INSERT", and `GaroaBulk` should sit comfortably below 1: `COPY` / `MySqlBulkCopy` skip
per-statement SQL parsing, stream the rows, and never build a giant statement (hence far fewer
allocations). The CI regression gate tracks `GaroaBulk` against this baseline.

Each class pins `invocationCount: 1` with an `[IterationSetup]` that truncates the table, so each
iteration is exactly one full bulk load against an empty table (no primary-key clashes). They need a
real server and will throw in `[GlobalSetup]` if the env var is unset.

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
regresses past the threshold. Two gates run via [`check_threshold.py`](check_threshold.py):

- **Read mapping** — `Garoa` mean must stay within `1.30x` of `Dapper`'s mean
  (`GAROA_BENCH_THRESHOLD`).
- **Bulk insert** — `GaroaBulk` mean must stay within `1.20x` of the `Dapper` multi-row INSERT mean
  (`GAROA_BULK_THRESHOLD`, `--baseline Dapper --candidate GaroaBulk`). This is a loose safety net; in
  practice the ratio is well under 1.

Both thresholds are set in `.github/workflows/benchmark.yml`. The script keys each report by
benchmark class, so a gate silently skips any class that lacks both of its named methods (the bulk
gate ignores the read-mapping classes and vice versa).

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
