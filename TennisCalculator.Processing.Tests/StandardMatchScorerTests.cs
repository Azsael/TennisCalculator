using FluentAssertions;
using TennisCalculator.Processing.Scoring;
using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Tests;

public class StandardMatchScorerTests
{
    private readonly StandardMatchScorer _scorer;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;
    private readonly TennisMatch _initialMatch;

    public StandardMatchScorerTests()
    {
        _scorer = new StandardMatchScorer();
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
        
        _initialMatch = new TennisMatch
        {
            MatchId = "TEST01",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet>()
        };
    }

    [Fact]
    public void AddSet_FirstSet_AddsSetToMatch()
    {
        // Arrange
        var completedSet = CreateCompletedSet(_player1);

        // Act
        var result = _scorer.AddSet(_initialMatch, completedSet);

        // Assert
        result.Sets.Should().ContainSingle();
        result.Sets[0].Should().Be(completedSet);
        result.CurrentSet.Should().BeNull();
        result.Winner.Should().BeNull(); // Not enough sets to win match
    }

    [Fact]
    public void AddSet_SecondSet_WinsMatch()
    {
        // Arrange - Player 1 has won 1 set already
        var firstSet = CreateCompletedSet(_player1);
        var matchWithOneSet = _initialMatch with { Sets = new[] { firstSet } };
        var secondSet = CreateCompletedSet(_player1);

        // Act
        var result = _scorer.AddSet(matchWithOneSet, secondSet);

        // Assert
        result.Sets.Should().HaveCount(2);
        result.Winner.Should().Be(_player1);
        result.HasWinner.Should().BeTrue();
    }

    [Fact]
    public void AddSet_AlternatingSets_NoWinnerYet()
    {
        // Arrange - Player 1 wins first set, Player 2 wins second set
        var firstSet = CreateCompletedSet(_player1);
        var matchWithOneSet = _initialMatch with { Sets = new[] { firstSet } };
        var secondSet = CreateCompletedSet(_player2);

        // Act
        var result = _scorer.AddSet(matchWithOneSet, secondSet);

        // Assert
        result.Sets.Should().HaveCount(2);
        result.Winner.Should().BeNull(); // 1-1 in sets, need third set
    }

    [Fact]
    public void AddSet_ThirdSet_WinsMatch()
    {
        // Arrange - Sets are 1-1, Player 1 wins third set
        var sets = new List<TennisSet>
        {
            CreateCompletedSet(_player1),
            CreateCompletedSet(_player2)
        };
        var matchWithTwoSets = _initialMatch with { Sets = sets };
        var thirdSet = CreateCompletedSet(_player1);

        // Act
        var result = _scorer.AddSet(matchWithTwoSets, thirdSet);

        // Assert
        result.Sets.Should().HaveCount(3);
        result.Winner.Should().Be(_player1);
        result.HasWinner.Should().BeTrue();
    }

    [Fact]
    public void DetermineMatchWinner_OnlyOneSet_ReturnsNull()
    {
        // Arrange - Player 1 has won 1 set
        var sets = new List<TennisSet> { CreateCompletedSet(_player1) };
        var match = _initialMatch with { Sets = sets };

        // Act
        var winner = _scorer.DetermineMatchWinner(match);

        // Assert
        winner.Should().BeNull();
    }

    [Fact]
    public void DetermineMatchWinner_TwoSets_ReturnsWinner()
    {
        // Arrange - Player 1 has won 2 sets
        var sets = new List<TennisSet>
        {
            CreateCompletedSet(_player1),
            CreateCompletedSet(_player1)
        };
        var match = _initialMatch with { Sets = sets };

        // Act
        var winner = _scorer.DetermineMatchWinner(match);

        // Assert
        winner.Should().Be(_player1);
    }

    [Fact]
    public void DetermineMatchWinner_BestOfThree_ReturnsCorrectWinner()
    {
        // Arrange - Player 2 wins 2-1 (Player 1, Player 2, Player 2)
        var sets = new List<TennisSet>
        {
            CreateCompletedSet(_player1),
            CreateCompletedSet(_player2),
            CreateCompletedSet(_player2)
        };
        var match = _initialMatch with { Sets = sets };

        // Act
        var winner = _scorer.DetermineMatchWinner(match);

        // Assert
        winner.Should().Be(_player2);
    }

    [Fact]
    public void StartNewSet_CreatesNewSetWithEmptyGames()
    {
        // Act
        var result = _scorer.StartNewSet(_initialMatch, new[] { _player1, _player2 });

        // Assert
        result.CurrentSet.Should().NotBeNull();
        result.CurrentSet!.Players[0].Should().Be(_player1);
        result.CurrentSet.Players[1].Should().Be(_player2);
        result.CurrentSet.Games.Should().BeEmpty();
        result.CurrentSet.Winner.Should().BeNull();
        result.CurrentSet.CurrentGame.Should().BeNull();
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