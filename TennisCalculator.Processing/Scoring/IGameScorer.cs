using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

/// <summary>
/// Provides tennis game scoring functionality using standard tennis rules
/// </summary>
public interface IGameScorer
{
    /// <summary>
    /// Adds a point to the game for the specified player and updates the game state
    /// </summary>
    /// <param name="game">The current game state</param>
    /// <param name="pointWinner">The player who won the point</param>
    /// <returns>Updated game state with the new point added</returns>
    TennisGame AddPoint(TennisGame game, TennisPlayer pointWinner);
    
    /// <summary>
    /// Determines if the game has a winner based on current scoring
    /// </summary>
    /// <param name="game">The game to evaluate</param>
    /// <returns>The winning player if the game is complete, null otherwise</returns>
    TennisPlayer? DetermineGameWinner(TennisGame game);
    
    /// <summary>
    /// Calculates the current tennis point scores for all players based on point history
    /// </summary>
    /// <param name="pointHistory">Ordered list of players who won each point</param>
    /// <param name="players">All players in the game</param>
    /// <returns>Dictionary mapping each player to their current tennis point score</returns>
    IReadOnlyDictionary<TennisPlayer, TennisPoint> CalculateCurrentScore(IReadOnlyList<TennisPlayer> pointHistory, IReadOnlyList<TennisPlayer> players);
}