namespace TennisCalculator.Domain;

public record TennisPlayerStatistics
{
    public required string PlayerName { get; init; }
    public required int GamesWon { get; init; }
    public required int GamesLost { get; init; }
    public required int GamesInProgress { get; init; }
}