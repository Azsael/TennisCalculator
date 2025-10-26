using Moq;
using TennisCalculator.DataAccess;
using TennisCalculator.Domain;
using TennisCalculator.GamePlay;

namespace TennisCalculator.GamePlay.Tests;

public class GamesPlayerQueryHandlerTests
{
    private readonly Mock<IMatchRepository> _mockRepository;
    private readonly GamesPlayerQueryHandler _handler;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;

    public GamesPlayerQueryHandlerTests()
    {
        _mockRepository = new Mock<IMatchRepository>();
        _handler = new GamesPlayerQueryHandler(_mockRepository.Object);
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
    }

    [Fact]
    public void Handle_ValidPlayerName_ReturnsFormattedStatistics()
    {
        // Arrange
        var matches = new List<TennisMatch>
        {
            CreateMatchWithGames("01", _player1, 6, 4), // Player 1 wins 6-4 in games
            CreateMatchWithGames("02", _player2, 6, 3)  // Player 2 wins 6-3, so Player 1 loses 3-6
        };
        
        _mockRepository.Setup(r => r.GetMatchesForPlayer("Player 1")).Returns(matches);
        var query = new GamesPlayerQuery { PlayerName = "Player 1" };

        // Act
        var result = _handler.Handle(query);

        // Assert
        Assert.Equal("9 10", result); // 6+3 games won, 4+6 games lost
    }

    [Fact]
    public void Handle_NonExistentPlayer_ReturnsErrorMessage()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetMatchesForPlayer("Unknown Player")).Returns(new List<TennisMatch>());
        var query = new GamesPlayerQuery { PlayerName = "Unknown Player" };

        // Act
        var result = _handler.Handle(query);

        // Assert
        Assert.Equal("Error: Player 'Unknown Player' not found", result);
    }

    [Fact]
    public void Handle_EmptyPlayerName_ThrowsQueryProcessingException()
    {
        // Arrange
        var query = new GamesPlayerQuery { PlayerName = "" };

        // Act & Assert
        var exception = Assert.Throws<QueryProcessingException>(() => _handler.Handle(query));
        Assert.Contains("Player name cannot be null or empty", exception.Message);
    }

    [Fact]
    public void Handle_NullPlayerName_ThrowsQueryProcessingException()
    {
        // Arrange
        var query = new GamesPlayerQuery { PlayerName = null! };

        // Act & Assert
        var exception = Assert.Throws<QueryProcessingException>(() => _handler.Handle(query));
        Assert.Contains("Player name cannot be null or empty", exception.Message);
    }

    [Fact]
    public void Handle_CaseInsensitivePlayerName_ReturnsStatistics()
    {
        // Arrange
        var matches = new List<TennisMatch>
        {
            CreateMatchWithGames("01", _player1, 6, 0)
        };
        
        _mockRepository.Setup(r => r.GetMatchesForPlayer("player 1")).Returns(matches);
        var query = new GamesPlayerQuery { PlayerName = "player 1" };

        // Act
        var result = _handler.Handle(query);

        // Assert
        Assert.Equal("6 0", result);
    }

    [Fact]
    public void Handle_PlayerWithNoGames_ReturnsZeroStatistics()
    {
        // Arrange
        var emptyMatch = new TennisMatch
        {
            MatchId = "01",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet>()
        };
        
        var matches = new List<TennisMatch> { emptyMatch };
        _mockRepository.Setup(r => r.GetMatchesForPlayer("Player 1")).Returns(matches);
        var query = new GamesPlayerQuery { PlayerName = "Player 1" };

        // Act
        var result = _handler.Handle(query);

        // Assert
        Assert.Equal("0 0", result);
    }

    [Fact]
    public void Constructor_NullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GamesPlayerQueryHandler(null!));
    }

    private TennisMatch CreateMatchWithGames(string matchId, TennisPlayer winner, int winnerGames, int loserGames)
    {
        var games = new List<TennisGame>();
        var loser = winner.Equals(_player1) ? _player2 : _player1;
        
        // Add games won by winner
        for (int i = 0; i < winnerGames; i++)
        {
            games.Add(CreateCompletedGame(winner));
        }
        
        // Add games won by loser
        for (int i = 0; i < loserGames; i++)
        {
            games.Add(CreateCompletedGame(loser));
        }

        var set = new TennisSet
        {
            Players = new[] { _player1, _player2 },
            Games = games,
            Winner = winnerGames > loserGames ? winner : loser
        };

        return new TennisMatch
        {
            MatchId = matchId,
            Players = new[] { _player1, _player2 },
            Sets = new[] { set },
            Winner = winnerGames > loserGames ? winner : loser
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