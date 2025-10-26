using System.Diagnostics;
using System.Text;
using FluentAssertions;
using Xunit;

namespace TennisCalculator.IntegrationTests;

/// <summary>
/// Integration tests for the Tennis Calculator application
/// Tests end-to-end functionality including file processing, query handling, and error scenarios
/// </summary>
public class TennisCalculatorIntegrationTests
{
    private readonly string _testDataPath;
    private readonly string _consoleAppPath;

    public TennisCalculatorIntegrationTests()
    {
        _testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        
        // Find the console app executable - navigate up from test output directory to solution root
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var currentDir = new DirectoryInfo(baseDir);
        
        // Navigate up to find the solution directory (contains .sln file)
        while (currentDir != null && !currentDir.GetFiles("*.sln").Any())
        {
            currentDir = currentDir.Parent;
        }
        
        if (currentDir != null)
        {
            _consoleAppPath = Path.Combine(currentDir.FullName, "TennisCalculator.Console", "bin", "Debug", "net9.0", "TennisCalculator.Console.exe");
        }
        else
        {
            throw new InvalidOperationException("Could not find solution directory");
        }
    }

    [Fact]
    public async Task ProcessFullTournament_WithValidQueries_ReturnsExpectedResults()
    {
        // Arrange
        var tournamentFile = Path.Combine(_testDataPath, "working_tournament.txt");
        var queries = new[]
        {
            "Score Match 02",
            "Games Player Person A",
            "quit"
        };

        // Act
        var result = await RunConsoleAppWithInput(tournamentFile, queries);

        // Assert
        result.ExitCode.Should().Be(0);
        
        var outputLines = result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith(">"))
            .ToArray();

        // Verify match result query
        outputLines.Should().Contain("Person C defeated Person A, 2 sets to 1");
        
