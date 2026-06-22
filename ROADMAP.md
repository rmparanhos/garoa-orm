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

Explicitly **out of scope for v1**: `DynamicParameters`, `GridReader`, multi-map,
`QueryFirst`.

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
- [ ] Optional naming convention (e.g. auto snake_case) for the write side, so snake_case
  tables don't require `[Column]`/explicit columns. (Today the write side emits member /
  `[Column]` names verbatim.)

### Connection pool & timeout

- [x] Correct pool management — no connection leaks (open-if-closed / close-what-we-opened on
  every operation, including bulk).
- [ ] Timeout configurable per-operation and globally.
- [ ] Avoid the Dapper MySQL issue where connections stay in an invalid state after a
  timeout.

### Providers (v1)

- [x] PostgreSQL (Npgsql) — bulk insert. Provider package: `Garoa.PostgreSQL`.
- [x] MySQL (MySqlConnector) — bulk insert. Provider package: `Garoa.MySql`.

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
  - [x] Add a bulk-insert benchmark (Garoa providers vs naive multi-row INSERT):
    `PostgresBulkInsertBenchmarks` / `MySqlBulkInsertBenchmarks` compare streaming `BulkInsert`
    (COPY / `MySqlBulkCopy`, `GaroaBulk`) against a chunked multi-row `INSERT` (`NaiveInsert`,
    the `[Baseline]`) for 1 000 / 10 000 rows. Gated by `GAROA_BULK_THRESHOLD` (`GaroaBulk` must
    stay within `1.20x` of `NaiveInsert`; expected well under 1).

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
