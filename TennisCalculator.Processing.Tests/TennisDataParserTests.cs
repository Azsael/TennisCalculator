using System.Linq;
using FluentAssertions;
using TennisCalculator.Processing.Loaders;
using TennisCalculator.Processing.RawData;

namespace TennisCalculator.Processing.Tests;

public class TennisDataParserTests
{
    private readonly TennisDataParser _parser;

    public TennisDataParserTests()
    {
        _parser = new TennisDataParser();
    }

    [Fact]
    public async Task ParseMatchData_ValidSingleMatch_ReturnsMatchData()
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
        results.Should().ContainSingle();
        var matchResult = results[0];
        matchResult.MatchId.Should().Be("01");
        matchResult.Player1Name.Should().Be("Player A");
        matchResult.Player2Name.Should().Be("Player B");
        matchResult.Points.Should().Equal(new[] { 0, 1, 0, 1 });
    }

    [Fact]
    public async Task ParseMatchData_MultipleMatches_ReturnsAllMatches()
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
        results.Should().HaveCount(2);
        
        results[0].MatchId.Should().Be("01");
        results[0].Player1Name.Should().Be("Player A");
        results[0].Player2Name.Should().Be("Player B");
        results[0].Points.Should().Equal([0, 1]);
        
        results[1].MatchId.Should().Be("02");
        results[1].Player1Name.Should().Be("Player C");
        results[1].Player2Name.Should().Be("Player D");
        results[1].Points.Should().Equal([1, 0]);
    }

    [Fact]
    public async Task ParseMatchData_EmptyMatchId_ThrowsTennisDataSourceException()
    {
        // Arrange
        var lines = new[]
        {
            "Match: ",
            "Player A vs Player B"
        }.ToAsyncEnumerable();

        // Act & Assert
        var act = async () =>
        {
            await foreach (var matchData in _parser.ParseMatchData(lines))
            {
                // Consume the enumerable
            }
        };
        
        var exception = await act.Should().ThrowAsync<TennisDataSourceException>();
        exception.Which.Message.Should().Contain("Match ID cannot be empty");
        exception.Which.LineNumber.Should().Be(1);
    }

    [Fact]
    public async Task ParseMatchData_InvalidPlayerFormat_ThrowsTennisDataSourceException()
    {
        // Arrange
        var lines = new[]
        {
            "Match: 01",
            "Player A versus Player B"
        }.ToAsyncEnumerable();

        // Act & Assert
        var act = async () =>
        {
            await foreach (var matchData in _parser.ParseMatchData(lines))
            {
                // Consume the enumerable
            }
        };
        
        var exception = await act.Should().ThrowAsync<TennisDataSourceException>();
        exception.Which.Message.Should().Contain("Expected Player Header. Encountered 'Player A versus Player B' at line 2");
        exception.Which.LineNumber.Should().Be(2);
    }

    [Fact]
    public async Task ParseMatchData_InvalidPointValues_ThrowsTennisDataSourceException()
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

        // Act & Assert
        var act = async () =>
        {
            await foreach (var matchData in _parser.ParseMatchData(lines))
            {
                // Consume the enumerable
            }
        };

        var exception = await act.Should().ThrowAsync<TennisDataSourceException>();
        exception.Which.Message.Should().Contain("Expected Point or New Match. Encountered '2' at line 4");
        exception.Which.LineNumber.Should().Be(4);
    }
}