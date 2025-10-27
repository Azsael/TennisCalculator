using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

/// <summary>
/// Provides tennis match scoring functionality a particular rule set
/// </summary>
internal interface IMatchScorer
{
    TennisMatch ConvertSets(string matchId, IList<TennisPlayer> players, IEnumerable<TennisSet> sets);
}