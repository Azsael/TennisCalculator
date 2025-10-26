using TennisCalculator.DataAccess.Data;

namespace TennisCalculator.DataAccess.Loaders;

internal interface ITennisMatchDataLoader
{
    /// <summary>
    /// Loads tournament data from the specified source
    /// </summary>
    /// <param name="dataSource">Data source (e.g., file path, URL, stream of data, etc.)</param>
    /// <returns>Collection of raw match data</returns>
    IAsyncEnumerable<RawMatchData> LoadTournamentData(string dataSource, CancellationToken cancellationToken = default);
}
