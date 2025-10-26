namespace TennisCalculator.Processing.Loaders;

/// <summary>
/// Data loader implementation for when the source is the data stream
/// </summary>
internal class TennisDataStreamLoader : ITennisDataLoader
{
    public bool CanHandle(string source) => !string.IsNullOrWhiteSpace(source) && source.StartsWith("stream://");

    public IAsyncEnumerable<string> LoadTournamentData(string source, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
