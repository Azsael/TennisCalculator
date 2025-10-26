namespace TennisCalculator.DataAccess.Processor;

/// <summary>
/// Interface for tournament data processing service
/// </summary>
public interface ITournamentDataProcessor
{
    /// <summary>
    /// Processes tournament data from the specified source
    /// </summary>
    /// <param name="source">Source identifier (e.g., file path)</param>
    /// <exception cref="UnsupportedDataSourceException">Thrown when no data loader can handle the source</exception>
    Task ProcessTournamentData(string source, CancellationToken cancellationToken = default);
}