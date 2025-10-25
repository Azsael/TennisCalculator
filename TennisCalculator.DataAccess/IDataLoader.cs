namespace TennisCalculator.DataAccess;

/// <summary>
/// Interface for loading tournament data from various sources
/// </summary>
public interface IDataLoader
{
    /// <summary>
    /// Loads tournament data from the specified source
    /// </summary>
    /// <param name="source">Source identifier (e.g., file path, URL, etc.)</param>
    /// <returns>Collection of raw match data</returns>
    Task<IEnumerable<RawMatchData>> LoadTournamentDataAsync(string source);
    
    /// <summary>
    /// Determines if this loader can handle the specified source
    /// </summary>
    /// <param name="source">Source identifier to check</param>
    /// <returns>True if this loader can handle the source, false otherwise</returns>
    Task<bool> CanHandleAsync(string source);
}