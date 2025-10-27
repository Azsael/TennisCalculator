using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

/// <summary>
/// Provides tennis set scoring functionality using simplified rules (first to 6 games wins)
/// </summary>
public interface ISetScorer
{
    /// <summary>
    /// Adds a completed game to the set and updates the set state
    /// </summary>
    /// <param name="set">The current set state</param>
    /// <param name="completedGame">The completed game to add</param>
    /// <returns>Updated set state with the new game added</returns>
    TennisSet AddGame(TennisSet set, TennisGame completedGame);
    
    /// <summary>
    /// Determines if the set has a winner based on games won
    /// </summary>
    /// <param name="set">The set to evaluate</param>
    /// <returns>The winning player if the set is complete, null otherwise</returns>
    TennisPlayer? DetermineSetWinner(TennisSet set);
    
    /// <summary>
    /// Starts a new game within the set
    /// </summary>
    /// <param name="set">The current set state</param>
    /// <param name="players">All players in the set</param>
    /// <returns>Updated set state with a new game started</returns>
    TennisSet StartNewGame(TennisSet set, IReadOnlyList<TennisPlayer> players);
}