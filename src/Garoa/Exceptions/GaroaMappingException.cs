namespace Garoa;

/// <summary>
/// Thrown when a result-set row cannot be mapped to the target type.
/// The message always identifies the offending column by name and ordinal — unlike some
/// micro-ORMs that report the previously-read column when a conversion fails.
/// </summary>
public sealed class GaroaMappingException : Exception
{
    /// <summary>Creates the exception with a descriptive message.</summary>
    public GaroaMappingException(string message) : base(message) { }

    /// <summary>Creates the exception wrapping the underlying conversion failure.</summary>
    public GaroaMappingException(string message, Exception innerException)
        : base(message, innerException) { }
}
