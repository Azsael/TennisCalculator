using FluentAssertions;
using TennisCalculator.Domain;

namespace TennisCalculator.Domain.Tests;

public class TennisPlayerTests
{
    [Fact]
    public void Equals_SameName_ReturnsTrue()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John Doe" };
        var player2 = new TennisPlayer { Name = "John Doe" };

        // Act & Assert
        player1.Equals(player2).Should().BeTrue();
        player1.Should().Be(player2);
    }

    [Fact]
    public void Equals_CaseInsensitive_ReturnsTrue()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John Doe" };
        var player2 = new TennisPlayer { Name = "john doe" };

        // Act & Assert
        player1.Equals(player2).Should().BeTrue();
        player1.Should().Be(player2);
    }

    [Fact]
    public void Equals_MixedCase_ReturnsTrue()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John DOE" };
        var player2 = new TennisPlayer { Name = "john doe" };

        // Act & Assert
        player1.Equals(player2).Should().BeTrue();
        player1.Should().Be(player2);
    }

    [Fact]
    public void Equals_DifferentNames_ReturnsFalse()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John Doe" };
        var player2 = new TennisPlayer { Name = "Jane Smith" };

        // Act & Assert
        player1.Equals(player2).Should().BeFalse();
        player1.Should().NotBe(player2);
    }

    [Fact]
    public void Equals_NullPlayer_ReturnsFalse()
    {
        // Arrange
        var player = new TennisPlayer { Name = "John Doe" };

        // Act & Assert
        player.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_CaseInsensitive_SameHashCode()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John Doe" };
        var player2 = new TennisPlayer { Name = "john doe" };

        // Act & Assert
        player1.GetHashCode().Should().Be(player2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentNames_DifferentHashCodes()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John Doe" };
        var player2 = new TennisPlayer { Name = "Jane Smith" };

        // Act & Assert
        player1.GetHashCode().Should().NotBe(player2.GetHashCode());
    }
}