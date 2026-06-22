# Garoa ORM

A lightweight, high-performance .NET data mapper — an alternative to Dapper focused on
**performance, simplicity and clarity**.

The name references TypeScript's [Drizzle ORM](https://orm.drizzle.team/) — *garoa* is
Portuguese for *drizzle*.

## Why Garoa

Garoa maps result sets with **runtime-compiled expression trees** (no IL emission) and reads
every value through `DbDataReader.GetFieldValue<T>()`, delegating type handling to the
provider. That design choice solves several long-standing micro-ORM pain points directly:

- **`DateOnly` / `TimeOnly` work natively** on PostgreSQL (and anywhere the provider supports
  them) — no manual type handlers required.
- **Mapping errors name the right column.** When a conversion fails, the exception identifies
  the offending column by name and ordinal — not the previously-read one.
- **No fragile IL.** Mappers are compiled expression trees, cached by type + column layout.

## Status

Pre-release. The v1 surface is intentionally tiny — see [`ROADMAP.md`](ROADMAP.md).

| Operation              | Method                          | Notes                          |
| ---------------------- | ------------------------------- | ------------------------------ |
| SELECT                 | `Query<T>` → `List<T>`          | Cached mapper                  |
| INSERT/UPDATE/DELETE   | `Execute`                       | Returns rows affected          |
| Bulk insert            | `BulkInsert<T>(IEnumerable<T>)` | Streaming, never in memory     |

`Query<T>`, `Execute`, `BulkInsert<T>` and their `…Async` counterparts are implemented today.

## Packages

| Package            | Contents                                                              |
| ------------------ | -------------------------------------------------------------------- |
| `Garoa`            | Core: `Query`/`Execute`, the mapper, and the bulk-insert plumbing.   |
| `Garoa.PostgreSQL` | `BulkInsert` for PostgreSQL via Npgsql's binary COPY protocol.       |
| `Garoa.MySql`      | `BulkInsert` for MySQL via MySqlConnector's `MySqlBulkCopy`.         |

## Usage

```csharp
using Garoa;

// Works over any ADO.NET provider (Npgsql, MySqlConnector, SQLite, …).
await using var connection = new NpgsqlConnection(connectionString);

// SELECT — rows are mapped to T by a compiled, cached mapper.
List<Person> people = connection.Query<Person>(
    "SELECT id, name, birth_date FROM people WHERE active = @Active",
    new { Active = true });

// Scalars work too.
List<int> ids = connection.Query<int>("SELECT id FROM people");

// INSERT / UPDATE / DELETE — returns rows affected.
int affected = connection.Execute(
    "UPDATE people SET name = @Name WHERE id = @Id",
    new { Id = 1, Name = "Ada" });

// Async variants accept a CancellationToken.
List<Person> page = await connection.QueryAsync<Person>(
    "SELECT id, name, birth_date FROM people LIMIT @Take", new { Take = 50 });
```

### Bulk insert

For high-volume inserts, `BulkInsert` streams rows straight to the server — it never builds a
giant `INSERT` string and never materialises the source sequence, so a million rows cost roughly
one row's worth of memory. Each provider package adds the extension to its own connection type:

```csharp
// PostgreSQL — Npgsql binary COPY. Returns the number of rows written.
using Garoa; // brings the BulkInsert extension into scope

await using var pg = new NpgsqlConnection(connectionString);
ulong written = await pg.BulkInsertAsync("people", people);

// MySQL — MySqlBulkCopy. The connection string needs AllowLoadLocalInfile=True.
await using var mysql = new MySqlConnection("...;AllowLoadLocalInfile=True");
long inserted = await mysql.BulkInsertAsync("people", people);

// Write a subset / control column order (e.g. let the DB assign an identity column):
await pg.BulkInsertAsync("people", people, columns: new[] { "name", "birth_date" });
```

> **Column names on the write side are explicit.** Unlike `Query<T>` — which has the result
> set's real column names and matches them case- and underscore-insensitively — `BulkInsert`
> must *emit* the destination column names. They come from the member name or `[Column("…")]`,
> or the `columns` argument. For a snake_case table, annotate members with `[Column("birth_date")]`
> or pass explicit `columns`.

### Timeouts

Every `Query`/`Execute`/`BulkInsert` overload takes a `commandTimeout` (in seconds) for that one
call. To set it everywhere, configure the process-wide default once at startup:

```csharp
// Global default for any call that doesn't pass its own commandTimeout.
GaroaDefaults.CommandTimeoutSeconds = 60;   // null = provider default (~30s); 0 = no timeout

// Per-call override always wins over the global default.
List<Person> slow = connection.Query<Person>(reportSql, commandTimeout: 300);
ulong written = await pg.BulkInsertAsync("people", people, commandTimeout: 600);
```

The timeout flows to the underlying ADO.NET command for `Query`/`Execute`, to the PostgreSQL COPY
writer for `BulkInsert`, and to `MySqlBulkCopy.BulkCopyTimeout` for the MySQL provider.

### Mapping rules

- Column-to-member matching is case-insensitive and underscore-insensitive: `birth_date`
  binds to `BirthDate`.
- Use `[Column("name")]` for an explicit column name.
- `null` becomes the member's default (or `null` for nullable/reference types).
- Enums are read from their numeric column value.
- Connections that are closed when a call begins are opened and then closed again — callers
  never leak a connection they didn't open.

### Compile-time mapping (`[GaroaMapped]`)

By default Garoa compiles a mapper with expression trees the first time it sees a given
type + column layout. Annotate a type with `[GaroaMapped]` and the bundled source generator
emits that mapper at **build time** instead:

```csharp
using Garoa;

[GaroaMapped]
public sealed class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateOnly BirthDate { get; set; }
}

// Nothing else changes — Query<Person> automatically uses the generated mapper.
List<Person> people = connection.Query<Person>("SELECT id, name, birth_date FROM people");
```

What you get:

- **No runtime `.Compile()`** — the mapper ships as plain compiled code, so the first query
  pays nothing to build it.
- **Typed reader getters** (`GetInt64`, `GetString`, …) for BCL types, which the JIT inlines;
  `GetFieldValue<T>` is still used for provider-resolved types like `DateOnly`/`TimeOnly`, so
  that advantage is preserved.
- **Native AOT / trimming friendly** — no expression-tree compilation at runtime.
- **Identical semantics** to the runtime mapper: same case/underscore matching, `[Column]`,
  nullable and enum handling, and the same column-accurate error messages.

It's purely opt-in and per-type: unannotated types keep using the runtime mapper, and the
runtime prefers the generated mapper automatically (it self-registers at module load). The
generator ships inside the `Garoa` package as an analyzer — no extra dependency to add. A
public parameterless constructor is required; types without one fall back to the runtime mapper.

## Performance

Garoa is benchmarked against Dapper in the same run (Dapper as the `[Baseline]`). It is faster
on single-row reads, within ~12% of Dapper's IL mapper on large result sets, and consistently
allocates ~25–31% less memory. See [`benchmarks/`](benchmarks/README.md) for numbers and how to
run them; CI tracks the ratio on every push to `main` and fails on a regression.

## Building

```bash
dotnet build
dotnet test
```

## License

MIT.
