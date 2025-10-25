using TennisCalculator.GamePlay;
using TennisCalculator.Domain;

namespace TennisCalculator;

/// <summary>
/// Handles interactive command-line interface for processing user queries
/// </summary>
public class CommandLineInterface
{
    private readonly ScoreMatchQueryHandler _scoreMatchQueryHandler;
    private readonly GamesPlayerQueryHandler _gamesPlayerQueryHandler;
    private readonly QueryParser _queryParser;

    public CommandLineInterface(
        ScoreMatchQueryHandler scoreMatchQueryHandler,
        GamesPlayerQueryHandler gamesPlayerQueryHandler)
    {
        _scoreMatchQueryHandler = scoreMatchQueryHandler ?? throw new ArgumentNullException(nameof(scoreMatchQueryHandler));
        _gamesPlayerQueryHandler = gamesPlayerQueryHandler ?? throw new ArgumentNullException(nameof(gamesPlayerQueryHandler));
        _queryParser = new QueryParser();
    }

    /// <summary>
    /// Starts the interactive mode, reading commands from standard input
    /// </summary>
    public void StartInteractiveMode()
    {
        // Check if input is redirected (piped)
        if (Console.IsInputRedirected)
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
        while ((input = Console.ReadLine()) != null)
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
            Console.Write("> ");
            var input = Console.ReadLine();

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

            Console.WriteLine(result);
            return false;
        }
        catch (InvalidQueryException ex)
        {
            Console.WriteLine(ex.Message);
            DisplayHelp();
            return false;
        }
        catch (QueryProcessingException ex)
        {
            Console.WriteLine($"Query processing error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Displays help text with available commands
    /// </summary>
    private void DisplayHelp()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("  Score Match <id>        - Get match result");
        Console.WriteLine("  Games Player <name>     - Get player statistics");
        Console.WriteLine("  quit                    - Exit application");
        Console.WriteLine("Please try again.");
    }
}