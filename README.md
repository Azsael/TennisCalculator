# Tennis Calculator

The tennis calculator takes a set of scores as inputs and produces useful statistics based on those scores.

This calculator will used a simplified version of scoring where whoever gets to 6 games first wins the set

## Overview

The Tennis Calculator takes inputs in the form of a list of points of a tennis match. 

Given this list of points, it will calculate the "games", "sets" and "matches" results.

From there it can be queried about various statistics around the input matches it received. 

## Input

The input will have some header lines, and then a list of points. 
For example:, the following would result in 2 games to "Person A":

    Match: 01
    Person A vs Person B
    0
    1
    0
    1
    0
    0
    0
    0
    0
    0

    
The first row is a match id, the second row shows who is playing against whom.
After that are a series of points, where 0 is a point for the first person listed, 1 is for last person.

i.e.

| Input                | Score   |
|----------------------|---------|
| Match: 01            |         |
| Person A vs Person B |         |
| 0                    | 15 - 0  |
| 1                    | 15 - 15 |
| 0                    | 30 - 15 |
| 1                    | 30 - 30 |
| 0                    | 40 - 30 |
| 0                    | Game    |
| 0                    | 15 - 0  |
| 0                    | 30 - 0  |
| 0                    | 40 - 0  |
| 0                    | Game    |


For processing, blank lines must be ignored

## Queries

### Query match result
Query scores for a particular match
Prints who defeated whom, and the result of the sets for the match (winning player score first).

Query: `Score Match <id>`

Example: `Score Match 01`

Example output:

    Person A defeated Person B
    2 sets to 0
 
### Query games for player
Prints a summary of games won vs lost for a particular player over the tournament
Query: `Games Player <Player Name>`

Example: `Games Player Person A`

Example output:

    23 17

## Sample output
Running the application against the 'full_tournament.txt' file results in the following:

    $ tennis_calculator_app test/test_data/full_tournament.txt << EOF
    Score Match 02
    Games Player Person A
    EOF
    
    Person C defeated Person A
    2 sets to 1
    
    23 17
    


## Scoring Rules
Details of tennis scoring can be found online. See here for reference:  
https://en.wikipedia.org/wiki/Tennis_scoring_system

The variation used for this application is a best of 3 sets match, with first to 6 games wins a set. 

Details as follows:
* A tennis match is split up into points, games and sets.
* Winning a game requires a person to win 4 points, but they must be ahead by at least 2 points (deuce, advantage, game)
* The first player to win 6 games wins a set. I.e:
    * Players do NOT need to be ahead by 2 to win a set (6-5 finishes a set) 
    * There is nothing special about that final game in a set. All games are the same.
* Best of 3 sets (first to 2 sets wins).

