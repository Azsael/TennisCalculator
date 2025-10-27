using TennisCalculator.Console.Commands;

namespace TennisCalculator.Console.Input;

/// <summary>
/// Handles interactive command-line interaction for processing user commands
/// </summary>
internal class UserInputProcessor(
    IUserInputHandler inputHandler
) : IUserInputProcessor
{
    /// <summary>
    /// Starts the interactive mode, reading commands from standard input
    /// </summary>
    public async Task<int> Process(CancellationToken cancellationToken = default)
    {
        while (true) 
        {
            var input = (await ReadLineAsync()).Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                if (System.Console.IsInputRedirected) return 0;// exit if redirected
                continue;
            }

            if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                return 0;

            try
            {
                var output = await inputHandler.Handle(input, cancellationToken);

                if (!string.IsNullOrWhiteSpace(output))
                {
                    System.Console.WriteLine(output);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Unexpected error: {ex.Message}");
                DisplayHelp();
            }
        }
    }

    // AI wrote this method, which I find quite clever, I normally would of just done a readline for console apps but its interesting to see how to handle it from a async perspective... not that I really write console apps much :S
    private static async Task<string> ReadLineAsync(CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            // Simple implementation - in a real application, you might want more sophisticated
            // async console input handling
            while (!cancellationToken.IsCancellationRequested)
            {
                if (System.Console.KeyAvailable)
                {
                    return System.Console.ReadLine() ?? string.Empty;
                }
                Thread.Sleep(50);
            }
            cancellationToken.ThrowIfCancellationRequested();
            return string.Empty;
        }, cancellationToken);
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