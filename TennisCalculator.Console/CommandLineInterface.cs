using TennisCalculator.GamePlay;
using TennisCalculator.Domain;

namespace TennisCalculator.Console;

/// <summary>
/// Handles interactive command-line interface for processing user queries
/// </summary>
public class CommandLineInterface(
    IScoreMatchQueryHandler scoreMatchQueryHandler,
    IGamesPlayerQueryHandler gamesPlayerQueryHandler,
    IQueryParser queryParser) : ICommandLineInterface
{
    private readonly IScoreMatchQueryHandler _scoreMatchQueryHandler = scoreMatchQueryHandler ?? throw new ArgumentNullException(nameof(scoreMatchQueryHandler));
    private readonly IGamesPlayerQueryHandler _gamesPlayerQueryHandler = gamesPlayerQueryHandler ?? throw new ArgumentNullException(nameof(gamesPlayerQueryHandler));
    private readonly IQueryParser _queryParser = queryParser ?? throw new ArgumentNullException(nameof(queryParser));

    /// <summary>
    /// Starts the interactive mode, reading commands from standard input
    /// </summary>
    public void StartInteractiveMode()
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
            var query = _queryParser.ParseQuery(input);

            if (query is QuitQuery)
            {
                return true;
            }

            var result = query switch
            {
                ScoreMatchQueryWrapper scoreWrapper => _scoreMatchQueryHandler.Handle(scoreWrapper.Query),
                GamesPlayerQueryWrapper gamesWrapper => _gamesPlayerQueryHandler.Handle(gamesWrapper.Query),
                _ => "Error: Unrecognised command"
            };

            System.Console.WriteLine(result);
            return false;
        }
        catch (InvalidQueryException ex)
        {
            System.Console.WriteLine(ex.Message);
            DisplayHelp();
            return false;
        }
        catch (QueryProcessingException ex)
        {
            System.Console.WriteLine($"Query processing error: {ex.Message}");
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