using FluentAssertions;
using Moq;
using TennisCalculator.Console.Commands;
using TennisCalculator.Domain;

namespace TennisCalculator.Console.Tests.Commands;

public class ScoreMatchCommandTests
{
    private readonly Mock<ITennisMatchRepository> _mockRepository;
    private readonly ScoreMatchCommand _command;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;

    public ScoreMatchCommandTests()
    {
        _mockRepository = new Mock<ITennisMatchRepository>();
        _command = new ScoreMatchCommand(_mockRepository.Object);
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
    }

    [Theory]
    [InlineData(new[] { "Score", "Match", "01" }, true)]
    [InlineData(new[] { "score", "match", "01" }, true)]
    [InlineData(new[] { "SCORE", "MATCH", "01" }, true)]
    [InlineData(new[] { "Score", "Match" }, false)]
    [InlineData(new[] { "Score", "Match", "01", "extra" }, false)]
    [InlineData(new[] { "Get", "Match", "01" }, false)]
    [InlineData(new[] { "Score", "Player", "01" }, false)]
    [InlineData(new string[0], false)]
    public void CanHandle_VariousInputs_ReturnsExpectedResult(string[] command, bool expected)
    {
        // Act
        var result = _command.CanHandle(command);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task Handle_MatchNotFound_ReturnsNotFoundMessage()
    {
        // Arrange
        var command = new[] { "Score", "Match", "99" };
        _mockRepository.Setup(r => r.GetMatch("99"))
            .ReturnsAsync((TennisMatch?)null);

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("Match '99' was not found");
        _mockRepository.Verify(r => r.GetMatch("99"), Times.Once);
    }

    [Fact]
    public async Task Handle_MatchInProgress_ReturnsInProgressMessage()
    {
        // Arrange
        var command = new[] { "Score", "Match", "01" };
        var match = new TennisMatch
        {
            MatchId = "01",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet>(),
            Winner = null
        };

        _mockRepository.Setup(r => r.GetMatch("01"))
            .ReturnsAsync(match);

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("Match '01' is still in progress. Player 1 vs Player 2");
        _mockRepository.Verify(r => r.GetMatch("01"), Times.Once);
    }

    [Fact]
    public async Task Handle_CompletedMatch_ReturnsWinnerMessage()
    {
        // Arrange
        var command = new[] { "Score", "Match", "01" };
        var set1 = new TennisSet 
        { 
            Players = new[] { _player1, _player2 },
            Games = new List<TennisGame>(),
            Winner = _player1 
        };
        var set2 = new TennisSet 
        { 
            Players = new[] { _player1, _player2 },
            Games = new List<TennisGame>(),
            Winner = _player1 
        };
        var set3 = new TennisSet 
        { 
            Players = new[] { _player1, _player2 },
            Games = new List<TennisGame>(),
            Winner = _player2 
        };
        
        var match = new TennisMatch
        {
            MatchId = "01",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet> { set1, set2, set3 },
            Winner = _player1
        };

        _mockRepository.Setup(r => r.GetMatch("01"))
            .ReturnsAsync(match);

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("Player 1 defeated Player 2, 2 sets to 1");
        _mockRepository.Verify(r => r.GetMatch("01"), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ReturnsErrorMessage()
    {
        // Arrange
        var command = new[] { "Score", "Match", "01" };
        _mockRepository.Setup(r => r.GetMatch("01"))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("Error processing score match query: Database error");
        _mockRepository.Verify(r => r.GetMatch("01"), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenCorrectly()
    {
        // Arrange
        var command = new[] { "Score", "Match", "01" };
        var cancellationToken = new CancellationToken();
        _mockRepository.Setup(r => r.GetMatch("01"))
            .ReturnsAsync((TennisMatch?)null);

        // Act
        await _command.Handle(command, cancellationToken);

        // Assert
        _mockRepository.Verify(r => r.GetMatch("01"), Times.Once);
    }
}