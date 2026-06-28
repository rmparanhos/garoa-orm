# Garoa ORM — Roadmap

Garoa ORM is a .NET data-mapping library, an alternative to Dapper focused on
performance, simplicity and clarity. The name references TypeScript's Drizzle ORM
(*garoa* = *drizzle* in Portuguese).

This document tracks everything that has been requested for the project. It is the
single source of truth for scope — update it whenever new work is requested or
completed.

## Conventions

- **All project artifacts are written in English** — code, comments, docs, commit
  messages, issue/PR text.
- Keep this roadmap current: when something is requested, add it here; when it ships,
  check it off.

## Status legend

- [x] Done
- [~] In progress
- [ ] Planned / not started

---

## v1 — Core API

The v1 surface is intentionally tiny:

| Operation                | Method                          | Notes                                   |
| ------------------------ | ------------------------------- | --------------------------------------- |
| SELECT                   | `Query<T>` → `List<T>`          | Cached mapper                           |
| INSERT/UPDATE/DELETE     | `Execute`                       | Returns rows affected                   |
| Bulk insert (high volume)| `BulkInsert<T>(IEnumerable<T>)` | Streaming, never materialised in memory |

Explicitly **out of scope for v1**: `DynamicParameters`, `GridReader`, multi-map.
(`QueryFirst`/`QueryFirstOrDefault` have since shipped — see "Requested but not yet scheduled";
the stricter `QuerySingle*` remain deferred.)

### Result mapping

- [x] Expression trees compiled at runtime, cached by type + columns.
- [x] Use `DbDataReader.GetFieldValue<T>()` directly — delegate type handling to the
  provider (natively resolves DateOnly/TimeOnly on PostgreSQL).
- [x] No IL emission.
- [x] Error messages identify the **correct** column by name + ordinal on a failed
  conversion (fixes the Dapper bug where the previous column was reported).
- [x] Snake_case / `[Column]` matching, nullable + enum handling.
- [x] **Compile-time mapper via source generator** (`[GaroaMapped]`). A Roslyn
  `IIncrementalGenerator` (`Garoa.SourceGenerator`, shipped as an analyzer inside the
  `Garoa` package) emits an `IGaroaRowMapper<T>` per annotated type — typed reader getters
  (`GetInt64`, …) for BCL types, `GetFieldValue<T>` for provider-resolved ones (DateOnly).
  No runtime `.Compile()`, Native-AOT/trimming friendly, same mapping semantics and
  column-accurate errors. Opt-in; unmarked types keep using the runtime mapper. The runtime
  prefers the generated mapper automatically (self-registered via a module initializer).

### Query / Execute

- [x] `Query<T>` extension over `DbConnection` (sync + async).
- [x] `Execute` extension over `DbConnection` (sync + async), returns rows affected.
- [x] Basic parameter binding from an anonymous object (no `DynamicParameters` class).
- [x] Correct connection lifetime handling — open if closed, close what we opened, no
  connection leaks.

### Bulk insert

- [x] Uniform `BulkInsert` / `BulkInsertAsync` surface per provider; shared streaming
  plumbing (`BulkColumnSet<T>`, `ObjectDataReader<T>`) lives in core. Streams row-by-row;
  never builds a giant SQL string; never materialises the sequence.
- [x] PostgreSQL provider via Npgsql COPY protocol (`NpgsqlBinaryImporter`).
- [x] MySQL provider via `MySqlBulkCopy`.
- Both require only normal INSERT privileges — no DDL / special grants. (MySQL also needs
  `AllowLoadLocalInfile=True` + server `local_infile=ON`, which `MySqlBulkCopy` relies on.)
- [x] Naming convention for the write side, so snake_case tables don't require `[Column]`/explicit
  columns. `BulkInsert` derives column names via `GaroaDefaults.BulkNamingConvention`, defaulting to
  `SnakeCase` (`BirthDate` → `birth_date`); `[Column]` and explicit `columns` always win. Set it to
  `MemberName` to emit members verbatim. Applied in the shared `BulkColumnSet` selection, so both
  the MySQL reader path and the PostgreSQL typed COPY writer honour it identically.

