using System.Runtime.CompilerServices;
using TennisCalculator.DataAccess.IO;
using TennisCalculator.Domain;

namespace TennisCalculator.DataAccess.IO;

/// <summary>
/// Parser for tournament files that processes match data line by line
/// </summary>
public class TournamentFileParser : ITournamentFileParser
{
    /// <inheritdoc />
    public async IAsyncEnumerable<RawMatchData> ParseTournamentFileAsync(
        IAsyncEnumerable<string> lines,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? currentMatchId = null;
        string? player1Name = null;
        string? player2Name = null;
        var points = new List<int>();
        int lineNumber = 0;
        
        await foreach (var line in lines.WithCancellation(cancellationToken))
        {
            lineNumber++;
            var trimmedLine = line.Trim();
            
            // Skip empty lines
            if (string.IsNullOrEmpty(trimmedLine))
            {
                // If we have a complete match, yield it
                if (currentMatchId is not null && player1Name is not null && player2Name is not null)
                {
                    yield return new RawMatchData
                    {
                        MatchId = currentMatchId,
                        Player1Name = player1Name,
                        Player2Name = player2Name,
                        Points = points.AsReadOnly()
                    };
                    
                    // Reset for next match
                    currentMatchId = null;
                    player1Name = null;
                    player2Name = null;
                    points.Clear();
                }
                continue;
            }
            
            // Parse match header: "Match: <id>"
            if (trimmedLine.StartsWith("Match:", StringComparison.OrdinalIgnoreCase))
            {
                // If we have a previous complete match, yield it first
                if (currentMatchId is not null && player1Name is not null && player2Name is not null)
                {
                    yield return new RawMatchData
                    {
                        MatchId = currentMatchId,
                        Player1Name = player1Name,
                        Player2Name = player2Name,
                        Points = points.AsReadOnly()
                    };
                    points.Clear();
                }
                
                var matchIdPart = trimmedLine.Substring(6).Trim();
                if (string.IsNullOrWhiteSpace(matchIdPart))
                {
                    throw new FileProcessingException($"Invalid match header format at line {lineNumber}: Match ID cannot be empty", "tournament file", lineNumber);
                }
                
                currentMatchId = matchIdPart;
                player1Name = null;
                player2Name = null;
                continue;
            }
            
            // Parse player names: "<Player1> vs <Player2>"
            if (trimmedLine.Contains(" vs ", StringComparison.OrdinalIgnoreCase))
            {
                if (currentMatchId is null)
                {
                    throw new FileProcessingException($"Player names found at line {lineNumber} without preceding match header", "tournament file", lineNumber);
                }
                
                var parts = trimmedLine.Split(" vs ", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    throw new FileProcessingException($"Invalid player format at line {lineNumber}: Expected '<Player1> vs <Player2>'", "tournament file", lineNumber);
                }
                
                var p1 = parts[0].Trim();
                var p2 = parts[1].Trim();
                
                if (string.IsNullOrWhiteSpace(p1) || string.IsNullOrWhiteSpace(p2))
                {
                    throw new FileProcessingException($"Invalid player names at line {lineNumber}: Player names cannot be empty", "tournament file", lineNumber);
                }
                
                if (p1.Equals(p2, StringComparison.OrdinalIgnoreCase))
                {
                    throw new FileProcessingException($"Invalid player names at line {lineNumber}: Players cannot have the same name", "tournament file", lineNumber);
                }
                
                player1Name = p1;
                player2Name = p2;
                continue;
            }
            
            // Parse point: "0" or "1"
            if (currentMatchId is not null && player1Name is not null && player2Name is not null)
            {
                if (trimmedLine == "0")
                {
                    points.Add(0);
                }
                else if (trimmedLine == "1")
                {
                    points.Add(1);
                }
                else
                {
                    // Log warning for invalid point values but continue processing as per requirements
                    Console.WriteLine($"Warning: Invalid point value '{trimmedLine}' at line {lineNumber}, skipping");
                }
            }
            else if (currentMatchId is not null)
            {
                // We have a match ID but no player names yet, and this isn't a player line
                if (!trimmedLine.Contains(" vs ", StringComparison.OrdinalIgnoreCase) && 
                    trimmedLine != "0" && trimmedLine != "1")
                {
                    throw new FileProcessingException($"Expected player names after match header at line {lineNumber}", "tournament file", lineNumber);
                }
            }
        }
        
        // Yield final match if we have one
        if (currentMatchId is not null && player1Name is not null && player2Name is not null)
        {
            yield return new RawMatchData
            {
                MatchId = currentMatchId,
                Player1Name = player1Name,
                Player2Name = player2Name,
                Points = points.AsReadOnly()
            };
        }
        else if (currentMatchId is not null)
        {
            throw new FileProcessingException("Incomplete match data: Missing player names for match", "tournament file");
        }
    }
}