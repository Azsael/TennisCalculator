using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

/// <summary>
/// Provides tennis match scoring functionality using best-of-3 sets format
/// </summary>
public interface IMatchScorer
{
    /// <summary>
    /// Adds a completed set to the match and updates the match state
    /// </summary>
    /// <param name="match">The current match state</param>
    /// <param name="completedSet">The completed set to add</param>
    /// <returns>Updated match state with the new set added</returns>
    TennisMatch AddSet(TennisMatch match, TennisSet completedSet);
    
    /// <summary>
    /// Determines if the match has a winner based on sets won
    /// </summary>
    /// <param name="match">The match to evaluate</param>
    /// <returns>The winning player if the match is complete, null otherwise</returns>
    TennisPlayer? DetermineMatchWinner(TennisMatch match);
    
    /// <summary>
    /// Starts a new set within the match
    /// </summary>
    /// <param name="match">The current match state</param>
    /// <param name="players">All players in the match</param>
    /// <returns>Updated match state with a new set started</returns>
    TennisMatch StartNewSet(TennisMatch match, IReadOnlyList<TennisPlayer> players);
}