using TennisCalculator.Domain;
using TennisCalculator.GamePlay;

namespace TennisCalculator.GamePlay.Tests;

public class StandardSetScorerTests
{
    private readonly StandardSetScorer _scorer;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;
    private readonly TennisSet _initialSet;

    public StandardSetScorerTests()
    {
        _scorer = new StandardSetScorer();
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
        
        _initialSet = new TennisSet
        {
            Players = new[] { _player1, _player2 },
            Games = new List<TennisGame>()
        };
    }

    [Fact]
    public void AddGame_FirstGame_AddsGameToSet()
    {
        // Arrange
        var completedGame = CreateCompletedGame(_player1);

        // Act
        var result = _scorer.AddGame(_initialSet, completedGame);

        // Assert
        Assert.Single(result.Games);
        Assert.Equal(completedGame, result.Games[0]);
        Assert.Null(result.CurrentGame);
        Assert.Null(result.Winner); // Not enough games to win set
    }

    [Fact]
    public void AddGame_SixthGame_WinsSet()
    {
        // Arrange - Player 1 has won 5 games already
        var games = new List<TennisGame>();
        for (int i = 0; i < 5; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }
        
        var setWithFiveGames = _initialSet with { Games = games };
        var sixthGame = CreateCompletedGame(_player1);

        // Act
        var result = _scorer.AddGame(setWithFiveGames, sixthGame);

        // Assert
        Assert.Equal(6, result.Games.Count);
        Assert.Equal(_player1, result.Winner);
        Assert.True(result.HasWinner);
    }

    [Fact]
    public void DetermineSetWinner_LessThanSixGames_ReturnsNull()
    {
        // Arrange - Player 1 has 5 games
        var games = new List<TennisGame>();
        for (int i = 0; i < 5; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }
        
        var set = _initialSet with { Games = games };

        // Act
        var winner = _scorer.DetermineSetWinner(set);

        // Assert
        Assert.Null(winner);
    }

    [Fact]
    public void DetermineSetWinner_SixGames_ReturnsWinner()
    {
        // Arrange - Player 1 has 6 games
        var games = new List<TennisGame>();
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }
        
        var set = _initialSet with { Games = games };

        // Act
        var winner = _scorer.DetermineSetWinner(set);

        // Assert
        Assert.Equal(_player1, winner);
    }

    [Fact]
    public void DetermineSetWinner_SixGamesEach_FirstToSixWins()
    {
        // Arrange - Player 1: 6 games, Player 2: 5 games, then Player 2 gets 6th
        var games = new List<TennisGame>();
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(_player2));
        }
        
        var set = _initialSet with { Games = games };

        // Act
        var winner = _scorer.DetermineSetWinner(set);

        // Assert
        // In simplified rules, first to 6 wins regardless of opponent's score
        Assert.Equal(_player1, winner); // Player 1 reached 6 first
    }

    [Fact]
    public void StartNewGame_CreatesNewGameWithLoveScore()
    {
        // Act
        var result = _scorer.StartNewGame(_initialSet, new[] { _player1, _player2 });

        // Assert
        Assert.NotNull(result.CurrentGame);
        Assert.Equal(_player1, result.CurrentGame.Players[0]);
        Assert.Equal(_player2, result.CurrentGame.Players[1]);
        Assert.Equal(TennisPoint.Love, result.CurrentGame.CurrentScore[_player1]);
        Assert.Equal(TennisPoint.Love, result.CurrentGame.CurrentScore[_player2]);
        Assert.Empty(result.CurrentGame.PointHistory);
        Assert.Null(result.CurrentGame.Winner);
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
            PointHistory = new List<TennisPlayer> { winner, winner, winner, winner }, // 4 points to win
            CurrentScore = initialScore,
            Winner = winner
        };
    }
}