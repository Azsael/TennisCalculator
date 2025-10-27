using System.Diagnostics.CodeAnalysis;

namespace TennisCalculator.Processing.RawData;

/// <summary>
/// Exception thrown when file processing errors occur
/// </summary>
public class TennisDataSourceException : Exception
{
    /// <summary>
    /// Gets the line number where the error occurred (if applicable)
    /// </summary>
    public int? LineNumber { get; init; }

    /// <summary>
    /// Initializes a new instance of FileProcessingException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="filePath">The file path that caused the error</param>
    /// <param name="lineNumber">The line number where the error occurred</param>
    public TennisDataSourceException(string message, int? lineNumber = null) : base(message)
    {
        LineNumber = lineNumber;
    }

    /// <summary>
    /// Initializes a new instance of FileProcessingException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="filePath">The file path that caused the error</param>
    /// <param name="innerException">The inner exception</param>
    /// <param name="lineNumber">The line number where the error occurred</param>
    public TennisDataSourceException(string message, Exception innerException, int? lineNumber = null) 
        : base(message, innerException)
    {
        LineNumber = lineNumber;
    }
}