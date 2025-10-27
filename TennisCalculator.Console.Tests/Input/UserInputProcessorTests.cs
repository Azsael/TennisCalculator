using FluentAssertions;
using Moq;
using TennisCalculator.Console.Input;

namespace TennisCalculator.Console.Tests.Input;

public class UserInputProcessorTests
{
    private readonly Mock<IUserInputHandler> _mockInputHandler;
    private readonly UserInputProcessor _processor;

    public UserInputProcessorTests()
    {
        _mockInputHandler = new Mock<IUserInputHandler>();
        _processor = new UserInputProcessor(_mockInputHandler.Object);
    }

    // Note: Testing UserInputProcessor is challenging because it directly uses Console I/O
    // In a real-world scenario, we would refactor to inject an IConsoleWrapper for better testability
    // For now, we'll focus on testing the UserInputHandler which contains the core logic

    [Fact]
    public void Constructor_WithValidHandler_CreatesInstance()
    {
        // Act & Assert
        _processor.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullHandler_ThrowsArgumentNullException()
    {
        // Act & Assert
        var action = () => new UserInputProcessor(null!);
        action.Should().Throw<ArgumentNullException>();
    }


}