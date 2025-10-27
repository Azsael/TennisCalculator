using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TennisCalculator.Console.Commands;
using TennisCalculator.Console.Input;

namespace TennisCalculator.Console.Tests.Input;

public class UserInputHandlerTests
{
    private readonly Mock<ILogger<UserInputHandler>> _mockLogger;
    private readonly Mock<IUserCommand> _mockCommand1;
    private readonly Mock<IUserCommand> _mockCommand2;
    private readonly UserInputHandler _handler;

    public UserInputHandlerTests()
    {
        _mockLogger = new Mock<ILogger<UserInputHandler>>();
        _mockCommand1 = new Mock<IUserCommand>();
        _mockCommand2 = new Mock<IUserCommand>();
        
        var commands = new[] { _mockCommand1.Object, _mockCommand2.Object };
        _handler = new UserInputHandler(commands, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_CommandFound_ReturnsCommandResult()
    {
        // Arrange
        var input = "Score Match 01";
        var expectedResult = "Match result";
        var splitInput = new[] { "Score", "Match", "01" };

        _mockCommand1.Setup(c => c.CanHandle(It.IsAny<IList<string>>()))
            .Returns(false);
        _mockCommand2.Setup(c => c.CanHandle(It.Is<IList<string>>(list => 
            list.SequenceEqual(splitInput))))
            .Returns(true);
        _mockCommand2.Setup(c => c.Handle(It.Is<IList<string>>(list => 
            list.SequenceEqual(splitInput)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(input);

        // Assert
        result.Should().Be(expectedResult);
        _mockCommand2.Verify(c => c.Handle(It.Is<IList<string>>(list => 
            list.SequenceEqual(splitInput)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoCommandFound_ReturnsHelpMessageAndLogsWarning()
    {
        // Arrange
        var input = "Unknown Command";
        
        _mockCommand1.Setup(c => c.CanHandle(It.IsAny<IList<string>>()))
            .Returns(false);
        _mockCommand2.Setup(c => c.CanHandle(It.IsAny<IList<string>>()))
            .Returns(false);

        // Act
        var result = await _handler.Handle(input);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Error: Unrecognised command");
        result.Should().Contain("Available commands:");
        result.Should().Contain("Score Match <id>");
        result.Should().Contain("Games Player <name>");
        result.Should().Contain("quit");
        
        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unknown command: Unknown Command")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_InputWithExtraSpaces_TrimsAndSplitsCorrectly()
    {
        // Arrange
        var input = "  Score   Match   01  ";
        var expectedSplit = new[] { "Score", "Match", "01" };

        _mockCommand1.Setup(c => c.CanHandle(It.Is<IList<string>>(list => 
            list.SequenceEqual(expectedSplit))))
            .Returns(true);
        _mockCommand1.Setup(c => c.Handle(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Result");

        // Act
        var result = await _handler.Handle(input);

        // Assert
        result.Should().Be("Result");
        _mockCommand1.Verify(c => c.CanHandle(It.Is<IList<string>>(list => 
            list.SequenceEqual(expectedSplit))), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyInput_ReturnsHelpMessage()
    {
        // Arrange
        var input = "";
        
        _mockCommand1.Setup(c => c.CanHandle(It.IsAny<IList<string>>()))
            .Returns(false);
        _mockCommand2.Setup(c => c.CanHandle(It.IsAny<IList<string>>()))
            .Returns(false);

        // Act
        var result = await _handler.Handle(input);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Error: Unrecognised command");
        result.Should().Contain("Available commands:");
    }

    [Fact]
    public async Task Handle_SingleWordInput_SplitsCorrectly()
    {
        // Arrange
        var input = "quit";
        var expectedSplit = new[] { "quit" };

        _mockCommand1.Setup(c => c.CanHandle(It.Is<IList<string>>(list => 
            list.SequenceEqual(expectedSplit))))
            .Returns(true);
        _mockCommand1.Setup(c => c.Handle(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Goodbye");

        // Act
        var result = await _handler.Handle(input);

        // Assert
        result.Should().Be("Goodbye");
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenToCommand()
    {
        // Arrange
        var input = "Score Match 01";
        var cancellationToken = new CancellationToken();

        _mockCommand1.Setup(c => c.CanHandle(It.IsAny<IList<string>>()))
            .Returns(true);
        _mockCommand1.Setup(c => c.Handle(It.IsAny<IList<string>>(), cancellationToken))
            .ReturnsAsync("Result");

        // Act
        await _handler.Handle(input, cancellationToken);

        // Assert
        _mockCommand1.Verify(c => c.Handle(It.IsAny<IList<string>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_FirstCommandMatches_DoesNotCheckSecondCommand()
    {
        // Arrange
        var input = "Score Match 01";

        _mockCommand1.Setup(c => c.CanHandle(It.IsAny<IList<string>>()))
            .Returns(true);
        _mockCommand1.Setup(c => c.Handle(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Result");

        // Act
        await _handler.Handle(input);

        // Assert
        _mockCommand1.Verify(c => c.CanHandle(It.IsAny<IList<string>>()), Times.Once);
        _mockCommand2.Verify(c => c.CanHandle(It.IsAny<IList<string>>()), Times.Never);
    }

    [Theory]
    [InlineData("Games Player John Doe")]
    [InlineData("Score Match 01")]
    [InlineData("help")]
    [InlineData("quit")]
    public async Task Handle_VariousInputFormats_SplitsCorrectly(string input)
    {
        // Arrange
        var expectedSplit = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        _mockCommand1.Setup(c => c.CanHandle(It.Is<IList<string>>(list => 
            list.SequenceEqual(expectedSplit))))
            .Returns(true);
        _mockCommand1.Setup(c => c.Handle(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Handled");

        // Act
        var result = await _handler.Handle(input);

        // Assert
        result.Should().Be("Handled");
        _mockCommand1.Verify(c => c.CanHandle(It.Is<IList<string>>(list => 
            list.SequenceEqual(expectedSplit))), Times.Once);
    }
}