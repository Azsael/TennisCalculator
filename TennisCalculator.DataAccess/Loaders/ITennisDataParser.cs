using TennisCalculator.DataAccess.Data;

namespace TennisCalculator.DataAccess.Loaders;

/// <summary>
/// Interface for parsing tournament files into raw match data
/// </summary>
internal interface ITennisDataParser
{
    /// <summary>
    /// Parses tournament file lines into raw match data asynchronously
    /// </summary>
    /// <param name="lines">Async enumerable of lines from the tournament file</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Async enumerable of parsed raw match data</returns>
    IAsyncEnumerable<RawMatchData> ParseMatchData(
        IAsyncEnumerable<string> lines, 
        CancellationToken cancellationToken = default);
}