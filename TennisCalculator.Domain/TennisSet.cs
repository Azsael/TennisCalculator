namespace TennisCalculator.Domain;

public record TennisSet
{
    public required IReadOnlyList<TennisPlayer> Players { get; init; }
    public required IReadOnlyList<TennisGame> Games { get; init; }
    public TennisGame? CurrentGame { get; init; }
    public TennisPlayer? Winner { get; init; }
    
    public bool HasWinner => Winner is not null;
}