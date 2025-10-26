namespace TennisCalculator.DataAccess.Loaders;

/// <summary>
/// Interface for loading tournament data from various sources
/// </summary>
internal interface ITennisDataLoader
{
    /// <summary>
    /// Determines if this loader can handle the specified source
    /// </summary>
    /// <param name="source">Source identifier to check</param>
    /// <returns>True if this loader can handle the source, false otherwise</returns>
    bool CanHandle(string source);

    /// <summary>
    /// Loads tournament data from the specified source
    /// </summary>
    /// <param name="source">Source identifier (e.g., file path, URL, etc.)</param>
    /// <returns>Collection of raw match data</returns>
    IAsyncEnumerable<string> LoadTournamentData(string source, CancellationToken cancellationToken = default);
}
