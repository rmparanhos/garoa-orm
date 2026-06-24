using System.Data;
using Npgsql;

namespace Garoa;

/// <summary>
/// Streaming bulk insert for PostgreSQL, built on Npgsql's binary COPY protocol
/// (<see cref="NpgsqlBinaryImporter"/>). Rows are written one at a time straight to the server,
/// so a giant <c>INSERT</c> string is never built and the source sequence is never materialised.
/// Each row is written through a compiled, typed writer (<see cref="NpgsqlCopyWriter{T}"/>), so
/// value-type columns never get boxed. Requires only normal <c>INSERT</c> privilege on the target table.
/// </summary>
public static class NpgsqlBulkInsertExtensions
{
    /// <summary>Bulk-inserts <paramref name="rows"/> into <paramref name="table"/> and returns the row count.</summary>
    /// <param name="connection">An open or closed connection; if closed it is opened and closed again.</param>
    /// <param name="table">Target table, used verbatim (qualify/quote it yourself if needed, e.g. <c>public."People"</c>).</param>
    /// <param name="rows">The rows to insert; streamed one at a time, never fully materialised.</param>
    /// <param name="columns">Columns to write; when null, all readable members of <typeparamref name="T"/> are used.</param>
    /// <param name="commandTimeout">Timeout in seconds for the COPY operation; when null, falls back to <see cref="GaroaDefaults.CommandTimeoutSeconds"/>.</param>
    public static ulong BulkInsert<T>(
        this NpgsqlConnection connection,
        string table,
        IEnumerable<T> rows,
        IReadOnlyList<string>? columns = null,
        int? commandTimeout = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(rows);

        NpgsqlCopyWriter<T> rowWriter = NpgsqlCopyWriter<T>.Get(columns);
        bool wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed)
            connection.Open();

        try
        {
            using NpgsqlBinaryImporter writer = connection.BeginBinaryImport(BuildCopyCommand(table, rowWriter.ColumnNames));
            ApplyTimeout(writer, commandTimeout);
            ulong written = 0;

            foreach (T row in rows)
            {
                writer.StartRow();
                rowWriter.WriteRow(writer, row);
                written++;
            }

            writer.Complete();
            return written;
        }
        finally
        {
            if (wasClosed)
                connection.Close();
        }
    }

    /// <summary>Asynchronously bulk-inserts <paramref name="rows"/> into <paramref name="table"/>.</summary>
    public static async Task<ulong> BulkInsertAsync<T>(
        this NpgsqlConnection connection,
        string table,
        IEnumerable<T> rows,
        IReadOnlyList<string>? columns = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(rows);

        NpgsqlCopyWriter<T> rowWriter = NpgsqlCopyWriter<T>.Get(columns);
        bool wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed)
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            NpgsqlBinaryImporter writer = await connection
                .BeginBinaryImportAsync(BuildCopyCommand(table, rowWriter.ColumnNames), cancellationToken)
                .ConfigureAwait(false);

            await using (writer.ConfigureAwait(false))
            {
                ApplyTimeout(writer, commandTimeout);
                ulong written = 0;

                foreach (T row in rows)
                {
                    // Row framing is awaited (the frequent I/O points); the typed per-value writes
                    // buffer in memory and only flush synchronously at buffer boundaries.
                    await writer.StartRowAsync(cancellationToken).ConfigureAwait(false);
                    rowWriter.WriteRow(writer, row);
                    written++;
                }

                await writer.CompleteAsync(cancellationToken).ConfigureAwait(false);
                return written;
            }
        }
        finally
        {
            if (wasClosed)
                await connection.CloseAsync().ConfigureAwait(false);
        }
    }

    private static string BuildCopyCommand(string table, IReadOnlyList<string> columnNames)
    {
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentException("Table name must not be empty.", nameof(table));

        string columnList = string.Join(", ", columnNames.Select(Quote));
        return $"COPY {table} ({columnList}) FROM STDIN (FORMAT BINARY)";
    }

    // Applies the per-call timeout, or the global default, to the COPY writer. Npgsql's importer
    // exposes a set-only TimeSpan Timeout; a value of 0 seconds means "no timeout" (TimeSpan.Zero).
    private static void ApplyTimeout(NpgsqlBinaryImporter writer, int? commandTimeout)
    {
        int? effectiveTimeout = GaroaDefaults.ResolveCommandTimeout(commandTimeout);
        if (effectiveTimeout.HasValue)
            writer.Timeout = TimeSpan.FromSeconds(effectiveTimeout.Value);
    }

    private static string Quote(string identifier) => $"\"{identifier.Replace("\"", "\"\"")}\"";
}
