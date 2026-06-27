using System.Data;
using Npgsql;

namespace Garoa;

/// <summary>
/// High-volume upsert for PostgreSQL. Streams the rows into a temporary staging table
/// via the binary COPY protocol (reusing <see cref="NpgsqlCopyWriter{T}"/>, so value types are never
/// boxed), then merges them into the target with a single set-based
/// <c>INSERT ... SELECT ... ON CONFLICT (...) DO UPDATE</c>. The source sequence is never fully
/// materialised and no giant statement is built — the same streaming guarantees as
/// <see cref="NpgsqlBulkInsertExtensions"/>, plus update-on-conflict.
/// </summary>
/// <remarks>
/// The benchmark (<c>PostgresBulkUpsertBenchmarks</c>) showed this staging approach is ~1.4x faster
/// at 1k rows and ~2x at 10k than a hand-written multi-row <c>INSERT ... ON CONFLICT</c>, while
/// allocating a near-constant few KB instead of megabytes.
/// </remarks>
public static class NpgsqlBulkUpsertExtensions
{
    /// <summary>Bulk-upserts <paramref name="rows"/> into <paramref name="table"/> and returns the number of rows streamed.</summary>
    /// <param name="connection">An open or closed connection; if closed it is opened and closed again.</param>
    /// <param name="table">Target table, used verbatim (qualify/quote it yourself if needed).</param>
    /// <param name="rows">The rows to upsert; streamed one at a time, never fully materialised.</param>
    /// <param name="conflictKeys">Columns of the unique/primary-key constraint to conflict on (named in <c>ON CONFLICT</c>).</param>
    /// <param name="updateColumns">Columns to overwrite on conflict; when null, every written column except the conflict keys. An empty set becomes <c>DO NOTHING</c>.</param>
    /// <param name="columns">Columns to write; when null, all readable members of <typeparamref name="T"/> are used.</param>
    /// <param name="commandTimeout">Timeout in seconds for each statement; when null, falls back to <see cref="GaroaDefaults.CommandTimeoutSeconds"/>.</param>
    /// <remarks>
    /// The conflict keys must be unique within a single call. PostgreSQL's <c>ON CONFLICT DO UPDATE</c>
    /// cannot affect the same target row twice in one command, so a batch containing duplicate conflict
    /// keys fails with <c>"ON CONFLICT DO UPDATE command cannot affect row a second time"</c>. This is
    /// deliberate: Garoa does not silently drop one of the duplicates, since which value should win is
    /// the caller's decision. Deduplicate the input yourself before calling if it may contain repeats.
    /// </remarks>
    public static ulong BulkUpsert<T>(
        this NpgsqlConnection connection,
        string table,
        IEnumerable<T> rows,
        IReadOnlyList<string> conflictKeys,
        IReadOnlyList<string>? updateColumns = null,
        IReadOnlyList<string>? columns = null,
        int? commandTimeout = null)
    {
        NpgsqlCopyWriter<T> rowWriter = Validate(connection, table, rows, conflictKeys, columns);
        string staging = StagingName();
        int? timeout = GaroaDefaults.ResolveCommandTimeout(commandTimeout);

        bool wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed)
            connection.Open();

