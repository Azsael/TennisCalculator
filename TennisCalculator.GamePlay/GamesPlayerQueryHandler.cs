using TennisCalculator.Domain;
using TennisCalculator.Domain.DataAccess;

namespace TennisCalculator.GamePlay;

/// <summary>
/// Handles queries for player game statistics
/// </summary>
public class GamesPlayerQueryHandler(ITennisMatchRepository matchRepository) : IGamesPlayerQueryHandler
{
    /// <summary>
    /// Handles a games player query and returns formatted player statistics
    /// </summary>
    /// <param name="query">The games player query</param>
    /// <returns>Formatted player statistics as "GamesWon GamesLost"</returns>
    /// <exception cref="QueryProcessingException">Thrown when query processing fails</exception>
    public string Handle(GamesPlayerQuery query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query.PlayerName))
            {
                return "Player name cannot be null or empty";
            }

            var playerMatches = matchRepository.GetMatchesForPlayer(query.PlayerName);
            
            if (!playerMatches.Any())
            {
                return $"Error: Player '{query.PlayerName}' not found";
            }

            var statistics = CalculatePlayerStatistics(query.PlayerName, playerMatches);
            
            return $"{statistics.GamesWon} {statistics.GamesLost}";
        }
        catch (Exception ex)
        {
            return $"Error processing games player query: {ex.Message}";
        }
    }

    /// <summary>
    /// Calculates game statistics for a player across all their matches
    /// </summary>
    /// <param name="playerName">The player name</param>
    /// <param name="matches">The matches the player participated in</param>
    /// <returns>Player statistics</returns>
    private TennisPlayerStatistics CalculatePlayerStatistics(string playerName, IEnumerable<TennisMatch> matches)
    {
        var gamesWon = 0;
        var gamesLost = 0;
        var gamesInProgress = 0;

        foreach (var match in matches)
        {
            var player = match.Players.First(p => p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            
            foreach (var set in match.Sets)
            {
                foreach (var game in set.Games)
                {
                    if (game.HasWinner)
                    {
                        if (game.Winner!.Equals(player))
                        {
                            gamesWon++;
                        }
                        else
                        {
                            gamesLost++;
                        }
                    }
                    else
                    {
                        gamesInProgress++;
                    }
                }
                
                // Count current game in set if it exists and is in progress
                if (set.CurrentGame != null && !set.CurrentGame.HasWinner)
                {
                    gamesInProgress++;
                }
            }
            
            // Count current game in current set if match is in progress
            if (match.CurrentSet?.CurrentGame != null && !match.CurrentSet.CurrentGame.HasWinner)
            {
                gamesInProgress++;
            }
        }

        return new TennisPlayerStatistics
        {
            PlayerName = playerName,
            GamesWon = gamesWon,
            GamesLost = gamesLost,
            GamesInProgress = gamesInProgress
        };
    }
}