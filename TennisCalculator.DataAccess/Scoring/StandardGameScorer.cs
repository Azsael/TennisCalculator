using TennisCalculator.Domain;

namespace TennisCalculator.DataAccess.Scoring;

internal class StandardGameScorer : IGameScorer
{
    public TennisGame AddPoint(TennisGame game, TennisPlayer pointWinner)
    {
        var newPointHistory = game.PointHistory.Append(pointWinner).ToList();
        var newCurrentScore = CalculateCurrentScore(newPointHistory, game.Players);
        var winner = DetermineGameWinner(game with { PointHistory = newPointHistory, CurrentScore = newCurrentScore });
        
        return game with 
        { 
            PointHistory = newPointHistory, 
            CurrentScore = newCurrentScore,
            Winner = winner
        };
    }
    
    public TennisPlayer? DetermineGameWinner(TennisGame game)
    {
        var pointCounts = game.Players.ToDictionary(p => p, p => game.PointHistory.Count(ph => ph.Equals(p)));
        
        var player1 = game.Players[0];
        var player2 = game.Players[1];
        var player1Points = pointCounts[player1];
        var player2Points = pointCounts[player2];
        
        // Need at least 4 points to win
        if (player1Points >= 4 || player2Points >= 4)
        {
            var pointDifference = Math.Abs(player1Points - player2Points);
            
            // Win by 2 or more points
            if (pointDifference >= 2)
            {
                return player1Points > player2Points ? player1 : player2;
            }
        }
        
        return null; // No winner yet
    }
    
    public IReadOnlyDictionary<TennisPlayer, TennisPoint> CalculateCurrentScore(IReadOnlyList<TennisPlayer> pointHistory, IReadOnlyList<TennisPlayer> players)
    {
        var pointCounts = players.ToDictionary(p => p, p => pointHistory.Count(ph => ph.Equals(p)));
        
        var player1 = players[0];
        var player2 = players[1];
        var player1Points = pointCounts[player1];
        var player2Points = pointCounts[player2];
        
        // Handle deuce and advantage scenarios
        if (player1Points >= 3 && player2Points >= 3)
        {
            var pointDifference = player1Points - player2Points;
            
            if (pointDifference == 0)
            {
                // Deuce - both at 40
                return new Dictionary<TennisPlayer, TennisPoint>
                {
                    { player1, TennisPoint.Forty },
                    { player2, TennisPoint.Forty }
                };
            }
            else if (pointDifference == 1)
            {
                // Player1 has advantage
                return new Dictionary<TennisPlayer, TennisPoint>
                {
                    { player1, TennisPoint.Advantage },
                    { player2, TennisPoint.Forty }
                };
            }
            else if (pointDifference == -1)
            {
                // Player2 has advantage
                return new Dictionary<TennisPlayer, TennisPoint>
                {
                    { player1, TennisPoint.Forty },
                    { player2, TennisPoint.Advantage }
                };
            }
        }
        
        // Standard scoring
        return new Dictionary<TennisPlayer, TennisPoint>
        {
            { player1, ConvertPointsToTennisPoint(player1Points) },
            { player2, ConvertPointsToTennisPoint(player2Points) }
        };
    }
    
    private static TennisPoint ConvertPointsToTennisPoint(int points)
    {
        return points switch
        {
            0 => TennisPoint.Love,
            1 => TennisPoint.Fifteen,
            2 => TennisPoint.Thirty,
            _ => TennisPoint.Forty
        };
    }
}