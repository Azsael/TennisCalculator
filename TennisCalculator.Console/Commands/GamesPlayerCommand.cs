using TennisCalculator.Domain;

namespace TennisCalculator.Console.Commands;

internal class GamesPlayerCommand(ITennisMatchRepository matchRepository) : IUserCommand
{
    public bool CanHandle(IList<string> command) => command.Count > 2 && command[0].Equals("Games", StringComparison.OrdinalIgnoreCase) && command[1].Equals("Player", StringComparison.OrdinalIgnoreCase);

    public async Task<string?> Handle(IList<string> command, CancellationToken cancellationToken = default)
    {
        var player = string.Join(' ', command.Skip(2));

        try
        {
            var playerMatches = await matchRepository.GetMatchesForPlayer(player);

            if (playerMatches.Count == 0) // player not found or not played, return 0 0
            {
                return "0 0";
            }

            var games = playerMatches.SelectMany(x => x.Sets).SelectMany(x => x.Games).ToList();

            var gamesWon = games.Count(x => x.HasWinner && x.Winner == player);
            var gamesLost = games.Count(x => x.HasWinner && x.Winner != player);

            return $"{gamesWon} {gamesLost}";
        }
        catch (Exception ex)
        {
            return $"Error processing games player query: {ex.Message}";
        }
    }
}
