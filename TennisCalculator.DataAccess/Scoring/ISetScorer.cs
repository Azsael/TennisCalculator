using TennisCalculator.Domain;

namespace TennisCalculator.DataAccess.Scoring;

public interface ISetScorer
{
    TennisSet AddGame(TennisSet set, TennisGame completedGame);
    TennisPlayer? DetermineSetWinner(TennisSet set);
    TennisSet StartNewGame(TennisSet set, IReadOnlyList<TennisPlayer> players);
}