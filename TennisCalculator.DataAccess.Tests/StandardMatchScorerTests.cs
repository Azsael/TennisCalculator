using TennisCalculator.DataAccess.Scoring;
using TennisCalculator.Domain;

namespace TennisCalculator.GamePlay.Tests;

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
        Assert.Single(result.Sets);
        Assert.Equal(completedSet, result.Sets[0]);
        Assert.Null(result.CurrentSet);
        Assert.Null(result.Winner); // Not enough sets to win match
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
        Assert.Equal(2, result.Sets.Count);
        Assert.Equal(_player1, result.Winner);
        Assert.True(result.HasWinner);
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
        Assert.Equal(2, result.Sets.Count);
        Assert.Null(result.Winner); // 1-1 in sets, need third set
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
        Assert.Equal(3, result.Sets.Count);
        Assert.Equal(_player1, result.Winner);
        Assert.True(result.HasWinner);
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
        Assert.Null(winner);
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
        Assert.Equal(_player1, winner);
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
        Assert.Equal(_player2, winner);
    }

    [Fact]
    public void StartNewSet_CreatesNewSetWithEmptyGames()
    {
        // Act
        var result = _scorer.StartNewSet(_initialMatch, new[] { _player1, _player2 });

        // Assert
        Assert.NotNull(result.CurrentSet);
        Assert.Equal(_player1, result.CurrentSet.Players[0]);
        Assert.Equal(_player2, result.CurrentSet.Players[1]);
        Assert.Empty(result.CurrentSet.Games);
        Assert.Null(result.CurrentSet.Winner);
        Assert.Null(result.CurrentSet.CurrentGame);
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