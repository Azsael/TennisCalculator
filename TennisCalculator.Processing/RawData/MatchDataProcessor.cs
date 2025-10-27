using TennisCalculator.Processing.Scoring;
using TennisCalculator.Domain;

namespace TennisCalculator.Processing.RawData;

internal class MatchDataProcessor(IMatchScorer matchScorer, ISetScorer setScorer, IGameScorer gameScorer) : IMatchDataProcessor
{
    public TennisMatch CreateMatchFromRawData(RawMatchData rawData)
    {
        var playerOne = new TennisPlayer { Name = rawData.Player1Name };
        var playerTwo = new TennisPlayer { Name = rawData.Player2Name };

        var games = gameScorer.ConvertPoints(playerOne, playerTwo, rawData.Points);
        var sets = setScorer.ConvertGames(games);

        return matchScorer.ConvertSets(rawData.MatchId, [playerOne, playerTwo], sets);
    }
}