### Connection pool & timeout

- [x] Correct pool management — no connection leaks (open-if-closed / close-what-we-opened on
  every operation, including bulk).
- [x] Timeout configurable per-operation and globally. Every `Query`/`Execute`/`BulkInsert`
  overload takes a `commandTimeout` (seconds); when omitted it falls back to the process-wide
  `GaroaDefaults.CommandTimeoutSeconds` (null = the provider default, 0 = no timeout). The default
  flows to ADO.NET commands (`Query`/`Execute`), the PostgreSQL COPY writer
  (`NpgsqlBinaryImporter.Timeout`) and the MySQL bulk copy (`MySqlBulkCopy.BulkCopyTimeout`).
- [ ] Avoid the Dapper MySQL issue where connections stay in an invalid state after a
  timeout.

### Providers (v1)

- [x] PostgreSQL (Npgsql) — bulk insert. Provider package: `Garoa.PostgreSQL`. Rows are written
  through a compiled, typed COPY writer (`NpgsqlCopyWriter<T>`) that calls `Write<T>` per column, so
  value types are never boxed — bulk allocation drops from ~74 B/row to a near-constant few KB,
  matching a hand-written COPY (the benchmark `ManualCopy` floor). Column selection is shared with
  the runtime fill (`BulkColumnSet<T>.SelectColumns`) so both stay in sync.
- [x] MySQL (MySqlConnector) — bulk insert. Provider package: `Garoa.MySql`. (Boxing is inherent
  here: `MySqlBulkCopy` consumes a `DbDataReader`, whose `GetValue` returns `object`.)

---

## Tooling, CI/CD & infrastructure

- [x] Solution + project structure (`src/`, `tests/`, `Directory.Build.props`).
- [x] Source generator project (`src/Garoa.SourceGenerator`, netstandard2.0 Roslyn analyzer),
  referenced as an analyzer by the core and packed into the `Garoa` NuGet under
  `analyzers/dotnet/cs`.
- [x] Unit tests for the mapper (incl. DateOnly/TimeOnly, null handling, enums, wrong-
  column error message), the **generated** mapper (same coverage, asserts the generator ran),
  end-to-end tests over SQLite + bulk core (`BulkColumnSet`, `ObjectDataReader`).
- [x] Integration tests against live PostgreSQL + MySQL (`Garoa.IntegrationTests`, skipped
  unless `GAROA_PG_CONN` / `GAROA_MYSQL_CONN` are set).
- [x] CI: build + unit test, plus an integration job with PostgreSQL + MySQL service
  containers (`.github/workflows/ci.yml`).
- [x] CI/CD: **publish the NuGet packages** (`Garoa`, `Garoa.PostgreSQL`, `Garoa.MySql`) on a
  `v*.*.*` tag (`.github/workflows/publish.yml`; requires the `NUGET_API_KEY` secret).
- [x] Performance CI: relative benchmark Garoa vs Dapper using BenchmarkDotNet
  `[Baseline]` (`benchmarks/Garoa.Benchmarks`). Runs on push to `main` **and on PRs
  targeting `main`** (trigger added). Threshold set to **1.30x** (`check_threshold.py` +
  `GAROA_BENCH_THRESHOLD`). Results published as an Actions artifact, committed to the
  `benchmark-results` branch for long-term tracking, and posted as a PR comment.
  - Baseline numbers: Garoa is ~11% faster at 1 row, ~8–12% slower at 100–1000 rows, and
    allocates ~25–31% less memory than Dapper.
  - [x] Benchmark over real **PostgreSQL** and **MySQL** connections in addition to SQLite
    (`PostgresQueryBenchmarks`, `MySqlQueryBenchmarks` — same service containers as
    integration tests).
  - [x] Add a bulk-insert benchmark (Garoa bulk vs Dapper multi-row INSERT):
    `PostgresBulkInsertBenchmarks` / `MySqlBulkInsertBenchmarks` compare streaming `BulkInsert`
    (COPY / `MySqlBulkCopy`, `GaroaBulk`) against a chunked multi-row `INSERT ... VALUES` executed
    through Dapper (`Dapper`, the `[Baseline]`) for 1 000 / 10 000 rows. The baseline is a competent
    multi-row insert, not a per-row `Execute` loop (an anti-pattern, deliberately not measured). The
    CI gate `GAROA_BULK_THRESHOLD` holds `GaroaBulk` within `1.50x` of the Dapper baseline (expected
    well under 1; the bound is loose because MySQL bulk-copy at the 1000-row batch is noisy on CI).

