using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Scoring;

internal class StandardMatchScorer : IMatchScorer
{
    public TennisMatch AddSet(TennisMatch match, TennisSet completedSet)
    {
        var newSets = match.Sets.Append(completedSet).ToList();
        var newMatch = match with { Sets = newSets, CurrentSet = null };
        var winner = DetermineMatchWinner(newMatch);
        
        return newMatch with { Winner = winner };
    }
    
    public TennisPlayer? DetermineMatchWinner(TennisMatch match)
    {
        var setsWon = match.Players.ToDictionary(p => p, p => 
            match.Sets.Count(s => s.HasWinner && s.Winner!.Equals(p)));
        
        // Best of 3 sets - first to win 2 sets
        foreach (var player in match.Players)
        {
            if (setsWon[player] >= 2)
            {
                return player;
            }
        }
        
        return null; // No winner yet
    }
    
    public TennisMatch StartNewSet(TennisMatch match, IReadOnlyList<TennisPlayer> players)
    {
        var newSet = new TennisSet
        {
            Players = players,
            Games = new List<TennisGame>()
        };
        
        return match with { CurrentSet = newSet };
    }
}