namespace TennisCalculator.Domain;

/// <summary>
/// Base exception class for all Tennis Calculator specific exceptions
/// </summary>
public class TennisCalculatorException : Exception
{
    /// <summary>
    /// Initializes a new instance of TennisCalculatorException
    /// </summary>
    /// <param name="message">The error message</param>
    public TennisCalculatorException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of TennisCalculatorException
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public TennisCalculatorException(string message, Exception innerException) : base(message, innerException)
    {
    }
}