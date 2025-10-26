using System.Runtime.CompilerServices;
using TennisCalculator.DataAccess.RawData;
using TennisCalculator.Domain;

namespace TennisCalculator.DataAccess.Loaders;

/// <summary>
/// Data loader implementation for file-based tournament data
/// </summary>
internal class TennisDataFileLoader : ITennisDataLoader
{
    public bool CanHandle(string source) => File.Exists(source);

    /// <inheritdoc />
    public async IAsyncEnumerable<string> LoadTournamentData(string source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!CanHandle(source))
        {
            throw new TennisDataSourceException($"Tournament file '{source}' not found or cannot be accessed", source);
        }

        using var reader = new StreamReader(source);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(line))
            {
                yield return line;
            }
        }
    }
}