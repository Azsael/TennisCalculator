# Design Document

## Overview

The Tennis Calculator is a C# .NET console application that implements a domain-driven design approach to process tennis tournament data. The system follows clean architecture principles with clear separation between domain logic, data processing, and user interface concerns. The application processes tournament files containing multiple matches, calculates scores using tennis rules, and provides an interactive query interface.

## Architecture

The application follows a layered architecture pattern:

```
┌─────────────────────────────────────┐
│           Console Interface         │  ← User interaction, command parsing
├─────────────────────────────────────┤
│          Application Layer          │  ← Query handlers, orchestration
├─────────────────────────────────────┤
│            Domain Layer             │  ← Tennis scoring logic, entities
├─────────────────────────────────────┤
│        Infrastructure Layer         │  ← File I/O, data persistence
└─────────────────────────────────────┘
```

### Key Architectural Decisions

1. **Domain-Driven Design**: Core tennis concepts (Match, TennisSet, TennisGame, Player) are modelled as first-class entities using records
2. **Immutable Scoring**: Each scoring action creates new state rather than mutating existing state using record with-expressions
3. **Strategy Pattern**: Scoring logic is abstracted to support different tennis scoring variations
4. **Strategy Pattern**: Data loading is abstracted to support different data sources (files, APIs, databases)
5. **Command Pattern**: User queries are handled through discrete command objects
6. **Repository Pattern**: Match data access is abstracted through repository interfaces
7. **Required Properties**: Non-nullable properties use the required keyword for compile-time safety

## Components and Interfaces

### Domain Layer

#### Core Entities

**TennisPoint**
```csharp
public enum TennisPoint { Love = 0, Fifteen = 15, Thirty = 30, Forty = 40, Advantage = 50 }
```

**TennisPlayer**
```csharp
public record TennisPlayer
{
    public required string Name { get; init; }
    
    public virtual bool Equals(TennisPlayer? other) => 
        other is not null && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        
    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);
}
```

**TennisGame**
```csharp
public record TennisGame
{
    public required IReadOnlyList<TennisPlayer> Players { get; init; }
    public required IReadOnlyList<TennisPlayer> PointHistory { get; init; } // Ordered history of who scored each point
    public required IReadOnlyDictionary<TennisPlayer, TennisPoint> CurrentScore { get; init; } // Each player's current tennis point score
    public TennisPlayer? Winner { get; init; }
    
    public bool HasWinner => Winner is not null;
}
```

**TennisSet**
```csharp
public record TennisSet
{
    public required IReadOnlyList<TennisPlayer> Players { get; init; }
    public required IReadOnlyList<TennisGame> Games { get; init; }
    public TennisGame? CurrentGame { get; init; }
    public TennisPlayer? Winner { get; init; }
    
    // Calculated properties
    public IReadOnlyDictionary<TennisPlayer, int> GamesWon => 
        Games.Where(g => g.HasWinner)
             .GroupBy(g => g.Winner!)
             .ToDictionary(g => g.Key, g => g.Count());
             
    public bool HasWinner => Winner is not null;
}
```

**TennisMatch**
```csharp
public record TennisMatch
{
    public required string MatchId { get; init; }
    public required IReadOnlyList<TennisPlayer> Players { get; init; }
    public required IReadOnlyList<TennisSet> Sets { get; init; }
    public TennisSet? CurrentSet { get; init; }
    public TennisPlayer? Winner { get; init; }
    
    public bool HasWinner => Winner is not null;
}
```

#### Value Objects



**TennisPlayerStatistics**
```csharp
public record TennisPlayerStatistics
{
    public required string PlayerName { get; init; }
    public required int GamesWon { get; init; }
    public required int GamesLost { get; init; }
    public required int GamesInProgress { get; init; }
    
    public string FormattedStats => $"{GamesWon} {GamesLost}";
}
```



#### Scoring Interfaces

**IGameScorer**
```csharp
public interface IGameScorer
{
    TennisGame AddPoint(TennisGame game, TennisPlayer pointWinner);
    TennisPlayer? DetermineGameWinner(TennisGame game);
    IReadOnlyDictionary<TennisPlayer, TennisPoint> CalculateCurrentScore(IReadOnlyList<TennisPlayer> pointHistory, IReadOnlyList<TennisPlayer> players);
}
```

**ISetScorer**
```csharp
public interface ISetScorer
{
    TennisSet AddGame(TennisSet set, TennisGame completedGame);
    TennisPlayer? DetermineSetWinner(TennisSet set);
    TennisSet StartNewGame(TennisSet set, IReadOnlyList<TennisPlayer> players);
}
```

**IMatchScorer**
```csharp
public interface IMatchScorer
{
    TennisMatch AddSet(TennisMatch match, TennisSet completedSet);
    TennisPlayer? DetermineMatchWinner(TennisMatch match);
    TennisMatch StartNewSet(TennisMatch match, IReadOnlyList<TennisPlayer> players);
}
```

