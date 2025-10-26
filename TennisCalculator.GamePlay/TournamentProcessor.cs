using TennisCalculator.DataAccess;
using TennisCalculator.Domain;

namespace TennisCalculator.GamePlay;

/// <summary>
/// Service that orchestrates data loading and match creation
/// </summary>
public class TournamentProcessor(
    IEnumerable<IDataLoader> dataLoaders,
    IMatchRepository matchRepository,
    IGameScorer gameScorer,
    ISetScorer setScorer,
    IMatchScorer matchScorer) : ITournamentProcessor
{
    private readonly IEnumerable<IDataLoader> _dataLoaders = dataLoaders ?? throw new ArgumentNullException(nameof(dataLoaders));
    private readonly IMatchRepository _matchRepository = matchRepository ?? throw new ArgumentNullException(nameof(matchRepository));
    private readonly IGameScorer _gameScorer = gameScorer ?? throw new ArgumentNullException(nameof(gameScorer));
    private readonly ISetScorer _setScorer = setScorer ?? throw new ArgumentNullException(nameof(setScorer));
    private readonly IMatchScorer _matchScorer = matchScorer ?? throw new ArgumentNullException(nameof(matchScorer));
    
    /// <summary>
    /// Processes tournament data from the specified source
    /// </summary>
    /// <param name="source">Source identifier (e.g., file path)</param>
    /// <exception cref="UnsupportedDataSourceException">Thrown when no data loader can handle the source</exception>
    public async Task ProcessTournamentDataAsync(string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        
        IDataLoader? dataLoader = null;
        foreach (var loader in _dataLoaders)
        {
            if (await loader.CanHandleAsync(source))
            {
                dataLoader = loader;
                break;
            }
        }
        
        if (dataLoader is null)
            throw new UnsupportedDataSourceException(source);
            
        var rawMatches = await dataLoader.LoadTournamentDataAsync(source);
        
        foreach (var rawMatch in rawMatches)
        {
            var match = CreateMatchFromRawData(rawMatch);
            _matchRepository.AddMatch(match);
        }
    }
    
    /// <summary>
    /// Creates a TennisMatch entity from raw match data by processing point sequences
    /// </summary>
    /// <param name="rawData">Raw match data containing players and point sequence</param>
    /// <returns>Completed tennis match with all scoring calculated</returns>
    private TennisMatch CreateMatchFromRawData(RawMatchData rawData)
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
        match = _matchScorer.StartNewSet(match, players);
        
        // Start first game in the set
        match = match with 
        { 
            CurrentSet = _setScorer.StartNewGame(match.CurrentSet!, players) 
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
        var updatedGame = _gameScorer.AddPoint(match.CurrentSet.CurrentGame, pointWinner);
        
        // Update the current set with the updated game
        var updatedSet = match.CurrentSet with { CurrentGame = updatedGame };
        match = match with { CurrentSet = updatedSet };
        
        // Check if game is won
        if (updatedGame.HasWinner)
        {
            // Add completed game to set
            updatedSet = _setScorer.AddGame(match.CurrentSet!, updatedGame);
            match = match with { CurrentSet = updatedSet };
            
            // Check if set is won
            if (updatedSet.HasWinner)
            {
                // Add completed set to match
                match = _matchScorer.AddSet(match, updatedSet);
                
                // Check if match is won
                if (!match.HasWinner)
                {
                    // Start new set
                    match = _matchScorer.StartNewSet(match, match.Players);
                    match = match with 
                    { 
                        CurrentSet = _setScorer.StartNewGame(match.CurrentSet!, match.Players) 
                    };
                }
            }
            else
            {
                // Start new game in current set
                match = match with 
                { 
                    CurrentSet = _setScorer.StartNewGame(updatedSet, match.Players) 
                };
            }
        }
        
        return match;
    }
}

