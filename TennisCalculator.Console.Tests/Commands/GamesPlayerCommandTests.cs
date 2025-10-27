using FluentAssertions;
using Moq;
using TennisCalculator.Console.Commands;
using TennisCalculator.Domain;

namespace TennisCalculator.Console.Tests.Commands;

public class GamesPlayerCommandTests
{
    private readonly Mock<ITennisMatchRepository> _mockRepository;
    private readonly GamesPlayerCommand _command;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;

    public GamesPlayerCommandTests()
    {
        _mockRepository = new Mock<ITennisMatchRepository>();
        _command = new GamesPlayerCommand(_mockRepository.Object);
        _player1 = new TennisPlayer { Name = "John Doe" };
        _player2 = new TennisPlayer { Name = "Jane Smith" };
    }

    private static TennisGame CreateGame(TennisPlayer player1, TennisPlayer player2, TennisPlayer? winner = null)
    {
        return new TennisGame
        {
            Players = new[] { player1, player2 },
            PointHistory = new List<TennisPlayer>(),
            CurrentScore = new Dictionary<TennisPlayer, TennisPoint>(),
            Winner = winner
        };
    }

    private static TennisSet CreateSet(TennisPlayer player1, TennisPlayer player2, IReadOnlyList<TennisGame> games)
    {
        return new TennisSet
        {
            Players = new[] { player1, player2 },
            Games = games
        };
    }

    [Theory]
    [InlineData(new[] { "Games", "Player", "John" }, true)]
    [InlineData(new[] { "games", "player", "john" }, true)]
    [InlineData(new[] { "GAMES", "PLAYER", "JOHN" }, true)]
    [InlineData(new[] { "Games", "Player", "John", "Doe" }, true)]
    [InlineData(new[] { "Games", "Player" }, false)]
    [InlineData(new[] { "Games" }, false)]
    [InlineData(new[] { "Score", "Player", "John" }, false)]
    [InlineData(new[] { "Games", "Match", "John" }, false)]
    [InlineData(new string[0], false)]
    public void CanHandle_VariousInputs_ReturnsExpectedResult(string[] command, bool expected)
    {
        // Act
        var result = _command.CanHandle(command);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task Handle_PlayerNotFound_ReturnsZeroZero()
    {
        // Arrange
        var command = new[] { "Games", "Player", "Unknown" };
        _mockRepository.Setup(r => r.GetMatchesForPlayer("Unknown"))
            .ReturnsAsync(new List<TennisMatch>());

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("0 0");
        _mockRepository.Verify(r => r.GetMatchesForPlayer("Unknown"), Times.Once);
    }

    [Fact]
    public async Task Handle_PlayerWithMultipleNameParts_JoinsNameCorrectly()
    {
        // Arrange
        var command = new[] { "Games", "Player", "John", "Doe" };
        _mockRepository.Setup(r => r.GetMatchesForPlayer("John Doe"))
            .ReturnsAsync(new List<TennisMatch>());

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("0 0");
        _mockRepository.Verify(r => r.GetMatchesForPlayer("John Doe"), Times.Once);
    }

    [Fact]
    public async Task Handle_PlayerWithGames_ReturnsCorrectStats()
    {
        // Arrange
        var command = new[] { "Games", "Player", "John", "Doe" };
        
        var game1 = CreateGame(_player1, _player2, _player1); // John wins
        var game2 = CreateGame(_player1, _player2, _player2); // Jane wins
        var game3 = CreateGame(_player1, _player2, _player1); // John wins
        var game4 = CreateGame(_player1, _player2, _player1); // John wins
        
        var set1 = CreateSet(_player1, _player2, new[] { game1, game2 });
        var set2 = CreateSet(_player1, _player2, new[] { game3, game4 });
        
        var match = new TennisMatch
        {
            MatchId = "01",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet> { set1, set2 }
        };

        _mockRepository.Setup(r => r.GetMatchesForPlayer("John Doe"))
            .ReturnsAsync(new List<TennisMatch> { match });

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("3 1"); // John won 3 games, lost 1 game
        _mockRepository.Verify(r => r.GetMatchesForPlayer("John Doe"), Times.Once);
    }

    [Fact]
    public async Task Handle_PlayerWithMultipleMatches_AggregatesStats()
    {
        // Arrange
        var command = new[] { "Games", "Player", "John", "Doe" };
        
        // Match 1: John wins 2, loses 1
        var match1Game1 = CreateGame(_player1, _player2, _player1);
        var match1Game2 = CreateGame(_player1, _player2, _player2);
        var match1Game3 = CreateGame(_player1, _player2, _player1);
        var match1Set = CreateSet(_player1, _player2, new[] { match1Game1, match1Game2, match1Game3 });
        var match1 = new TennisMatch
        {
            MatchId = "01",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet> { match1Set }
        };

        // Match 2: John wins 1, loses 2
        var match2Game1 = CreateGame(_player1, _player2, _player2);
        var match2Game2 = CreateGame(_player1, _player2, _player2);
        var match2Game3 = CreateGame(_player1, _player2, _player1);
        var match2Set = CreateSet(_player1, _player2, new[] { match2Game1, match2Game2, match2Game3 });
        var match2 = new TennisMatch
        {
            MatchId = "02",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet> { match2Set }
        };

        _mockRepository.Setup(r => r.GetMatchesForPlayer("John Doe"))
            .ReturnsAsync(new List<TennisMatch> { match1, match2 });

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("3 3"); // Total: John won 3, lost 3
        _mockRepository.Verify(r => r.GetMatchesForPlayer("John Doe"), Times.Once);
    }

    [Fact]
    public async Task Handle_GamesWithoutWinner_IgnoresIncompleteGames()
    {
        // Arrange
        var command = new[] { "Games", "Player", "John", "Doe" };
        
        var completeGame = CreateGame(_player1, _player2, _player1);
        var incompleteGame = CreateGame(_player1, _player2, null); // No winner
        
        var set = CreateSet(_player1, _player2, new[] { completeGame, incompleteGame });
        
        var match = new TennisMatch
        {
            MatchId = "01",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet> { set }
        };

        _mockRepository.Setup(r => r.GetMatchesForPlayer("John Doe"))
            .ReturnsAsync(new List<TennisMatch> { match });

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("1 0"); // Only complete game counted
        _mockRepository.Verify(r => r.GetMatchesForPlayer("John Doe"), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ReturnsErrorMessage()
    {
        // Arrange
        var command = new[] { "Games", "Player", "John", "Doe" };
        _mockRepository.Setup(r => r.GetMatchesForPlayer("John Doe"))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _command.Handle(command);

        // Assert
        result.Should().Be("Error processing games player query: Database error");
        _mockRepository.Verify(r => r.GetMatchesForPlayer("John Doe"), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenCorrectly()
    {
        // Arrange
        var command = new[] { "Games", "Player", "John", "Doe" };
        var cancellationToken = new CancellationToken();
        _mockRepository.Setup(r => r.GetMatchesForPlayer("John Doe"))
            .ReturnsAsync(new List<TennisMatch>());

        // Act
        await _command.Handle(command, cancellationToken);

        // Assert
        _mockRepository.Verify(r => r.GetMatchesForPlayer("John Doe"), Times.Once);
    }
}