using System.Text.RegularExpressions;

namespace TennisCalculator.Console.Commands;

/// <summary>
/// Parses user input into command objects with support for extensible command patterns
/// </summary>
public class CommandParser : ICommandParser
{
    private static readonly Regex ScoreMatchPattern = new(@"^Score\s+Match\s+(.+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex GamesPlayerPattern = new(@"^Games\s+Player\s+(.+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex QuitPattern = new(@"^quit$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Parses a user input string into a command object
    /// </summary>
    /// <param name="input">The user input string</param>
    /// <returns>A command object representing the parsed input</returns>
    /// <exception cref="InvalidCommandException">Thrown when the input cannot be parsed</exception>
    public ICommand ParseCommand(string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);

        var trimmedInput = input.Trim();

        // Check for quit command
        if (QuitPattern.IsMatch(trimmedInput))
        {
            return new QuitCommand();
        }

        // Check for Score Match command
        var scoreMatch = ScoreMatchPattern.Match(trimmedInput);
        if (scoreMatch.Success)
        {
            var matchId = scoreMatch.Groups[1].Value.Trim();
            if (string.IsNullOrWhiteSpace(matchId))
            {
                throw new InvalidCommandException("Error: Match ID cannot be empty");
            }
            return new ScoreMatchCommand(new ScoreMatchQuery { MatchId = matchId });
        }

        // Check for Games Player command
        var gamesPlayer = GamesPlayerPattern.Match(trimmedInput);
        if (gamesPlayer.Success)
        {
            var playerName = gamesPlayer.Groups[1].Value.Trim();
            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new InvalidCommandException("Error: Player name cannot be empty");
            }
            return new GamesPlayerCommand(new GamesPlayerQuery { PlayerName = playerName });
        }

        // No pattern matched
        throw new InvalidCommandException("Error: Unrecognised command");
    }
}

/// <summary>
/// Base interface for all command types
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Command representing a quit request
/// </summary>
public record QuitCommand : ICommand;

/// <summary>
/// Command for retrieving match score information
/// </summary>
public record ScoreMatchCommand(ScoreMatchQuery Query) : ICommand;

/// <summary>
/// Command for retrieving player game statistics
/// </summary>
public record GamesPlayerCommand(GamesPlayerQuery Query) : ICommand;

/// <summary>
/// Exception thrown when a command cannot be parsed
/// </summary>
public class InvalidCommandException : Exception
{
    public InvalidCommandException(string message) : base(message)
    {
    }
}