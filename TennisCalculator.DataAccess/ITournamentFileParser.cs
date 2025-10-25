namespace TennisCalculator.DataAccess;

/// <summary>
/// Interface for parsing tournament files into raw match data
/// </summary>
public interface ITournamentFileParser
{
    /// <summary>
    /// Parses tournament file lines into raw match data asynchronously
    /// </summary>
    /// <param name="lines">Async enumerable of lines from the tournament file</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Async enumerable of parsed raw match data</returns>
    IAsyncEnumerable<RawMatchData> ParseTournamentFileAsync(
        IAsyncEnumerable<string> lines, 
        CancellationToken cancellationToken = default);
}