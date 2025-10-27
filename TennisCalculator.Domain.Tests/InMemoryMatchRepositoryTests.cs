using FluentAssertions;

namespace TennisCalculator.Domain.Tests;

public class InMemoryMatchRepositoryTests
{
    private readonly TennisMatchMemoryBank _repository;
    private readonly TennisPlayer _player1;
    private readonly TennisPlayer _player2;
    private readonly TennisMatch _testMatch;

    public InMemoryMatchRepositoryTests()
    {
        _repository = new TennisMatchMemoryBank();
        _player1 = new TennisPlayer { Name = "Player 1" };
        _player2 = new TennisPlayer { Name = "Player 2" };
        
        _testMatch = new TennisMatch
        {
            MatchId = "01",
            Players = new[] { _player1, _player2 },
            Sets = new List<TennisSet>()
        };
    }

    [Fact]
    public async Task AddMatch_ValidMatch_AddsToRepository()
    {
        // Act
        _repository.AddMatch(_testMatch);

        // Assert
        var retrievedMatch = await _repository.GetMatch("01");
        retrievedMatch.Should().NotBeNull();
        retrievedMatch!.MatchId.Should().Be("01");
        retrievedMatch.Players[0].Should().Be(_player1);
        retrievedMatch.Players[1].Should().Be(_player2);
    }

    [Fact]
    public async Task AddMatch_DuplicateMatchId_OverwritesExisting()
    {
        // Arrange
        _repository.AddMatch(_testMatch);
        
        var updatedMatch = _testMatch with { Winner = _player1 };

        // Act
        _repository.AddMatch(updatedMatch);

        // Assert
        var retrievedMatch = await _repository.GetMatch("01");
        retrievedMatch.Should().NotBeNull();
        retrievedMatch!.Winner.Should().Be(_player1);
    }

    [Fact]
    public async Task GetMatch_ExistingMatchId_ReturnsMatch()
    {
        // Arrange
        _repository.AddMatch(_testMatch);

        // Act
        var result = await _repository.GetMatch("01");

        // Assert
        result.Should().NotBeNull();
        result!.MatchId.Should().Be("01");
    }

    [Theory]
    [InlineData("99")]
    [InlineData("-1")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetMatch_NonExistentMatchId_ReturnsNull(string matchId)
    {
        // Act
        var result = await _repository.GetMatch(matchId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetMatchesForPlayer_ExistingPlayer_ReturnsPlayerMatches()
    {
        // Arrange
        var player3 = new TennisPlayer { Name = "Player 3" };
        var match2 = new TennisMatch
        {
            MatchId = "02",
            Players = new[] { _player1, player3 },
            Sets = new List<TennisSet>()
        };
        var match3 = new TennisMatch
        {
            MatchId = "03",
            Players = new[] { _player2, player3 },
            Sets = new List<TennisSet>()
        };

        _repository.AddMatch(_testMatch); // Player 1 vs Player 2
        _repository.AddMatch(match2);     // Player 1 vs Player 3
        _repository.AddMatch(match3);     // Player 2 vs Player 3

        // Act
        var result = (await _repository.GetMatchesForPlayer("Player 1")).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.MatchId == "01");
        result.Should().Contain(m => m.MatchId == "02");
    }

    [Fact]
    public async Task GetMatchesForPlayer_CaseInsensitive_ReturnsPlayerMatches()
    {
        // Arrange
        _repository.AddMatch(_testMatch);

        // Act
        var result = (await _repository.GetMatchesForPlayer("player 1")).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].MatchId.Should().Be("01");
    }

    [Theory]
    [InlineData("Unknown Player")]
    [InlineData("Different Unknown Player")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetMatchesForPlayer_NonExistentPlayer_ReturnsEmptyCollection(string playerName)
    {
        // Arrange
        _repository.AddMatch(_testMatch);

        // Act
        var result = await _repository.GetMatchesForPlayer(playerName);

        // Assert
        result.Should().BeEmpty();
    }
}