using System.Data;
using System.Data.Common;
using Garoa.Mapping;

namespace Garoa;

/// <summary>
/// Garoa's data-access surface: <c>Query</c>/<c>Execute</c> extension methods over
/// <see cref="DbConnection"/>. Connections that are closed on entry are opened and then
/// closed again on exit, so callers never leak a connection they didn't open.
/// </summary>
public static class GaroaConnectionExtensions
{
    /// <summary>Executes <paramref name="sql"/> and materialises every row as <typeparamref name="T"/>.</summary>
    public static List<T> Query<T>(
        this DbConnection connection,
        string sql,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        ArgumentNullException.ThrowIfNull(connection);

        bool wasClosed = connection.State == ConnectionState.Closed;
        using DbCommand command = CreateCommand(connection, sql, param, commandTimeout, commandType);
        try
        {
            if (wasClosed)
                connection.Open();

            using DbDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult);
            var results = new List<T>();
            if (reader.Read())
            {
                Func<DbDataReader, T> map = Mapper<T>.ForReader(reader);
                do
                {
                    results.Add(map(reader));
                }
                while (reader.Read());
            }

            return results;
        }
        finally
        {
            if (wasClosed)
                connection.Close();
        }
    }

    /// <summary>
    /// Returns the first row mapped to <typeparamref name="T"/>, throwing when the query yields no
    /// rows. Hints the provider to fetch a single row (<see cref="CommandBehavior.SingleRow"/>) and
    /// never materialises a list. Use this when the row is expected to exist (pair it with a
    /// <c>LIMIT</c>/<c>TOP 1</c> in your SQL — Garoa never injects one).
    /// </summary>
    public static T QueryFirst<T>(
        this DbConnection connection,
        string sql,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
        => QueryFirstRow<T>(connection, sql, param, commandTimeout, commandType, throwIfEmpty: true)!;

    /// <summary>
    /// Returns the first row mapped to <typeparamref name="T"/>, or <see langword="default"/>
    /// (<see langword="null"/> for a reference type) when the query yields no rows. The idiomatic
    /// "fetch by id → entity or null".
    /// </summary>
    public static T? QueryFirstOrDefault<T>(
        this DbConnection connection,
        string sql,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
        => QueryFirstRow<T>(connection, sql, param, commandTimeout, commandType, throwIfEmpty: false);

    /// <summary>Asynchronously returns the first row, throwing when the query yields no rows.</summary>
    public static async Task<T> QueryFirstAsync<T>(
        this DbConnection connection,
        string sql,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
        => (await QueryFirstRowAsync<T>(connection, sql, param, commandTimeout, commandType, throwIfEmpty: true, cancellationToken)
            .ConfigureAwait(false))!;

    /// <summary>Asynchronously returns the first row, or <see langword="default"/> when there are none.</summary>
    public static Task<T?> QueryFirstOrDefaultAsync<T>(
        this DbConnection connection,
        string sql,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
        => QueryFirstRowAsync<T>(connection, sql, param, commandTimeout, commandType, throwIfEmpty: false, cancellationToken);

    private static T? QueryFirstRow<T>(
        DbConnection connection,
        string sql,
        object? param,
        int? commandTimeout,
        CommandType? commandType,
        bool throwIfEmpty)
    {
        ArgumentNullException.ThrowIfNull(connection);

        bool wasClosed = connection.State == ConnectionState.Closed;
        using DbCommand command = CreateCommand(connection, sql, param, commandTimeout, commandType);
        try
        {
            if (wasClosed)
                connection.Open();

            using DbDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult);
            if (!reader.Read())
                return throwIfEmpty ? throw NoRows() : default;

            return Mapper<T>.ForReader(reader)(reader);
        }
        finally
        {
            if (wasClosed)
                connection.Close();
        }
    }

    private static async Task<T?> QueryFirstRowAsync<T>(
        DbConnection connection,
        string sql,
        object? param,
        int? commandTimeout,
        CommandType? commandType,
        bool throwIfEmpty,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(connection);

        bool wasClosed = connection.State == ConnectionState.Closed;
        DbCommand command = CreateCommand(connection, sql, param, commandTimeout, commandType);
        await using (command.ConfigureAwait(false))
        {
            try
            {
                if (wasClosed)
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                DbDataReader reader = await command
                    .ExecuteReaderAsync(CommandBehavior.SingleRow | CommandBehavior.SingleResult, cancellationToken)
                    .ConfigureAwait(false);
                await using (reader.ConfigureAwait(false))
                {
                    if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        return throwIfEmpty ? throw NoRows() : default;

                    return Mapper<T>.ForReader(reader)(reader);
                }
            }
            finally
            {
                if (wasClosed)
                    await connection.CloseAsync().ConfigureAwait(false);
            }
        }
    }

    private static InvalidOperationException NoRows() =>
        new("QueryFirst expected at least one row, but the query returned none.");

    /// <summary>Executes a non-query statement (INSERT/UPDATE/DELETE) and returns rows affected.</summary>
    public static int Execute(
        this DbConnection connection,
        string sql,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        ArgumentNullException.ThrowIfNull(connection);

        bool wasClosed = connection.State == ConnectionState.Closed;
        using DbCommand command = CreateCommand(connection, sql, param, commandTimeout, commandType);
        try
        {
            if (wasClosed)
                connection.Open();

            return command.ExecuteNonQuery();
        }
        finally
        {
            if (wasClosed)
                connection.Close();
        }
    }

    /// <summary>Asynchronously executes <paramref name="sql"/> and materialises every row as <typeparamref name="T"/>.</summary>
    public static async Task<List<T>> QueryAsync<T>(
        this DbConnection connection,
        string sql,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);

        bool wasClosed = connection.State == ConnectionState.Closed;
        DbCommand command = CreateCommand(connection, sql, param, commandTimeout, commandType);
        await using (command.ConfigureAwait(false))
        {
            try
            {
                if (wasClosed)
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken)
                    .ConfigureAwait(false);
                await using (reader.ConfigureAwait(false))
                {
                    var results = new List<T>();
                    if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        // Mapping reads from the already-buffered row synchronously, matching the
                        // common micro-ORM pattern (async is for I/O, not per-field marshalling).
                        Func<DbDataReader, T> map = Mapper<T>.ForReader(reader);
                        do
                        {
                            results.Add(map(reader));
                        }
                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false));
                    }

                    return results;
                }
            }
            finally
            {
                if (wasClosed)
                    await connection.CloseAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>Asynchronously executes a non-query statement and returns rows affected.</summary>
    public static async Task<int> ExecuteAsync(
        this DbConnection connection,
        string sql,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);

        bool wasClosed = connection.State == ConnectionState.Closed;
        DbCommand command = CreateCommand(connection, sql, param, commandTimeout, commandType);
        await using (command.ConfigureAwait(false))
        {
            try
            {
                if (wasClosed)
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (wasClosed)
                    await connection.CloseAsync().ConfigureAwait(false);
            }
        }
    }

    private static DbCommand CreateCommand(
        DbConnection connection,
        string sql,
        object? param,
        int? commandTimeout,
        CommandType? commandType)
    {
        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("SQL text must not be empty.", nameof(sql));

        DbCommand command = connection.CreateCommand();
        command.CommandText = sql;

        // Per-call timeout wins; otherwise fall back to the global default (null = provider default).
        int? effectiveTimeout = GaroaDefaults.ResolveCommandTimeout(commandTimeout);
        if (effectiveTimeout.HasValue)
            command.CommandTimeout = effectiveTimeout.Value;
        if (commandType.HasValue)
            command.CommandType = commandType.Value;

        ParameterBinder.Bind(command, param);
        return command;
    }
}
