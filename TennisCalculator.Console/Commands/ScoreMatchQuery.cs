namespace TennisCalculator.Console.Commands;

/// <summary>
/// Query to retrieve the score and result of a specific match
/// </summary>
public record ScoreMatchQuery
{
    /// <summary>
    /// The ID of the match to retrieve
    /// </summary>
    public required string MatchId { get; init; }
}