**StandardGameScorer**
```csharp
public class StandardGameScorer : IGameScorer
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
        // Implement standard tennis game scoring logic
    }
    
    public IReadOnlyDictionary<TennisPlayer, TennisPoint> CalculateCurrentScore(IReadOnlyList<TennisPlayer> pointHistory, IReadOnlyList<TennisPlayer> players)
    {
        // Calculate current tennis point scores based on point history
    }
}
```

**StandardSetScorer**
```csharp
public class StandardSetScorer : ISetScorer
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
        // First to 6 games wins (simplified rules)
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
```

**StandardMatchScorer**
```csharp
public class StandardMatchScorer : IMatchScorer
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
        // Best of 3 sets - first to win 2 sets
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
```

**IDataLoader**
```csharp
public interface IDataLoader
{
    Task<IEnumerable<RawMatchData>> LoadTournamentDataAsync(string source);
    Task<bool> CanHandleAsync(string source);
}
```

**FileDataLoader**
```csharp
public class FileDataLoader : IDataLoader
{
    private readonly IFileReader _fileReader;
    private readonly ITournamentFileParser _parser;
    
    public async Task<IEnumerable<RawMatchData>> LoadTournamentDataAsync(string filePath)
    {
        var lines = _fileReader.ReadLinesAsync(filePath);
        var matches = new List<RawMatchData>();
        
        await foreach (var match in _parser.ParseTournamentFileAsync(lines))
        {
            matches.Add(match);
        }
        
        return matches;
    }
    
    public async Task<bool> CanHandleAsync(string source) => await _fileReader.FileExistsAsync(source);
}
```

### Application Layer

#### Query Handlers

**IQueryHandler Interface**
```csharp
public interface IQueryHandler<TQuery, TResult>
{
    TResult Handle(TQuery query);
}
```

**Match Score Query**
```csharp
public class ScoreMatchQuery
{
    public string MatchId { get; set; }
}

public class ScoreMatchQueryHandler : IQueryHandler<ScoreMatchQuery, string>
{
    private readonly IMatchRepository _matchRepository;
    
    public string Handle(ScoreMatchQuery query)
    {
        // Retrieve match, format result
    }
}
```

**Player Games Query**
```csharp
public class GamesPlayerQuery
{
    public string PlayerName { get; set; }
}

public class GamesPlayerQueryHandler : IQueryHandler<GamesPlayerQuery, string>
{
    private readonly IMatchRepository _matchRepository;
    
    public string Handle(GamesPlayerQuery query)
    {
        // Aggregate player statistics across all matches
    }
}
```

#### Services

**TournamentProcessor**
```csharp
public class TournamentProcessor
{
    private readonly IEnumerable<IDataLoader> _dataLoaders;
    private readonly IMatchRepository _matchRepository;
    private readonly IScoringStrategy _scoringStrategy;
    
    public async Task ProcessTournamentDataAsync(string source)
    {
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
    
    private TennisMatch CreateMatchFromRawData(RawMatchData rawData)
    {
        // Convert raw data to TennisMatch entity using scoring interfaces
    }
}
```

### Infrastructure Layer

#### File Processing

**IFileReader Interface**
```csharp
public interface IFileReader
{
    IAsyncEnumerable<string> ReadLinesAsync(string filePath, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string filePath);
}
```

**ITournamentFileParser**
```csharp
public interface ITournamentFileParser
{
    IAsyncEnumerable<RawMatchData> ParseTournamentFileAsync(IAsyncEnumerable<string> lines, CancellationToken cancellationToken = default);
}
```

**TournamentFileParser**
```csharp
public class TournamentFileParser : ITournamentFileParser
{
    public async IAsyncEnumerable<RawMatchData> ParseTournamentFileAsync(
        IAsyncEnumerable<string> lines, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Stream parse match headers and point sequences without loading entire file
        await foreach (var line in lines.WithCancellation(cancellationToken))
        {
            // Process line by line, yield matches as they are completed
        }
    }
}
```

**RawMatchData**
```csharp
public record RawMatchData
{
    public required string MatchId { get; init; }
    public required string Player1Name { get; init; }
    public required string Player2Name { get; init; }
    public required IReadOnlyList<int> Points { get; init; }
}
```

#### Data Access

**IMatchRepository Interface**
```csharp
public interface IMatchRepository
{
    void AddMatch(TennisMatch match);
    TennisMatch? GetMatch(string matchId);
    IEnumerable<TennisMatch> GetAllMatches();
    IEnumerable<TennisMatch> GetMatchesForPlayer(string playerName);
}
```

**InMemoryMatchRepository**
```csharp
public class InMemoryMatchRepository : IMatchRepository
{
    private readonly Dictionary<string, TennisMatch> _matches = new();
    
    public void AddMatch(TennisMatch match) => _matches[match.MatchId] = match;
    
    public TennisMatch? GetMatch(string matchId) => 
        _matches.TryGetValue(matchId, out var match) ? match : null;
    
    public IEnumerable<TennisMatch> GetAllMatches() => _matches.Values;
    
    public IEnumerable<TennisMatch> GetMatchesForPlayer(string playerName) =>
        _matches.Values.Where(m => m.Players.Any(p => p.Name == playerName));
}
```

