namespace TennisCalculator.Domain.DataAccess;

/// <summary>
/// Repository interface for managing tennis match data
/// </summary>
public interface ITennisMatchRepository
{
    /// <summary>
    /// Adds a match to the repository
    /// </summary>
    /// <param name="match">The match to add</param>
    void AddMatch(TennisMatch match);
    
    /// <summary>
    /// Retrieves a match by its ID
    /// </summary>
    /// <param name="matchId">The match ID to search for</param>
    /// <returns>The match if found, null otherwise</returns>
    TennisMatch? GetMatch(string matchId);
    
    /// <summary>
    /// Retrieves all matches in the repository
    /// </summary>
    /// <returns>Collection of all matches</returns>
    IEnumerable<TennisMatch> GetAllMatches();
    
    /// <summary>
    /// Retrieves all matches where the specified player participated
    /// </summary>
    /// <param name="playerName">The player name to search for</param>
    /// <returns>Collection of matches involving the player</returns>
    IEnumerable<TennisMatch> GetMatchesForPlayer(string playerName);
}