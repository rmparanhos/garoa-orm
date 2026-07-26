# Garoa ORM

A lightweight, high-performance .NET data mapper — an alternative to Dapper focused on
**performance, simplicity and clarity**.

The name references TypeScript's [Drizzle ORM](https://orm.drizzle.team/) — *garoa* is
Portuguese for *drizzle* — and so does the direction: a **headless, SQL-first** library that
never hides the query from you, on its way to a type-safe query builder (see
[where this is going](#where-this-is-going)).

## Why Garoa

Garoa maps result sets with **runtime-compiled expression trees** (no IL emission). BCL types
read through the typed reader getters (`GetInt64`, `GetString`, …), which the JIT inlines — the
same speed class as a hand-written mapper. Types the provider must resolve itself (`DateOnly`,
`TimeOnly`, …) read through `DbDataReader.GetFieldValue<T>()`, delegating type handling to the
provider. That split solves several long-standing micro-ORM pain points directly:

- **`DateOnly` / `TimeOnly` work natively** on PostgreSQL (and anywhere the provider supports
  them) — no manual type handlers required.
- **Mapping errors name the right column.** When a conversion fails, the exception identifies
  the offending column by name and ordinal — not the previously-read one.
- **Bulk is a first-class operation**, not an afterthought: `BulkInsert` and `BulkUpsert` stream
  through the provider's native bulk protocol instead of building a giant `INSERT` string.
- **No fragile IL.** Mappers are compiled expression trees, cached by type + column layout — or
  generated at build time with `[GaroaMapped]`, which is Native-AOT friendly.

## Status

Pre-release. The API surface is intentionally small — see [`ROADMAP.md`](ROADMAP.md).

| Operation            | Method                           | Notes                                          |
| -------------------- | -------------------------------- | ---------------------------------------------- |
| SELECT               | `Query<T>` → `List<T>`           | Cached mapper; `IN @ids` lists expanded        |
| First row            | `QueryFirst[OrDefault]<T>`       | One-row fetch, no list built                   |
| Exactly one row      | `QuerySingle[OrDefault]<T>`      | Throws when more than one row comes back       |
| INSERT/UPDATE/DELETE | `Execute`                        | Returns rows affected                          |
| Bulk insert          | `BulkInsert<T>(rows)`            | Streaming, never materialised in memory        |
| Bulk upsert          | `BulkUpsert<T>(rows, conflictKeys)` | Staging table + one set-based merge         |

Every method ships with an `…Async` counterpart taking a `CancellationToken`.

Explicitly **out of scope** for now: `DynamicParameters`, `GridReader`, multi-map. Each would need
a parallel stack to what already exists, which is exactly the bloat Garoa is avoiding.

## Packages

| Package            | Contents                                                                        |
| ------------------ | ------------------------------------------------------------------------------- |
| `Garoa`            | Core: `Query`/`Execute`/`QueryFirst`/`QuerySingle`, the mapper + source generator, parameter binding, and the shared bulk plumbing. |
| `Garoa.PostgreSQL` | `BulkInsert` / `BulkUpsert` for PostgreSQL via Npgsql's binary `COPY` protocol.  |
| `Garoa.MySql`      | `BulkInsert` / `BulkUpsert` for MySQL via MySqlConnector's `MySqlBulkCopy`.      |

`Query`/`Execute` and friends work over **any** ADO.NET provider (SQL Server, SQLite, Oracle, …) —
only the bulk paths are provider-specific, because only they use a native bulk protocol.

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

### Single-row reads

Two families, differing only in how strict they are about cardinality:

```csharp
// First row, or null when there is none. The idiomatic "fetch by id → entity or null".
// Fetches a single row (CommandBehavior.SingleRow) — no List<T> is built.
Person? one = connection.QueryFirstOrDefault<Person>(
    "SELECT id, name, birth_date FROM people WHERE id = @Id LIMIT 1", new { Id = 1 });

// Same, but throws when the query returns nothing.
Person mustExist = connection.QueryFirst<Person>(
    "SELECT id, name, birth_date FROM people WHERE id = @Id LIMIT 1", new { Id = 1 });

// QuerySingle asserts the result is *unique*: it throws if a second row comes back.
// QuerySingleOrDefault still allows zero rows, but not two.
Person exactlyOne = connection.QuerySingle<Person>(
    "SELECT id, name, birth_date FROM people WHERE email = @Email", new { Email = "ada@x.io" });
```

`QuerySingle*` is stricter and slightly costlier — it reads a **second** row to enforce
uniqueness — so reach for `QueryFirst*` when you already trust the cardinality (a primary-key
lookup) and for `QuerySingle*` when "exactly one" is an invariant you want checked.

> Garoa never rewrites your SQL to add a `LIMIT`/`TOP 1`. `SingleRow` is a client-side hint; if you
> want the *server* to stop early, put the limit in the query yourself.

### `IN` lists

Pass a collection as a parameter and Garoa expands it for an `IN` clause — the `@ids` token becomes
`(@ids0, @ids1, …)` with one parameter per element:

```csharp
List<Person> some = connection.Query<Person>(
    "SELECT id, name FROM people WHERE id IN @ids", new { ids = new[] { 1, 2, 3 } });
```

- Only a non-string, non-`byte[]` `IEnumerable` is expanded; strings and byte arrays stay scalar.
- An **empty** list is rewritten to a guaranteed-false predicate, so the query returns no rows
  instead of throwing — `IN ()` (a syntax error) is never emitted.
- It's a small token substitution, not a SQL parser. On **PostgreSQL** prefer
  `WHERE id = ANY(@ids)` with a native array parameter — one cached plan regardless of list
  length, and empty arrays just work. Expansion is most useful on **MySQL**/SQLite, which have no
  array-parameter equivalent, and for porting existing Dapper code unchanged.

### Bulk insert

For high-volume inserts, `BulkInsert` streams rows straight to the server — it never builds a
giant `INSERT` string and never materialises the source sequence, so a million rows cost roughly
one row's worth of memory. Each provider package adds the extension to its own connection type:

```csharp
using Garoa; // brings the BulkInsert extension into scope

// PostgreSQL — Npgsql binary COPY. Returns the number of rows written.
await using var pg = new NpgsqlConnection(connectionString);
ulong written = await pg.BulkInsertAsync("people", people);

// MySQL — MySqlBulkCopy. The connection string needs AllowLoadLocalInfile=True.
await using var mysql = new MySqlConnection("...;AllowLoadLocalInfile=True");
long inserted = await mysql.BulkInsertAsync("people", people);

// Write a subset / control column order (e.g. let the DB assign an identity column):
await pg.BulkInsertAsync("people", people, columns: new[] { "name", "birth_date" });
```

On PostgreSQL each row goes through a compiled, typed `COPY` writer, so value types are **never
boxed** — a bulk load allocates a near-constant few KB regardless of row count.

### Bulk upsert

`COPY` only appends, so high-volume upsert normally means hand-rolling a staging table.
`BulkUpsert` mechanises exactly that: create a temp staging table, stream the rows in with the
same bulk path as `BulkInsert`, then merge with **one set-based statement**, then drop the staging
table. Both providers share the call shape:

```csharp
// PostgreSQL — INSERT ... SELECT ... ON CONFLICT (id) DO UPDATE.
ulong upserted = await pg.BulkUpsertAsync("people", people, conflictKeys: new[] { "id" });

// MySQL — INSERT ... SELECT ... ON DUPLICATE KEY UPDATE.
long mysqlUpserted = await mysql.BulkUpsertAsync("people", people, conflictKeys: new[] { "id" });

// On conflict, overwrite only some columns (the rest keep their stored values):
await pg.BulkUpsertAsync("people", people,
    conflictKeys: new[] { "id" },
    updateColumns: new[] { "name" });
```

- By default every written column **except** the conflict keys is overwritten on conflict. Passing
  an empty `updateColumns` means "insert what's missing, leave existing rows alone"
  (`DO NOTHING` / `INSERT IGNORE`).
- `conflictKeys` is matched against the written columns case- and underscore-insensitively, just
  like the `columns` argument — `"UserId"` finds the emitted column `user_id`. An unknown key
  throws instead of producing broken SQL.
- **PostgreSQL:** the keys are named in `ON CONFLICT (...)`, so they must match a unique or
  primary-key constraint. The keys must also be **unique within the batch** — PostgreSQL refuses
  to update the same row twice in one command (`ON CONFLICT DO UPDATE command cannot affect row a
  second time`). Garoa deliberately lets that error surface rather than silently dropping one of
  the duplicates: which row should win is your decision, so deduplicate the input first.
- **MySQL:** `ON DUPLICATE KEY UPDATE` fires on *any* unique/PK index and doesn't name columns, so
  `conflictKeys` never reaches the SQL — it only selects the default update columns, keeping the
  call site identical across providers.

> **Write-side column names follow a convention.** Unlike `Query<T>` — which has the result set's
> real column names and matches them case- and underscore-insensitively — the bulk paths must
> *emit* the destination names. By default each member is converted to `snake_case` (`BirthDate` →
> `birth_date`), matching the PostgreSQL/MySQL convention, so snake_case tables need no
> annotations. An explicit `[Column("…")]` or the `columns` argument always overrides. To emit
> member names verbatim, set
> `GaroaDefaults.BulkNamingConvention = BulkNamingConvention.MemberName`.

### Timeouts

Every operation takes a `commandTimeout` (in seconds) for that one call. To set it everywhere,
configure the process-wide default once at startup:

```csharp
// Global default for any call that doesn't pass its own commandTimeout.
GaroaDefaults.CommandTimeoutSeconds = 60;   // null = provider default (~30s); 0 = no timeout

// Per-call override always wins over the global default.
List<Person> slow = connection.Query<Person>(reportSql, commandTimeout: 300);
ulong written = await pg.BulkInsertAsync("people", people, commandTimeout: 600);
```

The timeout flows to the underlying ADO.NET command for `Query`/`Execute`/`QueryFirst`/
`QuerySingle`, to the PostgreSQL `COPY` writer and to `MySqlBulkCopy.BulkCopyTimeout` for bulk
inserts, and to every statement a `BulkUpsert` issues (staging DDL, load and merge).

### Mapping rules

- Column-to-member matching is case-insensitive and underscore-insensitive: `birth_date`
  binds to `BirthDate`.
- Use `[Column("name")]` for an explicit column name.
- `null` becomes the member's default (or `null` for nullable/reference types).
- Enums are read from their numeric column value.
- A public parameterless constructor is required; properties and public fields are both mapped.
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

- **No runtime `.Compile()`** — the mapper ships as plain compiled code, so the first query pays
  nothing to build it. Both mappers use the same read strategy (typed getters for BCL types,
  `GetFieldValue<T>` for provider-resolved ones), so the generator's edge is skipping the runtime
  compilation, not a faster per-row path.
- **Native AOT / trimming friendly** — no expression-tree compilation at runtime.
- **Identical semantics** to the runtime mapper: same case/underscore matching, `[Column]`,
  nullable and enum handling, and the same column-accurate error messages.

It's purely opt-in and per-type: unannotated types keep using the runtime mapper, and the
runtime prefers the generated mapper automatically (it self-registers at module load). The
generator ships inside the `Garoa` package as an analyzer — no extra dependency to add. Types
without a public parameterless constructor fall back to the runtime mapper.

## Performance

Garoa is benchmarked against Dapper **in the same run**, over the same connection, with Dapper as
the BenchmarkDotNet `[Baseline]` — so the ADO.NET overhead is identical and the ratio is the
meaningful number. CI runs the whole suite on every push to `main` and every PR, and fails the
build on a regression.

Reads (ratio = Garoa ÷ Dapper, lower is better; one sample CI run):

| Rows | SQLite   | PostgreSQL | MySQL    | Allocated (vs Dapper) |
| ---- | -------- | ---------- | -------- | --------------------- |
| 1    | **0.88** | 1.00       | 1.00     | 0.69 – 0.86           |
| 100  | **0.93** | 1.06       | 1.00     | 0.69 – 0.99           |
| 1000 | **0.97** | 1.18       | 1.01     | 0.68 – 1.00           |

On PostgreSQL at 1 000 rows a *hand-written* mapper scores 1.15 in the same run — that gap is the
driver's own typed-getter cost, not Garoa's machinery. The runtime and source-generated mappers
now land within noise of each other.

Writes, versus the best a Dapper user can hand-write (a chunked multi-row `INSERT`):

| Operation            | 1 000 rows | 10 000 rows | Allocated (vs Dapper) |
| -------------------- | ---------- | ----------- | --------------------- |
| `BulkInsert` (PG)    | **0.42**   | **0.35**    | ~0.001                |
| `BulkInsert` (MySQL) | **0.34**   | **0.57**    | ~0.04                 |
| `BulkUpsert` (PG)    | **0.79**   | **0.48**    | ~0.003                |
| `BulkUpsert` (MySQL) | **0.51**   | **0.77**    | ~0.04                 |

Both bulk paths sit at the hand-written floor (a raw `COPY`/`MySqlBulkCopy` loop is no faster),
while allocating a near-constant few KB instead of megabytes. See
[`benchmarks/`](benchmarks/README.md) for the full methodology and how to run them yourself.

## Where this is going

Garoa's long-term goal is the Drizzle idea in C#: a **headless ORM with a type-safe query builder
that still reads like SQL**. C# source generators make that stronger than in TypeScript — schema
mistakes become compile errors, not runtime surprises.

The generator infrastructure already landed as the compile-time mapper; the query builder is the
headline of the next release. Hand-written SQL stays a first-class citizen — the builder will
compose on top of it, never replace it. Details in [`ROADMAP.md`](ROADMAP.md).

## Building

```bash
dotnet build
dotnet test
```

Integration tests and database benchmarks are skipped unless `GAROA_PG_CONN` /
`GAROA_MYSQL_CONN` are set.

## License

MIT.