---

## Long-term vision

A "Drizzle in C#": a headless ORM with a query builder that reads like SQL but is
type-safe. C# source generators give an edge over TypeScript here — stronger type
safety with errors at compile time.

The source-generator infrastructure landed first as the compile-time mapper
(`[GaroaMapped]` → `IGaroaRowMapper<T>`); it's the foundation the type-safe query
builder will build on.

- [x] Closing the read-mapping gap with Dapper on large result sets: the benchmark now
  measures a `[GaroaMapped]` type (`GaroaGenerated` row) alongside the runtime mapper,
  isolating the source generator's effect (typed getters vs the generic `GetFieldValue<T>`
  dispatch) across SQLite, PostgreSQL and MySQL.

---

## Requested but not yet scheduled

(Capture ad-hoc requests here as they arrive, before they're slotted above.)

### Planned post-v1 API additions

The guiding rule: each is a **thin shell over the existing core** (mapper cache, `ParameterBinder`,
connection-lifetime handling, `ObjectDataReader`/`BulkColumnSet`). Anything that would need a
*parallel* stack to what we already have is a bloat warning and gets questioned first.

- [x] **`QueryFirst` / `QueryFirstOrDefault`** (sync + async) — **implemented**. The workhorse
  single-row reads
  (`QueryFirstOrDefault` = "fetch by id → entity or `null`", the most common query of all). More
  ergonomic and more efficient than `Query<T>(...).FirstOrDefault()`, which materialises the whole
  result set. One shared private core: read the first row via `CommandBehavior.SingleRow` (the DB
  streams a single row, no `List` built), then either return it or, when empty, *throw* (`First`) or
  return `default(T)` (`FirstOrDefault`). Reuses `Mapper<T>` unchanged — **no new mapping code**. Low
  bloat risk; first candidate. Note: `SingleRow` is a client-side hint, **not** `LIMIT 1`/`TOP 1` —
  for the server to actually stop early, put the limit in your SQL. The method never injects it
  (that would be SQL rewriting).
- [ ] **`QuerySingle` / `QuerySingleOrDefault`** — deferred until asked. They assert *exactly one*
  row, which is a narrower need and costs an extra fetch (they must read a **second** row via
  `SingleResult` just to reject `>1`). For a PK lookup you already trust the cardinality, so
  `QueryFirstOrDefault` is the cheaper, idiomatic choice; `Query<T>` already lets you check `.Count`
  and throw your own way. Same shared core as the `First*` pair when it lands.
- [x] **`IN` expansion** (`WHERE id IN @ids`) — scoped tightly so it never becomes a SQL rewriter.
  Implemented in `ParameterBinder`: a parameter whose value is a non-string, non-`byte[]`
  `IEnumerable` has its `@name` token replaced with `(@name0, @name1, …)` and one parameter added per
  element. An empty sequence rewrites to a guaranteed-false predicate (`(SELECT 1 WHERE 1=0)`), never
  `IN ()`. The token scan is a single case-insensitive regex with word-boundary and `@@` guards — a
  deliberate substitution, not a parser. Built primarily for **MySQL** (which has no `= ANY` array
  escape hatch) and Dapper migrants; on PostgreSQL `= ANY(@ids)` with a native array parameter avoids
  expansion entirely (single cached plan, empty array handled natively) and is preferable there.
  Deliberately omits Dapper's power-of-two list padding (a plan-cache optimisation) for now.
- [ ] **`Garoa.SqlServer` bulk insert** via `SqlBulkCopy` (`Microsoft.Data.SqlClient`) — desired,
  deferred. Reuses the existing `ObjectDataReader<T>` + `BulkColumnSet` (same shape as the MySQL
  provider), so it is mostly a new package plus integration tests. Dev and CI cost nothing: SQL Server
  Developer Edition is free, and the official Docker image (`mcr.microsoft.com/mssql/server`) runs as
  a CI service container exactly like the PG/MySQL ones. Production licensing is the consumer's
  concern, not Garoa's. (`Query`/`Execute` already work against SQL Server today — only bulk is
  provider-specific.)