        // Verify player statistics query  
        outputLines.Should().Contain("22 24");
    }

    [Fact]
    public async Task ProcessSimpleMatch_WithScoreQuery_ReturnsCorrectResult()
    {
        // Arrange
        var tournamentFile = Path.Combine(_testDataPath, "complete_match.txt");
        var queries = new[]
        {
            "Score Match 01",
            "quit"
        };

        // Act
        var result = await RunConsoleAppWithInput(tournamentFile, queries);

        // Assert
        result.ExitCode.Should().Be(0);
        
        var outputLines = result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith(">"))
            .ToArray();

        outputLines.Should().Contain("Person A defeated Person B, 2 sets to 0");
    }

    [Fact]
    public async Task ProcessTournament_WithInvalidMatchId_ReturnsErrorMessage()
    {
        // Arrange
        var tournamentFile = Path.Combine(_testDataPath, "working_tournament.txt");
        var queries = new[]
        {
            "Score Match 99",
            "quit"
        };

        // Act
        var result = await RunConsoleAppWithInput(tournamentFile, queries);

        // Assert
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Error: Match '99' not found");
    }

    [Fact]
    public async Task ProcessTournament_WithInvalidPlayerName_ReturnsErrorMessage()
    {
        // Arrange
        var tournamentFile = Path.Combine(_testDataPath, "working_tournament.txt");
        var queries = new[]
        {
            "Games Player NonExistentPlayer",
            "quit"
        };

        // Act
        var result = await RunConsoleAppWithInput(tournamentFile, queries);

        // Assert
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Error: Player 'NonExistentPlayer' not found");
    }

    [Fact]
    public async Task ProcessTournament_WithUnrecognisedCommand_ShowsHelpText()
    {
        // Arrange
        var tournamentFile = Path.Combine(_testDataPath, "working_tournament.txt");
        var queries = new[]
        {
            "Invalid Command",
            "quit"
        };

        // Act
        var result = await RunConsoleAppWithInput(tournamentFile, queries);

        // Assert
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Error: Unrecognised command", result.Output);
        Assert.Contains("Available commands:", result.Output);
        Assert.Contains("Score Match <id>", result.Output);
        Assert.Contains("Games Player <name>", result.Output);
        Assert.Contains("quit", result.Output);
    }

    [Fact]
    public async Task StartApplication_WithoutArguments_ReturnsErrorAndExitCode1()
    {
        // Act
        var result = await RunConsoleApp(Array.Empty<string>());

        // Assert
        result.ExitCode.Should().Be(1);
        result.Output.Should().Contain("Error: Please provide a tournament file path");
        result.Output.Should().Contain("Usage: TennisCalculator.Console <tournament-file-path>");
    }

    [Fact]
    public async Task StartApplication_WithNonExistentFile_ReturnsErrorAndExitCode1()
    {
        // Act
        var result = await RunConsoleApp(new[] { "nonexistent.txt" });

        // Assert
        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Error: Tournament file 'nonexistent.txt' not found", result.Output);
    }

    [Fact]
    public async Task ProcessMalformedFile_WithInvalidMatchHeader_HandlesGracefully()
    {
        // Arrange
        var malformedFile = Path.Combine(_testDataPath, "malformed_match_header.txt");

        // Act
        var result = await RunConsoleApp(new[] { malformedFile });

        // Assert
        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Error loading tournament data", result.Output);
    }

    [Fact]
    public async Task ProcessMalformedFile_WithInvalidPoints_HandlesGracefully()
    {
        // Arrange
        var malformedFile = Path.Combine(_testDataPath, "invalid_points.txt");

        // Act
        var result = await RunConsoleApp(new[] { malformedFile });

        // Assert
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Warning: Invalid point value '2'", result.Output);
        Assert.Contains("Warning: Invalid point value 'invalid'", result.Output);
    }

    [Fact]
    public async Task ProcessTournament_WithMultipleValidQueries_ProcessesAllCorrectly()
    {
        // Arrange
        var tournamentFile = Path.Combine(_testDataPath, "working_tournament.txt");
        var queries = new[]
        {
            "Score Match 01",
            "Score Match 02", 
            "Games Player Person A",
            "Games Player Person B",
            "Games Player Person C",
            "quit"
        };

        // Act
        var result = await RunConsoleAppWithInput(tournamentFile, queries);

        // Assert
        Assert.Equal(0, result.ExitCode);
        
        var outputLines = result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith(">"))
            .ToArray();

        // Should contain results for both matches
        Assert.Contains("Person B defeated Person A, 2 sets to 1", outputLines);
        Assert.Contains("Person C defeated Person A, 2 sets to 1", outputLines);
        
        // Should contain player statistics
        Assert.Contains("22 24", outputLines); // Person A stats
    }

    [Fact]
    public async Task ProcessTournament_WithCaseInsensitiveCommands_WorksCorrectly()
    {
        // Arrange
        var tournamentFile = Path.Combine(_testDataPath, "working_tournament.txt");
        var queries = new[]
        {
            "score match 02",
            "games player person a",
            "QUIT"
        };

        // Act
        var result = await RunConsoleAppWithInput(tournamentFile, queries);

        // Assert
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Person C defeated Person A, 2 sets to 1", result.Output);
        Assert.Contains("22 24", result.Output);
    }

    private async Task<ProcessResult> RunConsoleApp(string[] args)
    {
        if (!File.Exists(_consoleAppPath))
        {
            throw new FileNotFoundException($"Console app not found at: {_consoleAppPath}");
        }

        var processInfo = new ProcessStartInfo
        {
            FileName = _consoleAppPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var arg in args)
        {
            processInfo.ArgumentList.Add(arg);
        }

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start process");
        }

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            Output = output + error
        };
    }

    private async Task<ProcessResult> RunConsoleAppWithInput(string tournamentFile, string[] inputCommands)
    {
        if (!File.Exists(_consoleAppPath))
        {
            throw new FileNotFoundException($"Console app not found at: {_consoleAppPath}");
        }

        var processInfo = new ProcessStartInfo
        {
            FileName = _consoleAppPath,
            ArgumentList = { tournamentFile },
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start process");
        }

        // Send input commands
        var inputWriter = process.StandardInput;
        foreach (var command in inputCommands)
        {
            await inputWriter.WriteLineAsync(command);
        }
        inputWriter.Close();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            Output = output + error
        };
    }

    private record ProcessResult
    {
        public int ExitCode { get; init; }
        public string Output { get; init; } = string.Empty;
    }
}