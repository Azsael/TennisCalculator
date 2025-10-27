using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

/// <summary>
/// Provides tennis match scoring functionality using best-of-3 sets format
/// </summary>
internal class StandardMatchScorer : IMatchScorer
{
    private const int SetsToWinMatch = 2;

    public TennisMatch ConvertSets(string matchId, IList<TennisPlayer> players, IEnumerable<TennisSet> sets)
    {
        var tennisSets = sets.ToList();

        var winner = tennisSets.GroupBy(g => g.Winner).Where(x => x.Count() >= SetsToWinMatch).Select(x => x.Key).FirstOrDefault();

        return new TennisMatch
        {
            MatchId = matchId,
            Players = [..players],
            Sets = tennisSets,
            Winner = winner
        };
    }
}