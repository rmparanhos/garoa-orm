using System.Data;
using Garoa.Bulk;
using MySqlConnector;

namespace Garoa;

/// <summary>
/// Streaming bulk insert for MySQL, built on MySqlConnector's <see cref="MySqlBulkCopy"/>.
/// Rows flow through a forward-only reader, so the source sequence is never materialised.
/// Requires only normal <c>INSERT</c> privilege; the connection string must enable local
/// infile (<c>AllowLoadLocalInfile=True</c>), which <see cref="MySqlBulkCopy"/> uses internally.
/// </summary>
public static class MySqlBulkInsertExtensions
{
    /// <summary>Bulk-inserts <paramref name="rows"/> into <paramref name="table"/> and returns the row count.</summary>
    /// <param name="connection">An open or closed connection; if closed it is opened and closed again.</param>
    /// <param name="table">Target table name.</param>
    /// <param name="rows">The rows to insert; streamed one at a time, never fully materialised.</param>
    /// <param name="columns">Columns to write; when null, all readable members of <typeparamref name="T"/> are used.</param>
    public static long BulkInsert<T>(
        this MySqlConnection connection,
        string table,
        IEnumerable<T> rows,
        IReadOnlyList<string>? columns = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(rows);

        BulkColumnSet<T> set = BulkColumnSet<T>.Get(columns);
        bool wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed)
            connection.Open();

        try
        {
            MySqlBulkCopy bulkCopy = CreateBulkCopy(connection, table, set);
            using var reader = new ObjectDataReader<T>(rows, set);
            MySqlBulkCopyResult result = bulkCopy.WriteToServer(reader);
            return result.RowsInserted;
        }
        finally
        {
            if (wasClosed)
                connection.Close();
        }
    }

    /// <summary>Asynchronously bulk-inserts <paramref name="rows"/> into <paramref name="table"/>.</summary>
    public static async Task<long> BulkInsertAsync<T>(
        this MySqlConnection connection,
        string table,
        IEnumerable<T> rows,
        IReadOnlyList<string>? columns = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(rows);

        BulkColumnSet<T> set = BulkColumnSet<T>.Get(columns);
        bool wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed)
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            MySqlBulkCopy bulkCopy = CreateBulkCopy(connection, table, set);
            using var reader = new ObjectDataReader<T>(rows, set);
            MySqlBulkCopyResult result = await bulkCopy
                .WriteToServerAsync(reader, cancellationToken)
                .ConfigureAwait(false);
            return result.RowsInserted;
        }
        finally
        {
            if (wasClosed)
                await connection.CloseAsync().ConfigureAwait(false);
        }
    }

    private static MySqlBulkCopy CreateBulkCopy<T>(MySqlConnection connection, string table, BulkColumnSet<T> set)
    {
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentException("Table name must not be empty.", nameof(table));

        var bulkCopy = new MySqlBulkCopy(connection) { DestinationTableName = table };

        // Map source ordinal -> destination column by name so column order is explicit and
        // independent of the table's physical layout.
        for (int i = 0; i < set.Count; i++)
            bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(i, set.ColumnNames[i]));

        return bulkCopy;
    }
}
