using FluentAssertions;
using TennisCalculator.Processing.Scoring;
using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Tests;

public class StandardSetScorerTests
{
    private readonly StandardSetScorer _scorer;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;

    public StandardSetScorerTests()
    {
        _scorer = new StandardSetScorer();
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
    }

    [Fact]
    public void ConvertGames_SixGamesForPlayer1_CreatesCompletedSet()
    {
        // Arrange - Player 1 wins 6 games
        var games = new List<TennisGame>();
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }

        // Act
        var sets = _scorer.ConvertGames(games).ToList();

        // Assert
        sets.Should().ContainSingle();
        var set = sets[0];
        set.Winner.Should().Be(_player1);
        set.HasWinner.Should().BeTrue();
        set.Games.Should().HaveCount(6);
        set.Games.Should().AllSatisfy(g => g.Winner.Should().Be(_player1));
    }

    [Fact]
    public void ConvertGames_IncompleteSet_CreatesUnfinishedSet()
    {
        // Arrange - Player 1 wins 4 games, Player 2 wins 2 games
        var games = new List<TennisGame>();
        for (int i = 0; i < 4; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }
        for (int i = 0; i < 2; i++)
        {
            games.Add(CreateCompletedGame(_player2));
        }

        // Act
        var sets = _scorer.ConvertGames(games).ToList();

        // Assert
        sets.Should().ContainSingle();
        var set = sets[0];
        set.Winner.Should().BeNull();
        set.HasWinner.Should().BeFalse();
        set.Games.Should().HaveCount(6);
    }

    [Fact]
    public void ConvertGames_MultipleSets_CreatesMultipleSets()
    {
        // Arrange - Player 1 wins first set 6-0, Player 2 wins second set 6-1
        var games = new List<TennisGame>();
        
        // First set: Player 1 wins 6 games
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }
        
        // Second set: Player 2 wins 6 games, Player 1 wins 1
        games.Add(CreateCompletedGame(_player1));
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(_player2));
        }

        // Act
        var sets = _scorer.ConvertGames(games).ToList();

        // Assert
        sets.Should().HaveCount(2);
        sets[0].Winner.Should().Be(_player1);
        sets[0].Games.Should().HaveCount(6);
        sets[1].Winner.Should().Be(_player2);
        sets[1].Games.Should().HaveCount(7);
    }

    [Fact]
    public void ConvertGames_EmptyGames_ReturnsNoSets()
    {
        // Arrange
        var games = new List<TennisGame>();

        // Act
        var sets = _scorer.ConvertGames(games).ToList();

        // Assert
        sets.Should().BeEmpty();
    }

    [Fact]
    public void ConvertGames_AlternatingWins_FirstToSixWins()
    {
        // Arrange - Alternating wins until one player reaches 6
        var games = new List<TennisGame>();
        
        // Player 1, Player 2, Player 1, Player 2, Player 1, Player 2 (3-3)
        // Then Player 1 wins 3 more to reach 6
        games.Add(CreateCompletedGame(_player1));
        games.Add(CreateCompletedGame(_player2));
        games.Add(CreateCompletedGame(_player1));
        games.Add(CreateCompletedGame(_player2));
        games.Add(CreateCompletedGame(_player1));
        games.Add(CreateCompletedGame(_player2));
        games.Add(CreateCompletedGame(_player1));
        games.Add(CreateCompletedGame(_player1));
        games.Add(CreateCompletedGame(_player1));

        // Act
        var sets = _scorer.ConvertGames(games).ToList();

        // Assert
        sets.Should().ContainSingle();
        var set = sets[0];
        set.Winner.Should().Be(_player1);
        set.Games.Should().HaveCount(9);
    }

    [Fact]
    public void ConvertGames_SixZero_Player1WinsOneSet()
    {
        // Arrange - 6-0
        var games = new List<TennisGame>();
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }

        // Act
        var sets = _scorer.ConvertGames(games).ToList();

        // Assert
        sets.Should().ContainSingle();
        sets[0].Winner.Should().Be(_player1);
        sets[0].Games.Should().HaveCount(6);
    }

    [Fact]
    public void ConvertGames_SixOne_Player1WinsFirstSetThenIncompleteSecondSet()
    {
        // Arrange - 6-1 (Player 1 wins 6, then Player 2 wins 1)
        var games = new List<TennisGame>();
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }
        games.Add(CreateCompletedGame(_player2));

        // Act
        var sets = _scorer.ConvertGames(games).ToList();

        // Assert
        sets.Should().HaveCount(2);
        sets[0].Winner.Should().Be(_player1);
        sets[0].Games.Should().HaveCount(6);
        sets[1].Winner.Should().BeNull(); // Incomplete set
        sets[1].Games.Should().ContainSingle();
    }

    [Fact]
    public void ConvertGames_SixFive_Player1WinsFirstSetThenIncompleteFiveGames()
    {
        // Arrange - 6-5 (Player 1 wins 6, then Player 2 wins 5)
        var games = new List<TennisGame>();
        for (int i = 0; i < 6; i++)
        {
            games.Add(CreateCompletedGame(_player1));
        }
        for (int i = 0; i < 5; i++)
        {
            games.Add(CreateCompletedGame(_player2));
        }

        // Act
        var sets = _scorer.ConvertGames(games).ToList();

        // Assert
        sets.Should().HaveCount(2);
        sets[0].Winner.Should().Be(_player1);
        sets[0].Games.Should().HaveCount(6);
        sets[1].Winner.Should().BeNull(); // Incomplete set
        sets[1].Games.Should().HaveCount(5);
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