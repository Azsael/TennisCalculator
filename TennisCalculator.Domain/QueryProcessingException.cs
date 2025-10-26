using System.Diagnostics.CodeAnalysis;

namespace TennisCalculator.Domain;

/// <summary>
/// Exception thrown when query processing errors occur
/// </summary>
public class QueryProcessingException : TennisCalculatorException
{
    /// <summary>
    /// Gets the query that caused the error
    /// </summary>
    public required string Query { get; init; }

    /// <summary>
    /// Initializes a new instance of QueryProcessingException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="query">The query that caused the error</param>
    [SetsRequiredMembers]
    public QueryProcessingException(string message, string query) : base(message)
    {
        Query = query;
    }

    /// <summary>
    /// Initializes a new instance of QueryProcessingException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="query">The query that caused the error</param>
    /// <param name="innerException">The inner exception</param>
    [SetsRequiredMembers]
    public QueryProcessingException(string message, string query, Exception innerException) : base(message, innerException)
    {
        Query = query;
    }
}