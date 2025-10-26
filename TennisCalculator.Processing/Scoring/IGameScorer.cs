using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

public interface IGameScorer
{
    TennisGame AddPoint(TennisGame game, TennisPlayer pointWinner);
    TennisPlayer? DetermineGameWinner(TennisGame game);
    IReadOnlyDictionary<TennisPlayer, TennisPoint> CalculateCurrentScore(IReadOnlyList<TennisPlayer> pointHistory, IReadOnlyList<TennisPlayer> players);
}