### Console Interface Layer

**Program**
```csharp
public class Program
{
    public static void Main(string[] args)
    {
        // Validate arguments, setup dependencies, start interactive loop
    }
}
```

**CommandLineInterface**
```csharp
public class CommandLineInterface
{
    private readonly Dictionary<string, IQueryHandler> _queryHandlers;
    
    public void StartInteractiveMode()
    {
        // Read commands from stdin, dispatch to handlers
    }
    
    private void DisplayHelp()
    {
        // Show available commands and formats
    }
}
```

**QueryParser**
```csharp
public class QueryParser
{
    public IQuery ParseQuery(string input)
    {
        // Parse "Score Match 01", "Games Player Person A", "quit"
    }
}
```

## Data Models

### File Format Structure

The tournament file follows this structure:
```
Match: <MatchId>
<Player1Name> vs <Player2Name>
<Point>  // 0 for Player1, 1 for Player2
<Point>
...
<BlankLine>  // Optional separator between matches
Match: <NextMatchId>
...
```

### Internal Data Flow

1. **File Processing**: Raw text → RawMatchData objects
2. **Domain Creation**: RawMatchData → Match entities with complete scoring
3. **Storage**: Match entities → Repository
4. **Querying**: Repository → Query handlers → Formatted output

## Error Handling

### File Processing Errors

**Missing File**
- Detect missing command-line argument
- Display: "Error: Please provide a tournament file path"
- Exit with code 1

**File Not Found**
- Check file existence before processing
- Display: "Error: Tournament file '{filePath}' not found"
- Exit with code 1

**Malformed Data**
- Invalid match header format
- Invalid point values (not 0 or 1)
- Display descriptive error with line number
- Continue processing subsequent matches

### Query Processing Errors

**Invalid Match ID**
- Display: "Error: Match '{matchId}' not found"
- Continue accepting queries

**Invalid Player Name**
- Display: "Error: Player '{playerName}' not found"
- Continue accepting queries

**Unrecognised Command**
- Display help text with available commands
- Prompt user to try again

### Exception Handling Strategy

```csharp
public class TennisCalculatorException : Exception
{
    public TennisCalculatorException(string message) : base(message) { }
}

public class FileProcessingException : TennisCalculatorException
{
    public required string FilePath { get; init; }
    public int? LineNumber { get; init; }
    
    public FileProcessingException(string message, string filePath, int? lineNumber = null) 
        : base(message)
    {
        FilePath = filePath;
        LineNumber = lineNumber;
    }
}

public class QueryProcessingException : TennisCalculatorException
{
    public required string Query { get; init; }
    
    public QueryProcessingException(string message, string query) : base(message)
    {
        Query = query;
    }
}

public class UnsupportedDataSourceException : TennisCalculatorException
{
    public required string Source { get; init; }
    
    public UnsupportedDataSourceException(string source) 
        : base($"No data loader available for source: {source}")
    {
        Source = source;
    }
}
```

## Testing Strategy

### Unit Testing Approach

**Domain Logic Testing**
- Test tennis scoring rules in isolation
- Verify game, set, and match progression
- Test edge cases (deuce scenarios, 6-0 sets)
- Use parameterised tests for scoring combinations

**Query Handler Testing**
- Mock repository dependencies
- Test query parsing and result formatting
- Verify error handling for invalid inputs

**File Processing Testing**
- Test with sample tournament files
- Verify parsing of multiple match formats
- Test error handling for malformed data

### Integration Testing

**End-to-End Scenarios**
- Process full_tournament.txt file
- Verify expected query results match sample output
- Test complete user interaction flows

**File I/O Testing**
- Test with various file formats and sizes
- Verify graceful handling of file system errors

### Test Data Strategy

**Sample Files**
- Use full_tournament.txt as primary test case
- Create minimal test files for specific scenarios
- Generate edge case files (empty matches, deuce games)

**Expected Results**
- Document expected outputs for sample queries
- Maintain test assertions based on README examples

### Testing Tools

- **xUnit**: Primary testing framework
- **Moq**: Mocking framework for dependencies
- **FluentAssertions**: Readable test assertions
- **Test Coverage**: Aim for >90% coverage on domain logic

## Performance Considerations

### Memory Management

- Process tournament files line-by-line to handle large files
- Use immutable objects to prevent accidental state mutation
- Implement efficient string handling for query parsing

### Scalability

- Repository pattern allows future database integration
- Command pattern supports adding new query types
- Modular design enables performance optimisation of individual components

The design prioritises clarity and maintainability over premature optimisation, following the CRITERIA.md guidelines for clean, readable, and pragmatic solutions.