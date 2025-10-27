using System.Runtime.CompilerServices;
using TennisCalculator.Processing.RawData;

namespace TennisCalculator.Processing.Loaders;

/// <summary>
/// Parser for tournament files that processes match data line by line
/// </summary>
internal class TennisDataParser : ITennisDataParser
{
    /// <inheritdoc />
    public async IAsyncEnumerable<RawMatchData> ParseMatchData(
        IAsyncEnumerable<string> lines,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? currentMatchId = null;
        RawMatchData? currentMatch = null;
        int lineNumber = 0;
        
        await foreach (var line in lines.WithCancellation(cancellationToken))
        {
            lineNumber++;
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

            // Parse match header: "Match: <id>"
            if (trimmedLine.StartsWith("Match:", StringComparison.OrdinalIgnoreCase))
            {
                currentMatchId = trimmedLine.Substring(6).Trim();

                // If we have a previous match, yield it first (even if its not complete)
                if (currentMatch is not null)
                {
                    yield return currentMatch;
                    currentMatch = null;
                }

                if (string.IsNullOrWhiteSpace(currentMatchId))
                {
                    throw new TennisDataSourceException($"Invalid match header format at line {lineNumber}: Match ID cannot be empty", lineNumber);
                }
                continue;
            }
            
            // Ensure we have a match ID before processing players or points
            if (currentMatchId is null)
            {
                throw new TennisDataSourceException($"Expected Match Header. Encountered '{trimmedLine}' at line {lineNumber}", lineNumber);
            }

            // Ensure we have match info before processing points
            if (currentMatch is null)
            {
                // Parse player names: "<Player1> vs <Player2>"
                if (trimmedLine.Contains(" vs ", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = trimmedLine.Split(" vs ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length != 2)
                    {
                        throw new TennisDataSourceException($"Invalid player format at line {lineNumber}: Expected '<Player1> vs <Player2>'", lineNumber);
                    }
                    var p1 = parts[0].Trim();
                    var p2 = parts[1].Trim();

                    if (p1.Equals(p2, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new TennisDataSourceException($"Invalid player names at line {lineNumber}: Players cannot have the same name", lineNumber);
                    }
                    currentMatch = new RawMatchData
                    {
                        MatchId = currentMatchId,
                        Player1Name = p1,
                        Player2Name = p2,
                        Points = []
                    };
                    continue;
                }
                throw new TennisDataSourceException($"Expected Player Header. Encountered '{trimmedLine}' at line {lineNumber}", lineNumber);
            }

            if (trimmedLine == "0")
            {
                currentMatch.Points.Add(0);
                continue;
            }
            else if (trimmedLine == "1")
            {
                currentMatch.Points.Add(1);
                continue;
            }
            
            throw new TennisDataSourceException($"Expected Point or New Match. Encountered '{trimmedLine}' at line {lineNumber}", lineNumber);
        }

        if (currentMatchId is not null && currentMatch is null)
        {
            throw new TennisDataSourceException("Incomplete match data: Missing player names for match");
        }

        // Yield final match if we have one
        if (currentMatch is not null)
        {
            yield return currentMatch;
        }
    }
}