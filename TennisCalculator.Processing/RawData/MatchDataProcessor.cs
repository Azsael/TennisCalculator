using TennisCalculator.Processing.Data;
using TennisCalculator.Processing.Scoring;
using TennisCalculator.Domain;

namespace TennisCalculator.Processing.RawData;

internal class MatchDataProcessor(
    IGameScorer gameScorer,
    ISetScorer setScorer,
    IMatchScorer matchScorer) : IMatchDataProcessor
{
    public TennisMatch CreateMatchFromRawData(RawMatchData rawData)
    {
        var players = new List<TennisPlayer>
        {
            new() { Name = rawData.Player1Name },
            new() { Name = rawData.Player2Name }
        };

        // Initialize match
        var match = new TennisMatch
        {
            MatchId = rawData.MatchId,
            Players = players,
            Sets = new List<TennisSet>()
        };

        // Start first set
        match = matchScorer.StartNewSet(match, players);

        // Start first game in the set
        match = match with
        {
            CurrentSet = setScorer.StartNewGame(match.CurrentSet!, players)
        };

        // Process each point
        foreach (var point in rawData.Points)
        {
            if (match.HasWinner)
                break;

            var pointWinner = point == 0 ? players[0] : players[1];
            match = ProcessPoint(match, pointWinner);
        }

        return match;

    }
    /// <summary>
    /// Processes a single point and updates match state accordingly
    /// </summary>
    /// <param name="match">Current match state</param>
    /// <param name="pointWinner">Player who won the point</param>
    /// <returns>Updated match state</returns>
    private TennisMatch ProcessPoint(TennisMatch match, TennisPlayer pointWinner)
    {
        if (match.CurrentSet?.CurrentGame is null)
            return match;

        // Add point to current game
        var updatedGame = gameScorer.AddPoint(match.CurrentSet.CurrentGame, pointWinner);

        // Update the current set with the updated game
        var updatedSet = match.CurrentSet with { CurrentGame = updatedGame };
        match = match with { CurrentSet = updatedSet };

        // Check if game is won
        if (updatedGame.HasWinner)
        {
            // Add completed game to set
            updatedSet = setScorer.AddGame(match.CurrentSet!, updatedGame);
            match = match with { CurrentSet = updatedSet };

            // Check if set is won
            if (updatedSet.HasWinner)
            {
                // Add completed set to match
                match = matchScorer.AddSet(match, updatedSet);

                // Check if match is won
                if (!match.HasWinner)
                {
                    // Start new set
                    match = matchScorer.StartNewSet(match, match.Players);
                    match = match with
                    {
                        CurrentSet = setScorer.StartNewGame(match.CurrentSet!, match.Players)
                    };
                }
            }
            else
            {
                // Start new game in current set
                match = match with
                {
                    CurrentSet = setScorer.StartNewGame(updatedSet, match.Players)
                };
            }
        }

        return match;
    }
}
