using TennisCalculator.Domain;

namespace TennisCalculator.DataAccess.Scoring;

internal class StandardSetScorer : ISetScorer
{
    public TennisSet AddGame(TennisSet set, TennisGame completedGame)
    {
        var newGames = set.Games.Append(completedGame).ToList();
        var newSet = set with { Games = newGames, CurrentGame = null };
        var winner = DetermineSetWinner(newSet);
        
        return newSet with { Winner = winner };
    }
    
    public TennisPlayer? DetermineSetWinner(TennisSet set)
    {
        var gamesWon = set.Players.ToDictionary(p => p, p => 
            set.Games.Count(g => g.HasWinner && g.Winner!.Equals(p)));
        
        // First to 6 games wins (simplified rules)
        foreach (var player in set.Players)
        {
            if (gamesWon[player] >= 6)
            {
                return player;
            }
        }
        
        return null; // No winner yet
    }
    
    public TennisSet StartNewGame(TennisSet set, IReadOnlyList<TennisPlayer> players)
    {
        var initialScore = players.ToDictionary(p => p, _ => TennisPoint.Love);
        var newGame = new TennisGame
        {
            Players = players,
            PointHistory = new List<TennisPlayer>(),
            CurrentScore = initialScore
        };
        
        return set with { CurrentGame = newGame };
    }
}