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
        Assert.True(player1.Equals(player2));
        Assert.Equal(player1, player2);
    }

    [Fact]
    public void Equals_CaseInsensitive_ReturnsTrue()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John Doe" };
        var player2 = new TennisPlayer { Name = "john doe" };

        // Act & Assert
        Assert.True(player1.Equals(player2));
        Assert.Equal(player1, player2);
    }

    [Fact]
    public void Equals_MixedCase_ReturnsTrue()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John DOE" };
        var player2 = new TennisPlayer { Name = "john doe" };

        // Act & Assert
        Assert.True(player1.Equals(player2));
        Assert.Equal(player1, player2);
    }

    [Fact]
    public void Equals_DifferentNames_ReturnsFalse()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John Doe" };
        var player2 = new TennisPlayer { Name = "Jane Smith" };

        // Act & Assert
        Assert.False(player1.Equals(player2));
        Assert.NotEqual(player1, player2);
    }

    [Fact]
    public void Equals_NullPlayer_ReturnsFalse()
    {
        // Arrange
        var player = new TennisPlayer { Name = "John Doe" };

        // Act & Assert
        Assert.False(player.Equals(null));
    }

    [Fact]
    public void GetHashCode_CaseInsensitive_SameHashCode()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John Doe" };
        var player2 = new TennisPlayer { Name = "john doe" };

        // Act & Assert
        Assert.Equal(player1.GetHashCode(), player2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentNames_DifferentHashCodes()
    {
        // Arrange
        var player1 = new TennisPlayer { Name = "John Doe" };
        var player2 = new TennisPlayer { Name = "Jane Smith" };

        // Act & Assert
        Assert.NotEqual(player1.GetHashCode(), player2.GetHashCode());
    }
}