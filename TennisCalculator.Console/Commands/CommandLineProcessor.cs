using TennisCalculator.Domain;

namespace TennisCalculator.Console.Commands;

/// <summary>
/// Handles interactive command-line interaction for processing user commands
/// </summary>
internal class CommandLineProcessor(
    ICommandParser commandParser,
    IScoreMatchQueryHandler scoreMatchQueryHandler,
    IGamesPlayerQueryHandler gamesPlayerQueryHandler
) : ICommandLineProcessor
{
    /// <summary>
    /// Starts the interactive mode, reading commands from standard input
    /// </summary>
    public Task<int> Process()
    {
        // Check if input is redirected (piped)
        if (System.Console.IsInputRedirected)
        {
            ProcessRedirectedInput();
        }
        else
        {
            ProcessInteractiveInput();
        }
        return Task.FromResult(0);
    }

    /// <summary>
    /// Processes input from pipes or redirected sources
    /// </summary>
    private void ProcessRedirectedInput()
    {
        string? input;
        while ((input = System.Console.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (ProcessCommand(input.Trim()))
                break;
        }
    }

    /// <summary>
    /// Processes interactive input from the console
    /// </summary>
    private void ProcessInteractiveInput()
    {
        while (true)
        {
            System.Console.Write("> ");
            var input = System.Console.ReadLine();

            if (input == null)
                break;

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (ProcessCommand(input.Trim()))
                break;
        }
    }

    /// <summary>
    /// Processes a single command and returns true if the application should quit
    /// </summary>
    /// <param name="input">The command input</param>
    /// <returns>True if the application should quit, false otherwise</returns>
    private bool ProcessCommand(string input)
    {
        try
        {
            var command = commandParser.ParseCommand(input);

            if (command is QuitCommand)
            {
                return true;
            }

            var result = command switch
            {
                ScoreMatchCommand scoreCommand => scoreMatchQueryHandler.Handle(scoreCommand.Query),
                GamesPlayerCommand gamesCommand => gamesPlayerQueryHandler.Handle(gamesCommand.Query),
                _ => "Error: Unrecognised command"
            };

            System.Console.WriteLine(result);
            return false;
        }
        catch (InvalidCommandException ex)
        {
            System.Console.WriteLine(ex.Message);
            DisplayHelp();
            return false;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Unexpected error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Displays help text with available commands
    /// </summary>
    private void DisplayHelp()
    {
        System.Console.WriteLine("Available commands:");
        System.Console.WriteLine("  Score Match <id>        - Get match result");
        System.Console.WriteLine("  Games Player <name>     - Get player statistics");
        System.Console.WriteLine("  quit                    - Exit application");
        System.Console.WriteLine("Please try again.");
    }
}