- [x] **`BulkUpsert<T>` — high-volume upsert** — **implemented for PostgreSQL**
  (`NpgsqlBulkUpsertExtensions.BulkUpsert`/`BulkUpsertAsync`). `BulkInsert`/COPY only appends, so
  high-volume upsert meant hand-rolling a staging table + a set-based merge; this mechanises it:
  (1) create a temp staging table (`CREATE TEMP TABLE ... AS SELECT cols FROM target WITH NO DATA`,
  so only the written columns, no copied NOT NULL), (2) stream the rows in via the typed COPY writer
  (`NpgsqlCopyWriter<T>`, no boxing), (3) one set-based `INSERT ... SELECT ... ON CONFLICT (...) DO
  UPDATE` from staging, (4) drop the staging table. API
  shape: target table, rows, conflict-key columns, optional update columns (default: all non-key
  columns). Reuses the bulk core, but it is a **thicker shell than `BulkInsert`** because the upsert
  dialect diverges per provider and the "conflict key" concept does not map 1:1:
    - PostgreSQL: `INSERT ... SELECT ... FROM staging ON CONFLICT (keys) DO UPDATE SET col = EXCLUDED.col` — keys are named.
    - MySQL: `INSERT ... SELECT ... FROM staging ON DUPLICATE KEY UPDATE col = VALUES(col)` — fires on any unique/PK index; keys are *not* named.
  Single-row / moderate-batch upsert needs no feature — a multi-row `INSERT ... ON CONFLICT` already
  runs through `Execute` today (worth documenting as a pattern). A generated single-row `Upsert(obj)`
  stays **out** (SQL generation + a per-dialect matrix = bloat). Built **benchmark-first**:
  `PostgresBulkUpsertBenchmarks` confirmed the staging+COPY approach beats a chunked multi-row
  `INSERT ... ON CONFLICT` (~1.4x at 1k rows, ~2x at 10k) while allocating a near-constant few KB,
  which justified the thicker API.
  - [x] **MySQL** (`MySqlBulkUpsertExtensions.BulkUpsert`/`BulkUpsertAsync`) — staging via
    `CREATE TEMPORARY TABLE ... AS SELECT ... WHERE 1=0`, filled with `MySqlBulkCopy`, then one
    set-based `INSERT ... SELECT ... ON DUPLICATE KEY UPDATE col = VALUES(col)`. MySQL fires on **any**
    unique/PK index and does not name the conflict columns, so `conflictKeys` is **not** emitted into
    the SQL — it is kept only for call-site parity with PostgreSQL and to derive the default update set
    (all written columns except the keys). An empty update set becomes `INSERT IGNORE` (MySQL has no
    `DO NOTHING`). Covered by `MySqlBulkUpsertBenchmarks` (`Dapper` / `ManualStaging` / `GaroaBulk`,
    same bulk gate as the insert path) and live integration tests.
  - [ ] SQL Server (`MERGE`) remains deferred (needs the `Garoa.SqlServer` package first).
