namespace Garoa;

/// <summary>
/// Process-wide defaults for Garoa operations, intended to be configured once during application
/// startup (e.g. in the composition root). Every operation also accepts a per-call
/// <c>commandTimeout</c> that overrides these defaults.
/// </summary>
public static class GaroaDefaults
{
    private static int? _commandTimeoutSeconds;

    /// <summary>
    /// Default command timeout, in seconds, applied to any <c>Query</c>/<c>Execute</c>/
    /// <c>BulkInsert</c> call that does not pass its own <c>commandTimeout</c>.
    /// <see langword="null"/> (the default) leaves the underlying provider's own default in place
    /// (typically 30&#160;seconds for ADO.NET commands). A value of <c>0</c> disables the timeout
    /// (the operation waits indefinitely). Setting it is intended for application startup; reads are
    /// lock-free.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is negative.</exception>
    public static int? CommandTimeoutSeconds
    {
        get => _commandTimeoutSeconds;
        set
        {
            if (value is < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "Command timeout must be greater than or equal to 0 (0 disables the timeout).");
            _commandTimeoutSeconds = value;
        }
    }

    /// <summary>
    /// Resolves the effective command timeout for an operation: the per-call <paramref name="perCall"/>
    /// value when supplied, otherwise the global <see cref="CommandTimeoutSeconds"/> default (which
    /// may itself be <see langword="null"/>, meaning "leave the provider default").
    /// </summary>
    internal static int? ResolveCommandTimeout(int? perCall) => perCall ?? _commandTimeoutSeconds;
}
