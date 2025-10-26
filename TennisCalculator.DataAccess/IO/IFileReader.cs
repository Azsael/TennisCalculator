namespace TennisCalculator.DataAccess;

/// <summary>
/// Interface for reading files asynchronously with streaming support
/// </summary>
public interface IFileReader
{
    /// <summary>
    /// Reads lines from a file asynchronously as an enumerable stream
    /// </summary>
    /// <param name="filePath">Path to the file to read</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Async enumerable of lines from the file</returns>
    IAsyncEnumerable<string> ReadLinesAsync(string filePath, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a file exists at the specified path
    /// </summary>
    /// <param name="filePath">Path to check for file existence</param>
    /// <returns>True if file exists, false otherwise</returns>
    Task<bool> FileExistsAsync(string filePath);
}