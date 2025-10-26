using TennisCalculator.DataAccess.IO;
using TennisCalculator.Domain;

namespace TennisCalculator.DataAccess.IO;

/// <summary>
/// Data loader implementation for file-based tournament data
/// </summary>
public class FileDataLoader : IDataLoader
{
    private readonly IFileReader _fileReader;
    private readonly ITournamentFileParser _parser;
    
    /// <summary>
    /// Initializes a new instance of FileDataLoader
    /// </summary>
    /// <param name="fileReader">File reader for accessing files</param>
    /// <param name="parser">Parser for processing tournament file content</param>
    public FileDataLoader(IFileReader fileReader, ITournamentFileParser parser)
    {
        _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<RawMatchData>> LoadTournamentDataAsync(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new FileProcessingException("File path cannot be null or empty", source ?? string.Empty);
        }

        if (!await CanHandleAsync(source))
        {
            throw new FileProcessingException($"Tournament file '{source}' not found or cannot be accessed", source);
        }
        
        try
        {
            var lines = _fileReader.ReadLinesAsync(source);
            var matches = new List<RawMatchData>();
            
            await foreach (var match in _parser.ParseTournamentFileAsync(lines))
            {
                matches.Add(match);
            }
            
            return matches;
        }
        catch (FileProcessingException)
        {
            // Re-throw file processing exceptions as-is
            throw;
        }
        catch (Exception ex)
        {
            throw new FileProcessingException($"Error processing tournament file '{source}': {ex.Message}", source, ex);
        }
    }
    
    /// <inheritdoc />
    public async Task<bool> CanHandleAsync(string source)
    {
        return await _fileReader.FileExistsAsync(source);
    }
}