using System.Linq;

namespace TennisCalculator.Domain;

/// <summary>
/// In-memory implementation of match repository using dictionary-based storage
/// </summary>
internal class TennisMatchMemoryBank : ITennisMatchRepository
{
    private readonly Dictionary<string, TennisMatch> _matches = new();
    
    /// <summary>
    /// Adds a match to the repository
    /// </summary>
    /// <param name="match">The match to add</param>
    public void AddMatch(TennisMatch match)
    {
        _matches[match.MatchId] = match;
    }
    
    /// <summary>
    /// Retrieves a match by its ID
    /// </summary>
    /// <param name="matchId">The match ID to search for</param>
    /// <returns>The match if found, null otherwise</returns>
    public Task<TennisMatch?> GetMatch(string matchId)
    {
        return Task.FromResult(_matches.TryGetValue(matchId, out var match) ? match : null);
    }

    /// <summary>
    /// Retrieves all matches where the specified player participated
    /// </summary>
    /// <param name="playerName">The player name to search for</param>
    /// <returns>Collection of matches involving the player</returns>
    public Task<IList<TennisMatch>> GetMatchesForPlayer(string playerName)
    {
        var matches = _matches.Values.Where(m => m.Players.Any(p => p == playerName)).ToList();

        return Task.FromResult<IList<TennisMatch>>(matches);
    }
}