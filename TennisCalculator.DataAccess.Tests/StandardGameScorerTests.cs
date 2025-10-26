using TennisCalculator.DataAccess.Scoring;
using TennisCalculator.Domain;

namespace TennisCalculator.GamePlay.Tests;

public class StandardGameScorerTests
{
    private readonly StandardGameScorer _scorer;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;
    private readonly TennisGame _initialGame;

    public StandardGameScorerTests()
    {
        _scorer = new StandardGameScorer();
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
        
        var initialScore = new Dictionary<TennisPlayer, TennisPoint>
        {
            { _player1, TennisPoint.Love },
            { _player2, TennisPoint.Love }
        };
        
        _initialGame = new TennisGame
        {
            Players = new[] { _player1, _player2 },
            PointHistory = new List<TennisPlayer>(),
            CurrentScore = initialScore
        };
    }

    [Fact]
    public void AddPoint_FirstPoint_UpdatesScoreToFifteen()
    {
        // Act
        var result = _scorer.AddPoint(_initialGame, _player1);

        // Assert
        Assert.Equal(TennisPoint.Fifteen, result.CurrentScore[_player1]);
        Assert.Equal(TennisPoint.Love, result.CurrentScore[_player2]);
        Assert.Single(result.PointHistory);
        Assert.Equal(_player1, result.PointHistory[0]);
        Assert.Null(result.Winner);
    }

    [Fact]
    public void AddPoint_FourPointsWithTwoPointLead_WinsGame()
    {
        // Arrange - Player 1 wins 4 points, Player 2 wins 2
        var game = _initialGame;
        game = _scorer.AddPoint(game, _player1); // 1-0
        game = _scorer.AddPoint(game, _player1); // 2-0
        game = _scorer.AddPoint(game, _player2); // 2-1
        game = _scorer.AddPoint(game, _player2); // 2-2
        game = _scorer.AddPoint(game, _player1); // 3-2
        
        // Act
        var result = _scorer.AddPoint(game, _player1); // 4-2

        // Assert
        Assert.Equal(_player1, result.Winner);
        Assert.True(result.HasWinner);
    }

    [Fact]
    public void AddPoint_DeuceScenario_ShowsFortyForty()
    {
        // Arrange - Both players get 3 points
        var game = _initialGame;
        game = _scorer.AddPoint(game, _player1); // 1-0
        game = _scorer.AddPoint(game, _player1); // 2-0
        game = _scorer.AddPoint(game, _player1); // 3-0
        game = _scorer.AddPoint(game, _player2); // 3-1
        game = _scorer.AddPoint(game, _player2); // 3-2
        
        // Act
        var result = _scorer.AddPoint(game, _player2); // 3-3 (deuce)

        // Assert
        Assert.Equal(TennisPoint.Forty, result.CurrentScore[_player1]);
        Assert.Equal(TennisPoint.Forty, result.CurrentScore[_player2]);
        Assert.Null(result.Winner);
    }

    [Fact]
    public void AddPoint_AdvantageScenario_ShowsAdvantage()
    {
        // Arrange - Deuce situation (3-3)
        var game = _initialGame;
        for (int i = 0; i < 3; i++)
        {
            game = _scorer.AddPoint(game, _player1);
            game = _scorer.AddPoint(game, _player2);
        }
        
        // Act - Player 1 gets advantage
        var result = _scorer.AddPoint(game, _player1); // 4-3

        // Assert
        Assert.Equal(TennisPoint.Advantage, result.CurrentScore[_player1]);
        Assert.Equal(TennisPoint.Forty, result.CurrentScore[_player2]);
        Assert.Null(result.Winner);
    }

    [Fact]
    public void AddPoint_AdvantageToWin_WinsGame()
    {
        // Arrange - Player 1 has advantage (4-3)
        var game = _initialGame;
        for (int i = 0; i < 3; i++)
        {
            game = _scorer.AddPoint(game, _player1);
            game = _scorer.AddPoint(game, _player2);
        }
        game = _scorer.AddPoint(game, _player1); // Advantage
        
        // Act - Player 1 wins from advantage
        var result = _scorer.AddPoint(game, _player1); // 5-3

        // Assert
        Assert.Equal(_player1, result.Winner);
        Assert.True(result.HasWinner);
    }

    [Theory]
    [InlineData(0, TennisPoint.Love)]
    [InlineData(1, TennisPoint.Fifteen)]
    [InlineData(2, TennisPoint.Thirty)]
    [InlineData(3, TennisPoint.Forty)]
    [InlineData(4, TennisPoint.Forty)]
    public void CalculateCurrentScore_StandardScoring_ReturnsCorrectPoints(int points, TennisPoint expected)
    {
        // Arrange
        var pointHistory = new List<TennisPlayer>();
        for (int i = 0; i < points; i++)
        {
            pointHistory.Add(_player1);
        }

        // Act
        var result = _scorer.CalculateCurrentScore(pointHistory, new[] { _player1, _player2 });

        // Assert
        Assert.Equal(expected, result[_player1]);
        Assert.Equal(TennisPoint.Love, result[_player2]);
    }

    [Fact]
    public void DetermineGameWinner_InsufficientPoints_ReturnsNull()
    {
        // Arrange - Player 1 has 3 points, Player 2 has 2
        var game = _initialGame;
        game = _scorer.AddPoint(game, _player1);
        game = _scorer.AddPoint(game, _player1);
        game = _scorer.AddPoint(game, _player1);
        game = _scorer.AddPoint(game, _player2);
        game = _scorer.AddPoint(game, _player2);

        // Act
        var winner = _scorer.DetermineGameWinner(game);

        // Assert
        Assert.Null(winner);
    }

    [Fact]
    public void DetermineGameWinner_FourPointsWithTwoPointLead_ReturnsWinner()
    {
        // Arrange - Player 1: 4 points, Player 2: 2 points
        var pointHistory = new List<TennisPlayer>();
        for (int i = 0; i < 4; i++) pointHistory.Add(_player1);
        for (int i = 0; i < 2; i++) pointHistory.Add(_player2);
        
        var game = _initialGame with { PointHistory = pointHistory };

        // Act
        var winner = _scorer.DetermineGameWinner(game);

        // Assert
        Assert.Equal(_player1, winner);
    }
}