namespace TennisCalculator.Console.Commands;

/// <summary>
/// Query to retrieve game statistics for a specific player
/// </summary>
public record GamesPlayerQuery
{
    /// <summary>
    /// The name of the player to retrieve statistics for
    /// </summary>
    public required string PlayerName { get; init; }
}