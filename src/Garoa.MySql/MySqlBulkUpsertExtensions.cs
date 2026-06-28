using System.Data;
using Garoa.Bulk;
using MySqlConnector;

namespace Garoa;

/// <summary>
/// High-volume upsert for MySQL. Streams the rows into a temporary staging table via
/// <see cref="MySqlBulkCopy"/> (so the source sequence is never materialised), then merges them into
/// the target with a single set-based <c>INSERT ... SELECT ... ON DUPLICATE KEY UPDATE</c>. The same
/// streaming guarantees as <see cref="MySqlBulkInsertExtensions"/>, plus update-on-conflict. The
/// connection string must enable local infile (<c>AllowLoadLocalInfile=True</c>), which
/// <see cref="MySqlBulkCopy"/> relies on.
/// </summary>
/// <remarks>
/// Two MySQL-specific differences from the PostgreSQL upsert are worth knowing:
/// <list type="bullet">
///   <item><c>ON DUPLICATE KEY UPDATE</c> fires on <em>any</em> unique or primary-key index and does
///   not name the conflict columns. <c>conflictKeys</c> is therefore not emitted into the SQL — it is
///   only used to derive the default set of columns to update (all written columns except the keys),
///   keeping the call site identical to the PostgreSQL <c>BulkUpsert</c>.</item>
///   <item>MySQL has no <c>DO NOTHING</c>; an empty update set becomes <c>INSERT IGNORE</c>, which
///   skips rows that would violate a unique/primary key.</item>
/// </list>
/// </remarks>
public static class MySqlBulkUpsertExtensions
{
    /// <summary>Bulk-upserts <paramref name="rows"/> into <paramref name="table"/> and returns the number of rows streamed.</summary>
    /// <param name="connection">An open or closed connection; if closed it is opened and closed again.</param>
    /// <param name="table">Target table name.</param>
    /// <param name="rows">The rows to upsert; streamed one at a time, never fully materialised.</param>
    /// <param name="conflictKeys">The unique/PK columns; not emitted into the SQL (MySQL fires on any unique index), only used to derive the default update columns.</param>
    /// <param name="updateColumns">Columns to overwrite on conflict; when null, every written column except the conflict keys. An empty set becomes <c>INSERT IGNORE</c>.</param>
    /// <param name="columns">Columns to write; when null, all readable members of <typeparamref name="T"/> are used.</param>
    /// <param name="commandTimeout">Timeout in seconds for each step; when null, falls back to <see cref="GaroaDefaults.CommandTimeoutSeconds"/>.</param>
    public static long BulkUpsert<T>(
        this MySqlConnection connection,
        string table,
        IEnumerable<T> rows,
        IReadOnlyList<string> conflictKeys,
        IReadOnlyList<string>? updateColumns = null,
        IReadOnlyList<string>? columns = null,
        int? commandTimeout = null)
    {
        BulkColumnSet<T> set = Validate(connection, table, rows, conflictKeys, columns);
        string staging = StagingName();
        int? timeout = GaroaDefaults.ResolveCommandTimeout(commandTimeout);

        bool wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed)
            connection.Open();

