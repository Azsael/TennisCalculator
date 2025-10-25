using TennisCalculator.Domain;

namespace TennisCalculator.GamePlay;

public interface IMatchScorer
{
    TennisMatch AddSet(TennisMatch match, TennisSet completedSet);
    TennisPlayer? DetermineMatchWinner(TennisMatch match);
    TennisMatch StartNewSet(TennisMatch match, IReadOnlyList<TennisPlayer> players);
}