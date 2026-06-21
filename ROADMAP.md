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
- [x] Unit tests for the mapper (incl. DateOnly/TimeOnly, null handling, enums, wrong-
  column error message) + end-to-end tests over SQLite + bulk core (`BulkColumnSet`,
  `ObjectDataReader`).
- [x] Integration tests against live PostgreSQL + MySQL (`Garoa.IntegrationTests`, skipped
  unless `GAROA_PG_CONN` / `GAROA_MYSQL_CONN` are set).
- [x] CI: build + unit test, plus an integration job with PostgreSQL + MySQL service
  containers (`.github/workflows/ci.yml`).
- [x] CI/CD: **publish the NuGet packages** (`Garoa`, `Garoa.PostgreSQL`, `Garoa.MySql`) on a
  `v*.*.*` tag (`.github/workflows/publish.yml`; requires the `NUGET_API_KEY` secret).
- [x] Performance CI: relative benchmark Garoa vs Dapper using BenchmarkDotNet
  `[Baseline]` (`benchmarks/Garoa.Benchmarks`). Runs on push to `main` (not on PRs).
  Threshold set to **1.30x** (`check_threshold.py` + `GAROA_BENCH_THRESHOLD`). Results
  published as an Actions artifact to track regressions.
  - Baseline numbers: Garoa is ~11% faster at 1 row, ~8–12% slower at 100–1000 rows, and
    allocates ~25–31% less memory than Dapper.
  - [ ] Add a bulk-insert benchmark (Garoa providers vs naive multi-row INSERT).

---

## Long-term vision

A "Drizzle in C#": a headless ORM with a query builder that reads like SQL but is
type-safe. C# source generators give an edge over TypeScript here — stronger type
safety with errors at compile time.

---

## Requested but not yet scheduled

(Capture ad-hoc requests here as they arrive, before they're slotted above.)
