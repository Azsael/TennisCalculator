using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

/// <summary>
/// Provides tennis game scoring functionality which could vary
/// </summary>
internal interface IGameScorer
{
    IEnumerable<TennisGame> ConvertPoints(TennisPlayer playerOne, TennisPlayer playerTwo, IList<int> points);

}