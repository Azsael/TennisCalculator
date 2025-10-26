using System.Diagnostics.CodeAnalysis;

namespace TennisCalculator.Domain;

/// <summary>
/// Exception thrown when no data loader is available for a given data source
/// </summary>
public class UnsupportedDataSourceException : Exception
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
}