using TennisCalculator.DataAccess;
using TennisCalculator.Domain;

namespace TennisCalculator.GamePlay;

/// <summary>
/// Handles queries for match scores and results
/// </summary>
public class ScoreMatchQueryHandler : IQueryHandler<ScoreMatchQuery, string>
{
    private readonly IMatchRepository _matchRepository;

    public ScoreMatchQueryHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository ?? throw new ArgumentNullException(nameof(matchRepository));
    }

    /// <summary>
    /// Handles a score match query and returns formatted match result
    /// </summary>
    /// <param name="query">The score match query</param>
    /// <returns>Formatted match result as "Winner defeated Loser, X sets to Y"</returns>
    /// <exception cref="QueryProcessingException">Thrown when query processing fails</exception>
    public string Handle(ScoreMatchQuery query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query.MatchId))
            {
                throw new QueryProcessingException("Match ID cannot be null or empty", query.MatchId ?? string.Empty);
            }

            var match = _matchRepository.GetMatch(query.MatchId);
            
            if (match == null)
            {
                return $"Error: Match '{query.MatchId}' not found";
            }

            if (!match.HasWinner)
            {
                return $"Error: Match '{query.MatchId}' is not completed";
            }

            var winner = match.Winner!;
            var loser = match.Players.First(p => !p.Equals(winner));
            
            // Calculate sets won by each player
            var winnerSets = 0;
            var loserSets = 0;
            
            foreach (var set in match.Sets)
            {
                if (set.HasWinner)
                {
                    if (set.Winner!.Equals(winner))
                    {
                        winnerSets++;
                    }
                    else
                    {
                        loserSets++;
                    }
                }
            }

            return $"{winner.Name} defeated {loser.Name}, {winnerSets} sets to {loserSets}";
        }
        catch (QueryProcessingException)
        {
            // Re-throw query processing exceptions as-is
            throw;
        }
        catch (Exception ex)
        {
            throw new QueryProcessingException($"Error processing score match query: {ex.Message}", query.MatchId ?? string.Empty, ex);
        }
    }
}