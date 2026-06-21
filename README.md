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
| Bulk insert (planned)  | `BulkInsert<T>(IEnumerable<T>)` | Streaming, never in memory     |

`Query<T>`, `Execute` and their `…Async` counterparts are implemented today. Bulk insert and
the dedicated PostgreSQL/MySQL provider packages are next.

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

### Mapping rules

- Column-to-member matching is case-insensitive and underscore-insensitive: `birth_date`
  binds to `BirthDate`.
- Use `[Column("name")]` for an explicit column name.
- `null` becomes the member's default (or `null` for nullable/reference types).
- Enums are read from their numeric column value.
- Connections that are closed when a call begins are opened and then closed again — callers
  never leak a connection they didn't open.

## Building

```bash
dotnet build
dotnet test
```

## License

MIT.
