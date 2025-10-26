using FluentAssertions;
using TennisCalculator.Domain;
using TennisCalculator.Domain.DataAccess;

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
    public void AddMatch_ValidMatch_AddsToRepository()
    {
        // Act
        _repository.AddMatch(_testMatch);

        // Assert
        var retrievedMatch = _repository.GetMatch("01");
        retrievedMatch.Should().NotBeNull();
        retrievedMatch!.MatchId.Should().Be("01");
        retrievedMatch.Players[0].Should().Be(_player1);
        retrievedMatch.Players[1].Should().Be(_player2);
    }

    [Fact]
    public void AddMatch_NullMatch_ThrowsArgumentNullException()
    {
        // Act & Assert
        _repository.Invoking(r => r.AddMatch(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMatch_DuplicateMatchId_OverwritesExisting()
    {
        // Arrange
        _repository.AddMatch(_testMatch);
        
        var updatedMatch = _testMatch with { Winner = _player1 };

        // Act
        _repository.AddMatch(updatedMatch);

        // Assert
        var retrievedMatch = _repository.GetMatch("01");
        retrievedMatch.Should().NotBeNull();
        retrievedMatch!.Winner.Should().Be(_player1);
    }

    [Fact]
    public void GetMatch_ExistingMatchId_ReturnsMatch()
    {
        // Arrange
        _repository.AddMatch(_testMatch);

        // Act
        var result = _repository.GetMatch("01");

        // Assert
        result.Should().NotBeNull();
        result!.MatchId.Should().Be("01");
    }

    [Fact]
    public void GetMatch_NonExistentMatchId_ReturnsNull()
    {
        // Act
        var result = _repository.GetMatch("99");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetMatch_NullMatchId_ThrowsArgumentException()
    {
        // Act & Assert
        _repository.Invoking(r => r.GetMatch(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetMatch_EmptyMatchId_ThrowsArgumentException()
    {
        // Act & Assert
        _repository.Invoking(r => r.GetMatch(""))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetMatch_WhitespaceMatchId_ThrowsArgumentException()
    {
        // Act & Assert
        _repository.Invoking(r => r.GetMatch("   "))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetAllMatches_EmptyRepository_ReturnsEmptyCollection()
    {
        // Act
        var result = _repository.GetAllMatches();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAllMatches_WithMatches_ReturnsAllMatches()
    {
        // Arrange
        var match2 = _testMatch with { MatchId = "02" };
        _repository.AddMatch(_testMatch);
        _repository.AddMatch(match2);

        // Act
        var result = _repository.GetAllMatches().ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.MatchId == "01");
        result.Should().Contain(m => m.MatchId == "02");
    }

    [Fact]
    public void GetMatchesForPlayer_ExistingPlayer_ReturnsPlayerMatches()
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
        var result = _repository.GetMatchesForPlayer("Player 1").ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.MatchId == "01");
        result.Should().Contain(m => m.MatchId == "02");
    }

    [Fact]
    public void GetMatchesForPlayer_CaseInsensitive_ReturnsPlayerMatches()
    {
        // Arrange
        _repository.AddMatch(_testMatch);

        // Act
        var result = _repository.GetMatchesForPlayer("player 1").ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].MatchId.Should().Be("01");
    }

    [Fact]
    public void GetMatchesForPlayer_NonExistentPlayer_ReturnsEmptyCollection()
    {
        // Arrange
        _repository.AddMatch(_testMatch);

        // Act
        var result = _repository.GetMatchesForPlayer("Unknown Player");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetMatchesForPlayer_NullPlayerName_ThrowsArgumentException()
    {
        // Act & Assert
        _repository.Invoking(r => r.GetMatchesForPlayer(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetMatchesForPlayer_EmptyPlayerName_ThrowsArgumentException()
    {
        // Act & Assert
        _repository.Invoking(r => r.GetMatchesForPlayer(""))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetMatchesForPlayer_WhitespacePlayerName_ThrowsArgumentException()
    {
        // Act & Assert
        _repository.Invoking(r => r.GetMatchesForPlayer("   "))
            .Should().Throw<ArgumentException>();
    }
}