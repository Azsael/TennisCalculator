using System.Diagnostics.CodeAnalysis;

namespace TennisCalculator.Domain;

/// <summary>
/// Exception thrown when no data loader is available for a given data source
/// </summary>
public class UnsupportedDataSourceException : TennisCalculatorException
{
    /// <summary>
    /// Gets the data source that is not supported
    /// </summary>
    public required string DataSource { get; init; }

    /// <summary>
    /// Initializes a new instance of UnsupportedDataSourceException
    /// </summary>
    /// <param name="source">The data source that is not supported</param>
    [SetsRequiredMembers]
    public UnsupportedDataSourceException(string source) 
        : base($"No data loader available for source: {source}")
    {
        DataSource = source;
    }

    /// <summary>
    /// Initializes a new instance of UnsupportedDataSourceException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="source">The data source that is not supported</param>
    [SetsRequiredMembers]
    public UnsupportedDataSourceException(string message, string source) : base(message)
    {
        DataSource = source;
    }

    /// <summary>
    /// Initializes a new instance of UnsupportedDataSourceException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="source">The data source that is not supported</param>
    /// <param name="innerException">The inner exception</param>
    [SetsRequiredMembers]
    public UnsupportedDataSourceException(string message, string source, Exception innerException) 
        : base(message, innerException)
    {
        DataSource = source;
    }
}