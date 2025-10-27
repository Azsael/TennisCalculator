using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

/// <summary>
/// Provides tennis set scoring functionality using simplified rules (first to 6 games wins)
/// </summary>
internal class StandardSetScorer : ISetScorer
{
    private const int GamesToWinSet = 6;

    public IEnumerable<TennisSet> ConvertGames(IEnumerable<TennisGame> games)
    {
        var gameHistory = new List<TennisGame>();

        foreach (var game in games)
        {
            gameHistory.Add(game);

            var winner = gameHistory.GroupBy(g => g.Winner).Where(x => x.Count() >= GamesToWinSet).Select(x => x.Key).FirstOrDefault();

            if (winner is not null)
            {
                yield return new TennisSet
                {
                    Players = game.Players,
                    Games = [.. gameHistory],
                    Winner = winner
                };
                // Reset for next set
                gameHistory.Clear();
            }
        }

        // an unfinished set
        if (gameHistory.Count > 0)
        {
            yield return new TennisSet
            {
                Players = gameHistory[0].Players,
                Games = [.. gameHistory],
                Winner = null
            };
        }
    }
}