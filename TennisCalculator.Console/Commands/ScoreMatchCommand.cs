using TennisCalculator.Domain;

namespace TennisCalculator.Console.Commands;

internal class ScoreMatchCommand(ITennisMatchRepository matchRepository) : IUserCommand
{
    public bool CanHandle(IList<string> command) => command.Count == 3 && command[0].Equals("Score", StringComparison.OrdinalIgnoreCase) && command[1].Equals("Match", StringComparison.OrdinalIgnoreCase);

    public async Task<string?> Handle(IList<string> command, CancellationToken cancellationToken = default)
    {
        var matchId = command[2];

        try
        {
            var match = await matchRepository.GetMatch(matchId);

            if (match == null)
            {
                return $"Match '{matchId}' was not found";
            }

            if (!match.HasWinner)
            {
                return $"Match '{matchId}' is still in progress. {match.Players[0].Name} vs {match.Players[1].Name}";
            }

            var winner = match.Winner;
            var loser = match.Players.First(p => !p.Equals(winner));

            // Calculate sets won by each player, if match has a winner, all sets should have a winner
            var winnerSets = match.Sets.Count(x => x.Winner == winner);
            var loserSets = match.Sets.Count - winnerSets;

            return $"{winner.Name} defeated {loser.Name}, {winnerSets} sets to {loserSets}";
        }
        catch (Exception ex)
        {
            return $"Error processing score match query: {ex.Message}";
        }
    }
}
