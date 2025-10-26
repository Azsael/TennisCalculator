# Requirements Document

## Introduction

The Tennis Calculator is a C# .NET console application that processes tennis match data from input files and provides statistical analysis. The system parses match point sequences, calculates game and set outcomes using simplified tennis scoring rules, and responds to user commands about match results and player statistics. This document covers the architectural refactoring to improve separation of concerns and eliminate the GamePlay library in favor of a cleaner domain-driven design.

## Glossary

- **Tennis_Calculator**: The main C# .NET console application system that processes tennis match data
- **Processing_Library**: The TennisCalculator.Processing library responsible for data loading, file parsing, and match processing
- **Domain_Library**: The TennisCalculator.Domain library containing core tennis entities and business logic
- **Console_Application**: The main console application handling user interaction and command processing
- **Match**: A tennis competition between two players consisting of multiple sets in a best-of-3 format
- **Set**: A collection of games where the first player to win 6 games wins the set
- **Game**: A scoring unit where a player must win 4 points and be ahead by at least 2 points
- **Point**: The smallest scoring unit in tennis (0, 15, 30, 40, deuce, advantage, game)
- **Tournament_File**: A text file containing multiple match data with header information and point sequences
- **User_Command**: A user input requesting specific match or player statistics or application control
- **Command_Processor**: The component responsible for parsing and executing user commands
- **Player_Statistics**: Aggregated data about games won and lost for a specific player
- **Match_Result**: The outcome of a completed match showing winner and set scores
- **Help_Text**: Instructional text displaying available commands and their formats

## Requirements

### Requirement 1

**User Story:** As a tennis tournament organiser, I want to process match data from input files, so that I can automatically calculate match results and maintain accurate records.

#### Acceptance Criteria

1. WHEN the Tennis_Calculator is started without a file path argument, THE Tennis_Calculator SHALL display an error message indicating missing file and exit
2. WHEN a Tournament_File path is provided as a command-line argument, THE Tennis_Calculator SHALL attempt to load and parse the file
3. WHEN the specified Tournament_File does not exist, THE Tennis_Calculator SHALL display an appropriate error message and exit gracefully
4. WHEN processing match headers from the Tournament_File, THE Tennis_Calculator SHALL parse lines in format "Match: <id>" followed by "<Player1> vs <Player2>"
5. WHEN processing point sequences from the Tournament_File, THE Tennis_Calculator SHALL ignore blank lines and process only valid point entries
6. WHEN a point entry contains "0", THE Tennis_Calculator SHALL award the point to the first player listed in the match header
7. WHEN a point entry contains "1", THE Tennis_Calculator SHALL award the point to the second player listed in the match header
8. THE Tennis_Calculator SHALL validate that each point entry contains only "0" or "1" values

### Requirement 2

**User Story:** As a tennis scoring system, I want to calculate game outcomes using tennis scoring rules, so that match progression is accurately tracked.

#### Acceptance Criteria

1. WHEN a player reaches 4 points and leads by at least 2 points, THE Tennis_Calculator SHALL award the game to that player
2. WHEN both players reach 3 points, THE Tennis_Calculator SHALL enter deuce state for the current game
3. WHILE in deuce state, THE Tennis_Calculator SHALL require a player to win 2 consecutive points to win the game
4. WHEN a player wins a point during deuce, THE Tennis_Calculator SHALL set that player to advantage state
5. WHEN a player with advantage wins the next point, THE Tennis_Calculator SHALL award the game to that player

### Requirement 3

**User Story:** As a tennis match tracker, I want to determine set winners using simplified scoring rules, so that match progression follows tournament standards.

#### Acceptance Criteria

1. WHEN a player wins 6 games in a set, THE Tennis_Calculator SHALL award the set to that player regardless of the opponent's game count
2. THE Tennis_Calculator SHALL start a new set immediately after a set is completed
3. THE Tennis_Calculator SHALL track the current set number for each match
4. THE Tennis_Calculator SHALL reset game counts to zero when starting a new set
5. THE Tennis_Calculator SHALL maintain a record of completed sets for match result queries

### Requirement 4

**User Story:** As a tournament administrator, I want to determine match winners using best-of-three format, so that matches conclude according to tournament rules.

#### Acceptance Criteria

