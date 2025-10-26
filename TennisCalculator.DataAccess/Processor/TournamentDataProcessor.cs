using TennisCalculator.DataAccess.Loaders;
using TennisCalculator.DataAccess.RawData;
using TennisCalculator.Domain;
using TennisCalculator.Domain.DataAccess;

namespace TennisCalculator.DataAccess.Processor;

/// <summary>
/// Service that orchestrates data loading and match creation
/// </summary>
internal class TournamentDataProcessor(
    ITennisMatchRepository repository,
    ITennisMatchDataLoader dataLoader,
    IMatchDataProcessor processor
) : ITournamentDataProcessor
{
    /// <summary>
    /// Processes tournament data from the specified source
    /// </summary>
    /// <param name="source">Source identifier (e.g., file path)</param>
    /// <exception cref="UnsupportedDataSourceException">Thrown when no data loader can handle the source</exception>
    public async Task ProcessTournamentData(string source, CancellationToken cancellationToken = default)
    {
        var rawMatches = dataLoader.LoadTournamentData(source, cancellationToken);

        await foreach (var rawMatch in rawMatches)
        {
            var match = processor.CreateMatchFromRawData(rawMatch);
            repository.AddMatch(match);
        }
    }
}
