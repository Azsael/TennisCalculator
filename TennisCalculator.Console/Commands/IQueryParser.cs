namespace TennisCalculator.Console.Commands;

/// <summary>
/// Interface for parsing user input commands into query objects
/// </summary>
public interface IQueryParser
{
    /// <summary>
    /// Parses a user input string into a query object
    /// </summary>
    /// <param name="input">The user input string</param>
    /// <returns>A query object representing the parsed command</returns>
    /// <exception cref="InvalidQueryException">Thrown when the input cannot be parsed</exception>
    IQuery ParseQuery(string input);
}