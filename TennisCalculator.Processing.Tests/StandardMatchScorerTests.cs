using FluentAssertions;
using TennisCalculator.Processing.Scoring;
using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Tests;

public class StandardMatchScorerTests
{
    private readonly StandardMatchScorer _scorer;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;

    public StandardMatchScorerTests()
    {
        _scorer = new StandardMatchScorer();
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
    }

    [Fact]
    public void ConvertSets_TwoSetsForPlayer1_CreatesCompletedMatch()
    {
        // Arrange - Player 1 wins 2 sets
        var sets = new List<TennisSet>
        {
            CreateCompletedSet(_player1),
            CreateCompletedSet(_player1)
        };

        // Act
        var match = _scorer.ConvertSets("TEST01", new[] { _player1, _player2 }, sets);

        // Assert
        match.MatchId.Should().Be("TEST01");
        match.Players.Should().BeEquivalentTo(new[] { _player1, _player2 });
        match.Sets.Should().HaveCount(2);
        match.Winner.Should().Be(_player1);
        match.HasWinner.Should().BeTrue();
    }

    [Fact]
    public void ConvertSets_OnlyOneSet_NoWinner()
    {
        // Arrange - Player 1 wins 1 set
        var sets = new List<TennisSet>
        {
            CreateCompletedSet(_player1)
        };

        // Act
        var match = _scorer.ConvertSets("TEST02", new[] { _player1, _player2 }, sets);

        // Assert
        match.MatchId.Should().Be("TEST02");
        match.Sets.Should().ContainSingle();
        match.Winner.Should().BeNull();
        match.HasWinner.Should().BeFalse();
    }

    [Fact]
    public void ConvertSets_BestOfThreeMatch_Player2Wins()
    {
        // Arrange - Player 2 wins 2-1 (Player 1, Player 2, Player 2)
        var sets = new List<TennisSet>
        {
            CreateCompletedSet(_player1),
            CreateCompletedSet(_player2),
            CreateCompletedSet(_player2)
        };

        // Act
        var match = _scorer.ConvertSets("TEST03", new[] { _player1, _player2 }, sets);

        // Assert
        match.Sets.Should().HaveCount(3);
        match.Winner.Should().Be(_player2);
        match.HasWinner.Should().BeTrue();
    }

    [Fact]
    public void ConvertSets_AlternatingSets_NoWinnerYet()
    {
        // Arrange - 1-1 in sets
        var sets = new List<TennisSet>
        {
            CreateCompletedSet(_player1),
            CreateCompletedSet(_player2)
        };

        // Act
        var match = _scorer.ConvertSets("TEST04", new[] { _player1, _player2 }, sets);

        // Assert
        match.Sets.Should().HaveCount(2);
        match.Winner.Should().BeNull(); // Need 2 sets to win, currently 1-1
        match.HasWinner.Should().BeFalse();
    }

    [Fact]
    public void ConvertSets_EmptySets_NoWinner()
    {
        // Arrange
        var sets = new List<TennisSet>();

        // Act
        var match = _scorer.ConvertSets("TEST05", new[] { _player1, _player2 }, sets);

        // Assert
        match.Sets.Should().BeEmpty();
        match.Winner.Should().BeNull();
        match.HasWinner.Should().BeFalse();
    }

    [Theory]
    [InlineData(2, 0)] // 2-0
    [InlineData(2, 1)] // 2-1
    public void ConvertSets_Player1WinsTwoSets_Player1Wins(int player1Sets, int player2Sets)
    {
        // Arrange
        var sets = new List<TennisSet>();
        for (int i = 0; i < player1Sets; i++)
        {
            sets.Add(CreateCompletedSet(_player1));
        }
        for (int i = 0; i < player2Sets; i++)
        {
            sets.Add(CreateCompletedSet(_player2));
        }

        // Act
        var match = _scorer.ConvertSets("TEST06", new[] { _player1, _player2 }, sets);

        // Assert
        match.Winner.Should().Be(_player1);
        match.HasWinner.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, 2)] // 0-2
    [InlineData(1, 2)] // 1-2
    public void ConvertSets_Player2WinsTwoSets_Player2Wins(int player1Sets, int player2Sets)
    {
        // Arrange
        var sets = new List<TennisSet>();
        for (int i = 0; i < player1Sets; i++)
        {
            sets.Add(CreateCompletedSet(_player1));
        }
        for (int i = 0; i < player2Sets; i++)
        {
            sets.Add(CreateCompletedSet(_player2));
        }

        // Act
        var match = _scorer.ConvertSets("TEST07", new[] { _player1, _player2 }, sets);

        // Assert
        match.Winner.Should().Be(_player2);
        match.HasWinner.Should().BeTrue();
    }

    private TennisSet CreateCompletedSet(TennisPlayer winner)
    {
        // Create 6 games won by the winner
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
            PointHistory = new List<TennisPlayer> { winner, winner, winner, winner }, // 4 points to win
            CurrentScore = initialScore,
            Winner = winner
        };
    }
}