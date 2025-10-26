# Implementation Plan - Refactoring Phase

This implementation plan covers the architectural refactoring to eliminate the GamePlay library and improve separation of concerns.

- [x] 1. Complete initial project structure setup (COMPLETED)
  - C# .NET console application project with latest .NET version
  - TennisCalculator.Domain class library project
  - TennisCalculator.DataAccess class library project (to be renamed to Processing)
  - TennisCalculator.GamePlay class library project (to be eliminated)
  - TennisPoint enum and TennisPlayer record in Domain project
  - _Requirements: 1.2, 1.3, 1.4_

- [x] 2. Core tennis domain entities (COMPLETED)
  - TennisGame, TennisSet, TennisMatch records in Domain project
  - TennisPlayerStatistics record with computed properties
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 3.1, 3.2, 3.3, 3.4, 3.5, 4.1, 4.2, 4.3, 4.4, 4.5_

- [x] 3. Move scoring interfaces from GamePlay to DataAccess (PARTIALLY COMPLETED)
  - IGameScorer, ISetScorer, IMatchScorer interfaces moved to DataAccess/Scoring
  - StandardGameScorer, StandardSetScorer, StandardMatchScorer implementations moved
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 3.1, 4.1, 4.2, 9.3_

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
  
- [x] 11. Integration tests (COMPLETED)
  - Integration test project for end-to-end testing
  - Test complete application flow with full_tournament.txt file
  - _Requirements: 5.1, 5.2, 5.3, 5.5, 6.1, 6.2, 6.3, 6.5, 7.1, 7.2, 7.3, 7.4, 7.5, 7.6_

- [ ] 12. Rename TennisCalculator.DataAccess to TennisCalculator.Processing





  - Rename the TennisCalculator.DataAccess project folder to TennisCalculator.Processing
  - Update the .csproj file name from TennisCalculator.DataAccess.csproj to TennisCalculator.Processing.csproj
  - Update the project namespace declarations in all files within the project
  - Update all project references in other projects to reference TennisCalculator.Processing
  - Update using statements in all consuming projects to use TennisCalculator.Processing namespaces
  - _Requirements: 9.1, 9.2, 9.6, 9.7_

- [ ] 13. Move query handlers from GamePlay to Console project
  - Move IQueryHandler, IScoreMatchQueryHandler, IGamesPlayerQueryHandler interfaces to Console/Commands
  - Move ScoreMatchQuery, ScoreMatchQueryHandler classes to Console/Commands
  - Move GamesPlayerQuery, GamesPlayerQueryHandler classes to Console/Commands
  - Update CommandLineProcessor to use local query handler interfaces instead of GamePlay references
  - Update dependency injection configuration to register query handlers in Console project
  - _Requirements: 9.4, 9.5, 9.8_

- [ ] 14. Update project references and eliminate GamePlay library
  - Remove TennisCalculator.GamePlay project reference from Console project
  - Remove TennisCalculator.GamePlay project from solution
  - Delete TennisCalculator.GamePlay project folder and all its contents
  - Update Console project to reference TennisCalculator.Processing directly
  - Verify all project references are correctly updated
  - _Requirements: 9.5, 9.6_

- [ ] 15. Update comments, documentation, and naming consistency
  - Review all code comments and XML documentation for outdated references to DataAccess or GamePlay
  - Update interface and class documentation to reflect new architecture
  - Ensure consistent naming conventions across all projects
  - Update README.md to reflect new project structure
  - Update any remaining references to old library names in error messages or logs
  - _Requirements: 9.7, 9.8_

- [ ] 16. Expand QueryParser to CommandProcessor functionality
  - Rename IQueryParser to ICommandParser to better reflect its purpose
  - Rename QueryParser class to CommandParser
  - Update method names and documentation to use "command" terminology instead of "query"
  - Enhance command parsing to support future extensibility
  - Update all references to use the new command-focused naming
  - _Requirements: 9.8_