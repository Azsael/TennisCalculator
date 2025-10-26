using System.Runtime.CompilerServices;

namespace TennisCalculator.DataAccess.IO;

/// <summary>
/// File reader implementation that provides streaming file access
/// </summary>
public class FileReader : IFileReader
{
    /// <inheritdoc />
    public async IAsyncEnumerable<string> ReadLinesAsync(
        string filePath, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(filePath);
        
        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is not null)
            {
                yield return line;
            }
        }
    }
    
    /// <inheritdoc />
    public Task<bool> FileExistsAsync(string filePath)
    {
        return Task.FromResult(File.Exists(filePath));
    }
}