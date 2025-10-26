using TennisCalculator.Processing.Data;
using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Loaders;

internal class TennisMatchDataLoader(
    IEnumerable<ITennisDataLoader> loaders,
    ITennisDataParser parser
) : ITennisMatchDataLoader
{
    /// <inheritdoc />
    public IAsyncEnumerable<RawMatchData> LoadTournamentData(string dataSource, CancellationToken cancellationToken = default)
    {
        var loader = loaders.FirstOrDefault(x => x.CanHandle(dataSource));

        if (loader is null)
            throw new UnsupportedDataSourceException(dataSource);

        var data = loader.LoadTournamentData(dataSource, cancellationToken);

        return parser.ParseMatchData(data, cancellationToken);
    }
}