using System.Text.RegularExpressions;
using TennisCalculator.GamePlay;

namespace TennisCalculator;

/// <summary>
/// Parses user input commands into query objects
/// </summary>
public class QueryParser
{
    private static readonly Regex ScoreMatchPattern = new(@"^Score\s+Match\s+(.+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex GamesPlayerPattern = new(@"^Games\s+Player\s+(.+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex QuitPattern = new(@"^quit$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Parses a user input string into a query object
    /// </summary>
    /// <param name="input">The user input string</param>
    /// <returns>A query object representing the parsed command</returns>
    /// <exception cref="InvalidQueryException">Thrown when the input cannot be parsed</exception>
    public IQuery ParseQuery(string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);

        var trimmedInput = input.Trim();

        // Check for quit command
        if (QuitPattern.IsMatch(trimmedInput))
        {
            return new QuitQuery();
        }

        // Check for Score Match command
        var scoreMatch = ScoreMatchPattern.Match(trimmedInput);
        if (scoreMatch.Success)
        {
            var matchId = scoreMatch.Groups[1].Value.Trim();
            if (string.IsNullOrWhiteSpace(matchId))
            {
                throw new InvalidQueryException("Error: Match ID cannot be empty");
            }
            return new ScoreMatchQueryWrapper(new ScoreMatchQuery { MatchId = matchId });
        }

        // Check for Games Player command
        var gamesPlayer = GamesPlayerPattern.Match(trimmedInput);
        if (gamesPlayer.Success)
        {
            var playerName = gamesPlayer.Groups[1].Value.Trim();
            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new InvalidQueryException("Error: Player name cannot be empty");
            }
            return new GamesPlayerQueryWrapper(new GamesPlayerQuery { PlayerName = playerName });
        }

        // No pattern matched
        throw new InvalidQueryException("Error: Unrecognised command");
    }
}

/// <summary>
/// Base interface for all query types
/// </summary>
public interface IQuery
{
}

/// <summary>
/// Query representing a quit command
/// </summary>
public record QuitQuery : IQuery;

/// <summary>
/// Wrapper for ScoreMatchQuery to implement IQuery
/// </summary>
public record ScoreMatchQueryWrapper(ScoreMatchQuery Query) : IQuery;

/// <summary>
/// Wrapper for GamesPlayerQuery to implement IQuery
/// </summary>
public record GamesPlayerQueryWrapper(GamesPlayerQuery Query) : IQuery;

/// <summary>
/// Exception thrown when a query cannot be parsed
/// </summary>
public class InvalidQueryException : Exception
{
    public InvalidQueryException(string message) : base(message)
    {
    }
}