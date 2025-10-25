using TennisCalculator.Domain;

namespace TennisCalculator.DataAccess;

/// <summary>
/// In-memory implementation of match repository using dictionary-based storage
/// </summary>
public class InMemoryMatchRepository : IMatchRepository
{
    private readonly Dictionary<string, TennisMatch> _matches = new();
    
    /// <summary>
    /// Adds a match to the repository
    /// </summary>
    /// <param name="match">The match to add</param>
    public void AddMatch(TennisMatch match)
    {
        ArgumentNullException.ThrowIfNull(match);
        _matches[match.MatchId] = match;
    }
    
    /// <summary>
    /// Retrieves a match by its ID
    /// </summary>
    /// <param name="matchId">The match ID to search for</param>
    /// <returns>The match if found, null otherwise</returns>
    public TennisMatch? GetMatch(string matchId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(matchId);
        return _matches.TryGetValue(matchId, out var match) ? match : null;
    }
    
    /// <summary>
    /// Retrieves all matches in the repository
    /// </summary>
    /// <returns>Collection of all matches</returns>
    public IEnumerable<TennisMatch> GetAllMatches()
    {
        return _matches.Values;
    }
    
    /// <summary>
    /// Retrieves all matches where the specified player participated
    /// </summary>
    /// <param name="playerName">The player name to search for</param>
    /// <returns>Collection of matches involving the player</returns>
    public IEnumerable<TennisMatch> GetMatchesForPlayer(string playerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(playerName);
        return _matches.Values.Where(m => 
            m.Players.Any(p => p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase)));
    }
}