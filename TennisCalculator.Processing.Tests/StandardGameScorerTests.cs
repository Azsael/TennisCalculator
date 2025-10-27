using FluentAssertions;
using TennisCalculator.Processing.Scoring;
using TennisCalculator.Domain;

namespace TennisCalculator.Processing.Tests;

public class StandardGameScorerTests
{
    private readonly StandardGameScorer _scorer;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;

    public StandardGameScorerTests()
    {
        _scorer = new StandardGameScorer();
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
    }

    [Fact]
    public void ConvertPoints_SimpleGame_CreatesCompletedGame()
    {
        // Arrange - Player 1 wins 4-0
        var points = new List<int> { 0, 0, 0, 0 }; // Player 1 wins all 4 points

        // Act
        var games = _scorer.ConvertPoints(_player1, _player2, points).ToList();

        // Assert
        games.Should().ContainSingle();
        var game = games[0];
        game.Winner.Should().Be(_player1);
        game.HasWinner.Should().BeTrue();
        game.PointHistory.Should().HaveCount(4);
        game.PointHistory.Should().AllBeEquivalentTo(_player1);
        game.CurrentScore[_player1].Should().Be(TennisPoint.Game);
        game.CurrentScore[_player2].Should().Be(TennisPoint.Love);
    }

    [Fact]
    public void ConvertPoints_DeuceGame_CreatesCompletedGame()
    {
        // Arrange - Deuce game: 3-3, then Player 1 wins 2 more
        var points = new List<int> { 0, 1, 0, 1, 0, 1, 0, 0 }; // 3-3 then Player 1 wins

        // Act
        var games = _scorer.ConvertPoints(_player1, _player2, points).ToList();

        // Assert
        games.Should().ContainSingle();
        var game = games[0];
        game.Winner.Should().Be(_player1);
        game.HasWinner.Should().BeTrue();
        game.PointHistory.Should().HaveCount(8);
    }

    [Fact]
    public void ConvertPoints_MultipleGames_CreatesMultipleGames()
    {
        // Arrange - Two complete games: Player 1 wins first 4-0, Player 2 wins second 4-1
        var points = new List<int> { 0, 0, 0, 0, 1, 1, 1, 0, 1 }; // Game 1: 4-0, Game 2: 1-4

        // Act
        var games = _scorer.ConvertPoints(_player1, _player2, points).ToList();

        // Assert
        games.Should().HaveCount(2);
        games[0].Winner.Should().Be(_player1);
        games[1].Winner.Should().Be(_player2);
    }

    [Fact]
    public void ConvertPoints_IncompleteGame_CreatesUnfinishedGame()
    {
        // Arrange - Incomplete game: 2-1
        var points = new List<int> { 0, 0, 1 }; // Player 1: 2 points, Player 2: 1 point

        // Act
        var games = _scorer.ConvertPoints(_player1, _player2, points).ToList();

        // Assert
        games.Should().ContainSingle();
        var game = games[0];
        game.Winner.Should().BeNull();
        game.HasWinner.Should().BeFalse();
        game.CurrentScore[_player1].Should().Be(TennisPoint.Thirty);
        game.CurrentScore[_player2].Should().Be(TennisPoint.Fifteen);
    }

    [Fact]
    public void ConvertPoints_EmptyPoints_ReturnsNoGames()
    {
        // Arrange
        var points = new List<int>();

        // Act
        var games = _scorer.ConvertPoints(_player1, _player2, points).ToList();

        // Assert
        games.Should().BeEmpty();
    }

    [Fact]
    public void ConvertPoints_AdvantageScenario_HandlesCorrectly()
    {
        // Arrange - Deuce, Player 1 advantage, Player 2 equalizes, Player 2 wins
        var points = new List<int> { 0, 1, 0, 1, 0, 1, 0, 1, 1 }; // 3-3, 4-3, 4-4, 4-5

        // Act
        var games = _scorer.ConvertPoints(_player1, _player2, points).ToList();

        // Assert
        games.Should().ContainSingle();
        var game = games[0];
        game.Winner.Should().Be(_player2);
        game.HasWinner.Should().BeTrue();
    }

    [Theory]
    [InlineData(new[] { 0, 0, 0, 0 }, TennisPoint.Game, TennisPoint.Love)] // 4-0
    [InlineData(new[] { 0, 0, 0, 1, 0 }, TennisPoint.Game, TennisPoint.Fifteen)] // 4-1
    [InlineData(new[] { 0, 1, 0 }, TennisPoint.Thirty, TennisPoint.Fifteen)] // 2-1 incomplete
    [InlineData(new[] { 0, 1, 0, 1, 0, 1 }, TennisPoint.Forty, TennisPoint.Forty)] // 3-3 deuce
    public void ConvertPoints_VariousScenarios_ProducesCorrectScores(int[] pointArray, TennisPoint expectedP1, TennisPoint expectedP2)
    {
        // Arrange
        var points = pointArray.ToList();

        // Act
        var games = _scorer.ConvertPoints(_player1, _player2, points).ToList();

        // Assert
        games.Should().ContainSingle();
        var game = games[0];
        game.CurrentScore[_player1].Should().Be(expectedP1);
        game.CurrentScore[_player2].Should().Be(expectedP2);
    }
}