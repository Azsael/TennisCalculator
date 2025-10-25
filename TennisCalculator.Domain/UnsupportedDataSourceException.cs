namespace TennisCalculator.Domain;

/// <summary>
/// Exception thrown when an unsupported data source is encountered
/// </summary>
public class UnsupportedDataSourceException : TennisCalculatorException
{
    /// <summary>
    /// Gets the data source that is not supported
    /// </summary>
    public required string Source { get; init; }

    /// <summary>
    /// Initializes a new instance of UnsupportedDataSourceException
    /// </summary>
    /// <param name="source">The unsupported data source</param>
    public UnsupportedDataSourceException(string source) 
        : base($"No data loader available for source: {source}")
    {
        Source = source;
    }

    /// <summary>
    /// Initializes a new instance of UnsupportedDataSourceException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="source">The unsupported data source</param>
    public UnsupportedDataSourceException(string message, string source) : base(message)
    {
        Source = source;
    }

    /// <summary>
    /// Initializes a new instance of UnsupportedDataSourceException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="source">The unsupported data source</param>
    /// <param name="innerException">The inner exception</param>
    public UnsupportedDataSourceException(string message, string source, Exception innerException) 
        : base(message, innerException)
    {
        Source = source;
    }
}