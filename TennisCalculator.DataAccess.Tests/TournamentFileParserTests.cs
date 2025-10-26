using System.Linq;
using TennisCalculator.DataAccess.Data;
using TennisCalculator.DataAccess.Loaders;
using TennisCalculator.DataAccess.RawData;

namespace TennisCalculator.DataAccess.Tests;

public class ITennisDataParserTests
{
    private readonly ITennisDataParser _parser;

    public ITennisDataParserTests()
    {
        _parser = new TennisDataParser();
    }

    [Fact]
    public async Task ParseTournamentFileAsync_ValidSingleMatch_ReturnsMatchData()
    {
        // Arrange
        var lines = new[]
        {
            "Match: 01",
            "Player A vs Player B",
            "0",
            "1",
            "0",
            "1"
        }.ToAsyncEnumerable();

        // Act
        var results = new List<RawMatchData>();
        await foreach (var matchData in _parser.ParseMatchData(lines))
        {
            results.Add(matchData);
        }

        // Assert
        Assert.Single(results);
        var matchResult = results[0];
        Assert.Equal("01", matchResult.MatchId);
        Assert.Equal("Player A", matchResult.Player1Name);
        Assert.Equal("Player B", matchResult.Player2Name);
        Assert.Equal(new[] { 0, 1, 0, 1 }, matchResult.Points);
    }

    [Fact]
    public async Task ParseTournamentFileAsync_MultipleMatches_ReturnsAllMatches()
    {
        // Arrange
        var lines = new[]
        {
            "Match: 01",
            "Player A vs Player B",
            "0",
            "1",
            "",
            "Match: 02",
            "Player C vs Player D",
            "1",
            "0"
        }.ToAsyncEnumerable();

        // Act
        var results = new List<RawMatchData>();
        await foreach (var matchData in _parser.ParseMatchData(lines))
        {
            results.Add(matchData);
        }

        // Assert
        Assert.Equal(2, results.Count);
        
        Assert.Equal("01", results[0].MatchId);
        Assert.Equal("Player A", results[0].Player1Name);
        Assert.Equal("Player B", results[0].Player2Name);
        Assert.Equal(new[] { 0, 1 }, results[0].Points);
        
        Assert.Equal("02", results[1].MatchId);
        Assert.Equal("Player C", results[1].Player1Name);
        Assert.Equal("Player D", results[1].Player2Name);
        Assert.Equal(new[] { 1, 0 }, results[1].Points);
    }

    [Fact]
    public async Task ParseTournamentFileAsync_EmptyMatchId_ThrowsFileProcessingException()
    {
        // Arrange
        var lines = new[]
        {
            "Match: ",
            "Player A vs Player B"
        }.ToAsyncEnumerable();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TennisDataSourceException>(async () =>
        {
            await foreach (var matchData in _parser.ParseMatchData(lines))
            {
                // Consume the enumerable
            }
        });
        
        Assert.Contains("Match ID cannot be empty", exception.Message);
        Assert.Equal(1, exception.LineNumber);
    }

    [Fact]
    public async Task ParseTournamentFileAsync_InvalidPlayerFormat_ThrowsFileProcessingException()
    {
        // Arrange
        var lines = new[]
        {
            "Match: 01",
            "Player A versus Player B"
        }.ToAsyncEnumerable();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TennisDataSourceException>(async () =>
        {
            await foreach (var matchData in _parser.ParseMatchData(lines))
            {
                // Consume the enumerable
            }
        });
        
        Assert.Contains("Expected '<Player1> vs <Player2>'", exception.Message);
        Assert.Equal(2, exception.LineNumber);
    }

    [Fact]
    public async Task ParseTournamentFileAsync_InvalidPointValues_SkipsInvalidPoints()
    {
        // Arrange
        var lines = new[]
        {
            "Match: 01",
            "Player A vs Player B",
            "0",
            "2", // Invalid point value
            "1",
            "invalid", // Invalid point value
            "0"
        }.ToAsyncEnumerable();

        // Act
        var results = new List<RawMatchData>();
        await foreach (var matchData in _parser.ParseMatchData(lines))
        {
            results.Add(matchData);
        }

        // Assert
        Assert.Single(results);
        var matchResult = results[0];
        Assert.Equal(new[] { 0, 1, 0 }, matchResult.Points); // Invalid points should be skipped
    }
}