1. WHEN a player wins 2 sets in a match, THE Tennis_Calculator SHALL declare that player the match winner
2. THE Tennis_Calculator SHALL end match processing when a winner is determined
3. THE Tennis_Calculator SHALL store the final match result with winner and set scores
4. THE Tennis_Calculator SHALL support matches that go to 3 sets (2-1 results)
5. THE Tennis_Calculator SHALL prevent processing additional points after match completion

### Requirement 5

**User Story:** As a user, I want to query specific match results, so that I can retrieve winner information and set scores for any completed match.

#### Acceptance Criteria

1. WHEN receiving a "Score Match <id>" Query_Command, THE Tennis_Calculator SHALL locate the match with the specified ID
2. WHEN a match ID exists in the system, THE Tennis_Calculator SHALL display the winner and loser names
3. WHEN displaying match results, THE Tennis_Calculator SHALL show set scores with the winning player's score first
4. WHEN a requested match ID does not exist, THE Tennis_Calculator SHALL display an appropriate error message
5. THE Tennis_Calculator SHALL format match results as "{Winner} defeated {Loser}" followed by "{WinnerSets} sets to {LoserSets}"

### Requirement 6

**User Story:** As a tournament statistician, I want to query player game statistics, so that I can analyse individual player performance across all matches.

#### Acceptance Criteria

1. WHEN receiving a "Games Player <Player Name>" Query_Command, THE Tennis_Calculator SHALL calculate total games won and lost for the specified player
2. THE Tennis_Calculator SHALL aggregate game statistics across all matches where the player participated
3. WHEN displaying player statistics, THE Tennis_Calculator SHALL show games won followed by games lost separated by a space
4. WHEN a player name does not exist in any match data, THE Tennis_Calculator SHALL display an appropriate error message
5. THE Tennis_Calculator SHALL perform case-sensitive matching for player names in queries

### Requirement 7

**User Story:** As a user, I want to interact with the application through a command-line interface, so that I can query match and player statistics interactively.

#### Acceptance Criteria

1. THE Tennis_Calculator SHALL process multiple matches from a single Tournament_File sequentially
2. THE Tennis_Calculator SHALL read Query_Commands from standard input after loading the Tournament_File
3. WHEN receiving a "quit" Query_Command, THE Tennis_Calculator SHALL terminate the application gracefully
4. WHEN receiving an unrecognised Query_Command, THE Tennis_Calculator SHALL display Help_Text showing available query formats
5. WHEN displaying Help_Text, THE Tennis_Calculator SHALL list "Score Match <id>", "Games Player <Player Name>", and "quit" commands
6. WHEN displaying Help_Text, THE Tennis_Calculator SHALL prompt the user to try again

### Requirement 8

**User Story:** As a developer, I want the system to handle input processing reliably, so that the application can process tournament data files without errors.

#### Acceptance Criteria

1. WHEN the Tournament_File contains malformed data, THE Tennis_Calculator SHALL display descriptive error messages
2. THE Tennis_Calculator SHALL continue processing subsequent matches even if one match contains errors
3. THE Tennis_Calculator SHALL validate match header format before processing point sequences
4. THE Tennis_Calculator SHALL handle Tournament_Files containing multiple matches as demonstrated in full_tournament.txt
5. THE Tennis_Calculator SHALL maintain match state separately for each match in the Tournament_File

### Requirement 9

**User Story:** As a developer, I want to refactor the application architecture to improve separation of concerns, so that the codebase is more maintainable and follows domain-driven design principles.

#### Acceptance Criteria

1. THE Tennis_Calculator SHALL rename the TennisCalculator.DataAccess library to TennisCalculator.Processing
2. THE Processing_Library SHALL contain all data loading, file parsing, and match processing functionality
3. THE Domain_Library SHALL contain all tennis scoring logic, entities, and business rules previously in GamePlay
4. THE Console_Application SHALL contain all user interaction and command processing functionality previously in GamePlay
5. THE Tennis_Calculator SHALL eliminate the TennisCalculator.GamePlay library entirely
6. THE Tennis_Calculator SHALL update all project references to reflect the new library structure
7. THE Tennis_Calculator SHALL ensure all comments, documentation, and naming conventions are consistent with the new architecture
8. THE Command_Processor SHALL replace the previous query parser with expanded functionality for user input processing