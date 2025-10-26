# Implementation Plan

- [x] 1. Set up project structure with separate library projects
  - Create C# .NET console application project with latest .NET version
  - Create TennisCalculator.Domain class library project
  - Create TennisCalculator.GamePlay class library project  
  - Create TennisCalculator.DataAccess class library project
  - Add project references: Console → GamePlay → Domain, Console → DataAccess → Domain
  - Create TennisPoint enum with Love, Fifteen, Thirty, Forty, Advantage values in Domain project
  - Implement TennisPlayer record with case-insensitive equality in Domain project
  - _Requirements: 1.2, 1.3, 1.4_

- [x] 2. Implement core tennis domain entities in Domain project
  - Create TennisGame record with Players, PointHistory, CurrentScore, and Winner properties
  - Create TennisSet record with Players, Games, CurrentGame, and Winner properties
  - Create TennisMatch record with MatchId, Players, Sets, CurrentSet, and Winner properties
  - Add HasWinner computed properties to all entities
  - Create TennisPlayerStatistics record with GamesWon, GamesLost, GamesInProgress properties
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 3.1, 3.2, 3.3, 3.4, 3.5, 4.1, 4.2, 4.3, 4.4, 4.5_

- [x] 3. Create scoring strategy interfaces and implementations in GamePlay project
  - Define IGameScorer interface with AddPoint, DetermineGameWinner, and CalculateCurrentScore methods
  - Define ISetScorer interface with AddGame, DetermineSetWinner, and StartNewGame methods
  - Define IMatchScorer interface with AddSet, DetermineMatchWinner, and StartNewSet methods
  - Implement StandardGameScorer with tennis scoring rules (0, 15, 30, 40, deuce, advantage)
  - Implement StandardSetScorer with first-to-6-games rule
  - Implement StandardMatchScorer with best-of-3-sets rule
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 3.1, 4.1, 4.2_

- [x] 4. Implement data loading infrastructure in DataAccess project
  - Create IFileReader interface with async ReadLinesAsync and FileExistsAsync methods
  - Create IDataLoader interface with async LoadTournamentDataAsync and CanHandleAsync methods
  - Create ITournamentFileParser interface with async ParseTournamentFileAsync method
  - Create RawMatchData record for parsed tournament data in DataAccess project
  - Implement FileDataLoader with streaming file processing
  - Implement TournamentFileParser with async enumerable parsing
  - _Requirements: 1.1, 1.2, 1.3, 1.5, 1.6, 1.7, 1.8, 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 5. Create match repository and data access in DataAccess project
  - Define IMatchRepository interface with AddMatch, GetMatch, GetAllMatches, and GetMatchesForPlayer methods
  - Implement InMemoryMatchRepository with dictionary-based storage
  - Create TournamentProcessor service in GamePlay project to orchestrate data loading and match creation
  - Implement match creation logic that processes point sequences using scoring strategies
  - _Requirements: 1.4, 1.5, 1.6, 1.7, 1.8, 8.4, 8.5_

- [x] 6. Implement query handling system in GamePlay project
  - Create query record types: ScoreMatchQuery and GamesPlayerQuery
  - Define IQueryHandler interface for query processing
  - Implement ScoreMatchQueryHandler to format match results as "Winner defeated Loser, X sets to Y"
  - Implement GamesPlayerQueryHandler to calculate and format player statistics
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 6.1, 6.2, 6.3, 6.4, 6.5_

- [x] 7. Build console interface and command parsing in console application
  - Create Program class with Main method for command-line argument validation
  - Implement CommandLineInterface for interactive query processing
  - Create QueryParser to parse "Score Match <id>", "Games Player <name>", and "quit" commands
  - Add help text display for unrecognised commands
  - Implement graceful application termination on "quit" command
  - _Requirements: 1.1, 7.1, 7.2, 7.3, 7.4, 7.5, 7.6_

- [x] 8. Add error handling and validation across projects
  - Create custom exception types in Domain project: TennisCalculatorException, FileProcessingException, QueryProcessingException, UnsupportedDataSourceException
  - Implement file existence validation with descriptive error messages in DataAccess project
  - Add tournament file format validation during parsing in DataAccess project
  - Handle invalid match IDs and player names in queries with appropriate error messages in GamePlay project
  - Add command-line argument validation for missing file path in console application
  - _Requirements: 1.1, 1.3, 5.4, 6.4, 7.4, 8.1, 8.2, 8.3_

- [x] 9. Wire up dependency injection and application startup in console application
  - Configure dependency injection container with all services and interfaces
  - Register scoring strategies, data loaders, repository, and query handlers
  - Implement application startup flow: validate arguments → load tournament data → start interactive mode
  - Add proper async/await handling throughout the application pipeline
  - _Requirements: 1.1, 1.2, 7.1, 7.2, 8.4, 8.5_

- [x] 10. Create unit tests for core functionality
  - Create test projects for each library: TennisCalculator.Domain.Tests, TennisCalculator.GamePlay.Tests, TennisCalculator.DataAccess.Tests
  - Write tests for tennis scoring logic in StandardGameScorer, StandardSetScorer, StandardMatchScorer
  - Test TennisPlayer equality with case-insensitive name matching
  - Test query handlers with mock repository data
  - Test file parsing with sample tournament data
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 3.1, 3.2, 3.3, 3.4, 3.5, 4.1, 4.2, 4.3, 4.4, 4.5_
  
- [x] 11. Add integration tests with sample data
  - Create integration test project for end-to-end testing
  - Test complete application flow with full_tournament.txt file
  - Verify expected query results match README sample output
  - Test error scenarios with malformed tournament files
  - Test interactive command processing with various input combinations
  - _Requirements: 5.1, 5.2, 5.3, 5.5, 6.1, 6.2, 6.3, 6.5, 7.1, 7.2, 7.3, 7.4, 7.5, 7.6_