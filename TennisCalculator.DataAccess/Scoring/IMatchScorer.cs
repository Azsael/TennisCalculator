using TennisCalculator.Domain;

namespace TennisCalculator.DataAccess.Scoring;

public interface IMatchScorer
{
    TennisMatch AddSet(TennisMatch match, TennisSet completedSet);
    TennisPlayer? DetermineMatchWinner(TennisMatch match);
    TennisMatch StartNewSet(TennisMatch match, IReadOnlyList<TennisPlayer> players);
}