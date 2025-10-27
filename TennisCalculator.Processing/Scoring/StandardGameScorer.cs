using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

/// <summary>
/// Provides tennis game scoring functionality using standard tennis rules
/// </summary>
internal class StandardGameScorer : IGameScorer
{
    public IEnumerable<TennisGame> ConvertPoints(TennisPlayer playerOne, TennisPlayer playerTwo, IList<int> points)
    {
        var pointHistory = new List<TennisPlayer>();
        var scores = (TennisPoint.Love, TennisPoint.Love);

        foreach (var point in points)
        {
            var pointWinner = point == 0 ? playerOne : playerTwo;
            pointHistory.Add(pointWinner);

            scores = UpdateScores(scores, point == 0);

            if (scores.Item1 == TennisPoint.Game || scores.Item2 == TennisPoint.Game)
            {
                yield return new TennisGame
                {
                    Players = [playerOne, playerTwo],
                    PointHistory = [.. pointHistory],
                    CurrentScore = new Dictionary<TennisPlayer, TennisPoint>
                    {
                        { playerOne, scores.Item1 },
                        { playerTwo, scores.Item2 }
                    },
                    Winner = scores.Item1 == TennisPoint.Game ? playerOne : playerTwo
                };
                // Reset for next game
                pointHistory.Clear();
                scores = (TennisPoint.Love, TennisPoint.Love);
            }
        }

        // an unfinished game
        if (pointHistory.Count > 0)
        {
            yield return new TennisGame
            {
                Players = [playerOne, playerTwo],
                PointHistory = [.. pointHistory],
                CurrentScore = new Dictionary<TennisPlayer, TennisPoint>
                {
                    { playerOne, scores.Item1 },
                    { playerTwo, scores.Item2 }
                },
                Winner = null
            };
        }
    }

    private (TennisPoint PlayerOne, TennisPoint PlayerTwo) UpdateScores((TennisPoint PlayerOne, TennisPoint PlayerTwo) scores, bool playerOneScored)
    {
        return (
            AdjustPointScore(scores.PlayerOne, playerOneScored, scores.PlayerTwo >= TennisPoint.Forty),
            AdjustPointScore(scores.PlayerTwo, !playerOneScored, scores.PlayerOne >= TennisPoint.Forty)
        );
    }

    /// <summary>
    /// Adjusts score point based upon whether the player won the point and if the opponent is at deuce or advantage
    /// </summary>
    /// <param name="point">Current point standing</param>
    /// <param name="wonPoint">They won the point</param>
    /// <param name="potentialDeuce">Other player is at 40 or Advantage</param>
    /// <returns></returns>
    private static TennisPoint AdjustPointScore(TennisPoint point, bool wonPoint, bool potentialDeuce)
    {
        if (!wonPoint)
        {
            // If the player did not win the point, their score remains unchanged unless they had advantage which they lose
            return point == TennisPoint.Advantage ? TennisPoint.Forty : point;
        }

        return point switch
        {
            TennisPoint.Love => TennisPoint.Fifteen,
            TennisPoint.Fifteen => TennisPoint.Thirty,
            TennisPoint.Thirty => TennisPoint.Forty,
            TennisPoint.Forty => potentialDeuce ? TennisPoint.Advantage : TennisPoint.Game,
            TennisPoint.Advantage => TennisPoint.Game,
            _ => point
        };
    }
}