        try
        {
            Exec(connection, CreateStaging(staging, table, set.ColumnNames), timeout);
            try
            {
                long written;
                MySqlBulkCopy bulkCopy = CreateBulkCopy(connection, staging, set, timeout);
                using (var reader = new ObjectDataReader<T>(rows, set))
                    written = bulkCopy.WriteToServer(reader).RowsInserted;

                Exec(connection, UpsertCommand(table, staging, set.ColumnNames, conflictKeys, updateColumns), timeout);
                return written;
            }
            finally
            {
                Exec(connection, $"DROP TEMPORARY TABLE IF EXISTS {Quote(staging)}", timeout);
            }
        }
        finally
        {
            if (wasClosed)
                connection.Close();
        }
    }

    /// <summary>Asynchronously bulk-upserts <paramref name="rows"/> into <paramref name="table"/>.</summary>
    public static async Task<long> BulkUpsertAsync<T>(
        this MySqlConnection connection,
        string table,
        IEnumerable<T> rows,
        IReadOnlyList<string> conflictKeys,
        IReadOnlyList<string>? updateColumns = null,
        IReadOnlyList<string>? columns = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        BulkColumnSet<T> set = Validate(connection, table, rows, conflictKeys, columns);
        string staging = StagingName();
        int? timeout = GaroaDefaults.ResolveCommandTimeout(commandTimeout);

        bool wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed)
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            await ExecAsync(connection, CreateStaging(staging, table, set.ColumnNames), timeout, cancellationToken).ConfigureAwait(false);
            try
            {
                long written;
                MySqlBulkCopy bulkCopy = CreateBulkCopy(connection, staging, set, timeout);
                using (var reader = new ObjectDataReader<T>(rows, set))
                    written = (await bulkCopy.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false)).RowsInserted;

                await ExecAsync(connection, UpsertCommand(table, staging, set.ColumnNames, conflictKeys, updateColumns), timeout, cancellationToken)
                    .ConfigureAwait(false);
                return written;
            }
            finally
            {
                await ExecAsync(connection, $"DROP TEMPORARY TABLE IF EXISTS {Quote(staging)}", timeout, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        finally
        {
            if (wasClosed)
                await connection.CloseAsync().ConfigureAwait(false);
        }
    }

    private static BulkColumnSet<T> Validate<T>(
        MySqlConnection connection, string table, IEnumerable<T> rows, IReadOnlyList<string> conflictKeys, IReadOnlyList<string>? columns)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(conflictKeys);
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentException("Table name must not be empty.", nameof(table));
        if (conflictKeys.Count == 0)
            throw new ArgumentException("At least one conflict-key column is required.", nameof(conflictKeys));

        return BulkColumnSet<T>.Get(columns);
    }

    // Temp tables are per-session in MySQL; a unique name avoids colliding with a crashed prior call.
    // Guid "N" format is 32 hex chars (no hyphens), a valid bare identifier.
    private static string StagingName() => $"garoa_upsert_{Guid.NewGuid():N}";

    // Staging holds exactly the written columns with their target types but no constraints
    // (CREATE TABLE ... AS SELECT copies neither keys nor auto_increment), so a subset upsert never
    // trips a copied constraint on a column we don't write.
    private static string CreateStaging(string staging, string table, IReadOnlyList<string> columns) =>
        $"CREATE TEMPORARY TABLE {Quote(staging)} AS SELECT {ColumnList(columns)} FROM {table} WHERE 1 = 0";

    private static MySqlBulkCopy CreateBulkCopy<T>(MySqlConnection connection, string staging, BulkColumnSet<T> set, int? timeout)
    {
        var bulkCopy = new MySqlBulkCopy(connection) { DestinationTableName = staging };
        if (timeout.HasValue)
            bulkCopy.BulkCopyTimeout = timeout.Value;

        // Map source ordinal -> staging column by name, so order is explicit.
        for (int i = 0; i < set.Count; i++)
            bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(i, set.ColumnNames[i]));

        return bulkCopy;
    }

    private static string UpsertCommand(
        string table, string staging, IReadOnlyList<string> writtenColumns, IReadOnlyList<string> conflictKeys, IReadOnlyList<string>? updateColumns)
    {
        string cols = ColumnList(writtenColumns);

        // Default: overwrite every written column that isn't part of the conflict key.
        string[] updates = (updateColumns
            ?? writtenColumns.Where(c => !conflictKeys.Contains(c, StringComparer.Ordinal)).ToArray()).ToArray();

        // No update columns -> MySQL has no DO NOTHING; INSERT IGNORE skips the conflicting rows.
        if (updates.Length == 0)
            return $"INSERT IGNORE INTO {table} ({cols}) SELECT {cols} FROM {Quote(staging)}";

        string set = string.Join(", ", updates.Select(c => $"{Quote(c)} = VALUES({Quote(c)})"));
        return $"INSERT INTO {table} ({cols}) SELECT {cols} FROM {Quote(staging)} ON DUPLICATE KEY UPDATE {set}";
    }

    private static string ColumnList(IReadOnlyList<string> columns) => string.Join(", ", columns.Select(Quote));

    private static void Exec(MySqlConnection connection, string sql, int? timeout)
    {
        using MySqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        if (timeout.HasValue)
            command.CommandTimeout = timeout.Value;
        command.ExecuteNonQuery();
    }

    private static async Task ExecAsync(MySqlConnection connection, string sql, int? timeout, CancellationToken cancellationToken)
    {
        await using MySqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        if (timeout.HasValue)
            command.CommandTimeout = timeout.Value;
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static string Quote(string identifier) => $"`{identifier.Replace("`", "``")}`";
}
