using System.Diagnostics.CodeAnalysis;

namespace TennisCalculator.Domain;

/// <summary>
/// Exception thrown when file processing errors occur
/// </summary>
public class FileProcessingException : TennisCalculatorException
{
    /// <summary>
    /// Gets the file path that caused the error
    /// </summary>
    public required string FilePath { get; init; }

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
    [SetsRequiredMembers]
    public FileProcessingException(string message, string filePath, int? lineNumber = null) : base(message)
    {
        FilePath = filePath;
        LineNumber = lineNumber;
    }

    /// <summary>
    /// Initializes a new instance of FileProcessingException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="filePath">The file path that caused the error</param>
    /// <param name="innerException">The inner exception</param>
    /// <param name="lineNumber">The line number where the error occurred</param>
    [SetsRequiredMembers]
    public FileProcessingException(string message, string filePath, Exception innerException, int? lineNumber = null) 
        : base(message, innerException)
    {
        FilePath = filePath;
        LineNumber = lineNumber;
    }
}