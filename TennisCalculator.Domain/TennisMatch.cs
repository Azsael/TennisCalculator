namespace TennisCalculator.Domain;

public record TennisMatch
{
    public required string MatchId { get; init; }
    public required IReadOnlyList<TennisPlayer> Players { get; init; }
    public required IReadOnlyList<TennisSet> Sets { get; init; }
    public TennisSet? CurrentSet { get; init; }
    public TennisPlayer? Winner { get; init; }
    
    public bool HasWinner => Winner is not null;
}