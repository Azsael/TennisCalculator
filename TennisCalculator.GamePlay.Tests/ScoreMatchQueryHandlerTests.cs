using Moq;
using TennisCalculator.Domain;
using TennisCalculator.Domain.DataAccess;
using TennisCalculator.GamePlay;

namespace TennisCalculator.GamePlay.Tests;

public class ScoreMatchQueryHandlerTests
{
    private readonly Mock<ITennisMatchRepository> _mockRepository;
    private readonly ScoreMatchQueryHandler _handler;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;

    public ScoreMatchQueryHandlerTests()
    {
        _mockRepository = new Mock<ITennisMatchRepository>();
        _handler = new ScoreMatchQueryHandler(_mockRepository.Object);
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
    }

    [Fact]
    public void Handle_ValidMatchId_ReturnsFormattedResult()
    {
        // Arrange
        var match = CreateCompletedMatch("01", _player1, 2, 1);
        _mockRepository.Setup(r => r.GetMatch("01")).Returns(match);
        var query = new ScoreMatchQuery { MatchId = "01" };

        // Act
        var result = _handler.Handle(query);

        // Assert
        Assert.Equal("Player 1 defeated Player 2, 2 sets to 1", result);
    }

    [Fact]
    public void Handle_NonExistentMatchId_ReturnsErrorMessage()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetMatch("99")).Returns((TennisMatch?)null);
        var query = new ScoreMatchQuery { MatchId = "99" };

        // Act
        var result = _handler.Handle(query);

        // Assert
        Assert.Equal("Error: Match '99' not found", result);
    }

    [Fact]
    public void Handle_IncompleteMatch_ReturnsErrorMessage()
    {
        // Arrange
        var incompleteMatch = new TennisMatch
        {
            MatchId = "01",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet>(),
            Winner = null
        };
        _mockRepository.Setup(r => r.GetMatch("01")).Returns(incompleteMatch);
        var query = new ScoreMatchQuery { MatchId = "01" };

        // Act
        var result = _handler.Handle(query);

        // Assert
        Assert.Equal("Error: Match '01' is not completed", result);
    }

    [Fact]
    public void Handle_EmptyMatchId_ThrowsQueryProcessingException()
    {
        // Arrange
        var query = new ScoreMatchQuery { MatchId = "" };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _handler.Handle(query));
        Assert.Contains("Match ID cannot be null or empty", exception.Message);
    }

    [Fact]
    public void Handle_NullMatchId_ThrowsQueryProcessingException()
    {
        // Arrange
        var query = new ScoreMatchQuery { MatchId = null! };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _handler.Handle(query));
        Assert.Contains("Match ID cannot be null or empty", exception.Message);
    }

    [Fact]
    public void Handle_Player2Wins_ReturnsCorrectFormat()
    {
        // Arrange
        var match = CreateCompletedMatch("02", _player2, 2, 0);
        _mockRepository.Setup(r => r.GetMatch("02")).Returns(match);
        var query = new ScoreMatchQuery { MatchId = "02" };

        // Act
        var result = _handler.Handle(query);

        // Assert
        Assert.Equal("Player 2 defeated Player 1, 2 sets to 0", result);
    }

    [Fact]
    public void Constructor_NullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ScoreMatchQueryHandler(null!));
    }

    private TennisMatch CreateCompletedMatch(string matchId, TennisPlayer winner, int winnerSets, int loserSets)
    {
        var sets = new List<TennisSet>();
        
        // Add sets won by winner
        for (int i = 0; i < winnerSets; i++)
        {
            sets.Add(CreateCompletedSet(winner));
        }
        
        // Add sets won by loser
        var loser = winner.Equals(_player1) ? _player2 : _player1;
        for (int i = 0; i < loserSets; i++)
        {
            sets.Add(CreateCompletedSet(loser));
        }

        return new TennisMatch
        {
            MatchId = matchId,
            Players = new[] { _player1, _player2 },
            Sets = sets,
            Winner = winner
        };
    }

    private TennisSet CreateCompletedSet(TennisPlayer winner)
    {
        var games = new List<TennisGame>();
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(winner));
        }

        return new TennisSet
        {
            Players = new[] { _player1, _player2 },
            Games = games,
            Winner = winner
        };
    }

    private TennisGame CreateCompletedGame(TennisPlayer winner)
    {
        var initialScore = new Dictionary<TennisPlayer, TennisPoint>
        {
            { _player1, TennisPoint.Love },
            { _player2, TennisPoint.Love }
        };

        return new TennisGame
        {
            Players = new[] { _player1, _player2 },
            PointHistory = new List<TennisPlayer> { winner, winner, winner, winner },
            CurrentScore = initialScore,
            Winner = winner
        };
    }
}