        try
        {
            Exec(connection, CreateStaging(staging, table, rowWriter.ColumnNames), timeout);
            try
            {
                ulong written = 0;
                using (NpgsqlBinaryImporter writer = connection.BeginBinaryImport(StagingCopyCommand(staging, rowWriter)))
                {
                    if (timeout.HasValue)
                        writer.Timeout = TimeSpan.FromSeconds(timeout.Value);

                    foreach (T row in rows)
                    {
                        writer.StartRow();
                        rowWriter.WriteRow(writer, row);
                        written++;
                    }

                    writer.Complete();
                }

                Exec(connection, UpsertCommand(table, staging, rowWriter.ColumnNames, conflictKeys, updateColumns), timeout);
                return written;
            }
            finally
            {
                Exec(connection, $"DROP TABLE IF EXISTS {NpgsqlBulkInsertExtensions.Quote(staging)}", timeout);
            }
        }
        finally
        {
            if (wasClosed)
                connection.Close();
        }
    }

    /// <summary>Asynchronously bulk-upserts <paramref name="rows"/> into <paramref name="table"/>.</summary>
    /// <remarks>
    /// The conflict keys must be unique within a single call; duplicates fail with PostgreSQL's
    /// <c>"ON CONFLICT DO UPDATE command cannot affect row a second time"</c> rather than being silently
    /// dropped. See <see cref="BulkUpsert{T}"/> for details. Deduplicate the input yourself if needed.
    /// </remarks>
    public static async Task<ulong> BulkUpsertAsync<T>(
        this NpgsqlConnection connection,
        string table,
        IEnumerable<T> rows,
        IReadOnlyList<string> conflictKeys,
        IReadOnlyList<string>? updateColumns = null,
        IReadOnlyList<string>? columns = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        NpgsqlCopyWriter<T> rowWriter = Validate(connection, table, rows, conflictKeys, columns);
        string staging = StagingName();
        int? timeout = GaroaDefaults.ResolveCommandTimeout(commandTimeout);

        bool wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed)
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            await ExecAsync(connection, CreateStaging(staging, table, rowWriter.ColumnNames), timeout, cancellationToken).ConfigureAwait(false);
            try
            {
                ulong written = 0;
                NpgsqlBinaryImporter writer = await connection
                    .BeginBinaryImportAsync(StagingCopyCommand(staging, rowWriter), cancellationToken)
                    .ConfigureAwait(false);
                await using (writer.ConfigureAwait(false))
                {
                    if (timeout.HasValue)
                        writer.Timeout = TimeSpan.FromSeconds(timeout.Value);

                    foreach (T row in rows)
                    {
                        await writer.StartRowAsync(cancellationToken).ConfigureAwait(false);
                        rowWriter.WriteRow(writer, row);
                        written++;
                    }

                    await writer.CompleteAsync(cancellationToken).ConfigureAwait(false);
                }

                await ExecAsync(connection, UpsertCommand(table, staging, rowWriter.ColumnNames, conflictKeys, updateColumns), timeout, cancellationToken)
                    .ConfigureAwait(false);
                return written;
            }
            finally
            {
                await ExecAsync(connection, $"DROP TABLE IF EXISTS {NpgsqlBulkInsertExtensions.Quote(staging)}", timeout, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        finally
        {
            if (wasClosed)
                await connection.CloseAsync().ConfigureAwait(false);
        }
    }

    private static NpgsqlCopyWriter<T> Validate<T>(
        NpgsqlConnection connection, string table, IEnumerable<T> rows, IReadOnlyList<string> conflictKeys, IReadOnlyList<string>? columns)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(conflictKeys);
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentException("Table name must not be empty.", nameof(table));
        if (conflictKeys.Count == 0)
            throw new ArgumentException("At least one conflict-key column is required.", nameof(conflictKeys));

        return NpgsqlCopyWriter<T>.Get(columns);
    }

    // A unique temp-table name so a crashed prior call never collides; temp tables are per-session.
    private static string StagingName() => $"garoa_upsert_{Guid.NewGuid():N}";

    // A staging table holding exactly the written columns, with their target types but no
    // constraints (CREATE TABLE AS copies neither NOT NULL nor defaults), so a subset upsert never
    // trips a copied NOT NULL on a column we don't write.
    private static string CreateStaging(string staging, string table, string[] columns) =>
        $"CREATE TEMP TABLE {NpgsqlBulkInsertExtensions.Quote(staging)} AS SELECT {ColumnList(columns)} FROM {table} WITH NO DATA";

    private static string StagingCopyCommand<T>(string staging, NpgsqlCopyWriter<T> rowWriter) =>
        $"COPY {NpgsqlBulkInsertExtensions.Quote(staging)} ({ColumnList(rowWriter.ColumnNames)}) FROM STDIN (FORMAT BINARY)";

    private static string ColumnList(string[] columns) =>
        string.Join(", ", columns.Select(NpgsqlBulkInsertExtensions.Quote));

    private static string UpsertCommand(
        string table, string staging, string[] writtenColumns, IReadOnlyList<string> conflictKeys, IReadOnlyList<string>? updateColumns)
    {
        string cols = ColumnList(writtenColumns);
        string keys = string.Join(", ", conflictKeys.Select(NpgsqlBulkInsertExtensions.Quote));

        // Default: overwrite every written column that isn't part of the conflict key.
        string[] updates = (updateColumns
            ?? writtenColumns.Where(c => !conflictKeys.Contains(c, StringComparer.Ordinal)).ToArray()).ToArray();

        string action = updates.Length == 0
            ? "DO NOTHING"
            : "DO UPDATE SET " + string.Join(", ",
                updates.Select(c => $"{NpgsqlBulkInsertExtensions.Quote(c)} = EXCLUDED.{NpgsqlBulkInsertExtensions.Quote(c)}"));

        return $"INSERT INTO {table} ({cols}) SELECT {cols} FROM {NpgsqlBulkInsertExtensions.Quote(staging)} " +
               $"ON CONFLICT ({keys}) {action}";
    }

    private static void Exec(NpgsqlConnection connection, string sql, int? timeout)
    {
        using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        if (timeout.HasValue)
            command.CommandTimeout = timeout.Value;
        command.ExecuteNonQuery();
    }

    private static async Task ExecAsync(NpgsqlConnection connection, string sql, int? timeout, CancellationToken cancellationToken)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        if (timeout.HasValue)
            command.CommandTimeout = timeout.Value;
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
