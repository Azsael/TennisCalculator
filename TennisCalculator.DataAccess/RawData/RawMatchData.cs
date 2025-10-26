namespace TennisCalculator.DataAccess.Data;

/// <summary>
/// Raw match data parsed from tournament data source before domain processing
/// </summary>
public record RawMatchData
{
    /// <summary>
    /// Unique identifier for the match
    /// </summary>
    public required string MatchId { get; init; }
    
    /// <summary>
    /// Name of the first player
    /// </summary>
    public required string Player1Name { get; init; }
    
    /// <summary>
    /// Name of the second player
    /// </summary>
    public required string Player2Name { get; init; }
    
    /// <summary>
    /// Sequence of points where 0 = Player1 wins point, 1 = Player2 wins point
    /// </summary>
    public required IReadOnlyList<int> Points { get; init; }
}