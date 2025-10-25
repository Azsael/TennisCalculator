namespace TennisCalculator.Domain;

public record TennisGame
{
    public required IReadOnlyList<TennisPlayer> Players { get; init; }
    public required IReadOnlyList<TennisPlayer> PointHistory { get; init; }
    public required IReadOnlyDictionary<TennisPlayer, TennisPoint> CurrentScore { get; init; }
    public TennisPlayer? Winner { get; init; }
    
    public bool HasWinner => Winner is not null;
}