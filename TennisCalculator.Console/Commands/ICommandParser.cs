namespace TennisCalculator.Console.Commands;

/// <summary>
/// Interface for parsing user input into command objects
/// </summary>
public interface ICommandParser
{
    /// <summary>
    /// Parses a user input string into a command object
    /// </summary>
    /// <param name="input">The user input string</param>
    /// <returns>A command object representing the parsed input</returns>
    /// <exception cref="InvalidCommandException">Thrown when the input cannot be parsed</exception>
    ICommand ParseCommand(string input);
}