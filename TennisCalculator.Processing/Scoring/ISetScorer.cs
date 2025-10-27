using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

/// <summary>
/// Provides tennis set scoring functionality which could vary
/// </summary>
public interface ISetScorer
{
    public IEnumerable<TennisSet> ConvertGames(IEnumerable<TennisGame> games);
}