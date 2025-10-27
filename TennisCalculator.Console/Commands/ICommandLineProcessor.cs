namespace TennisCalculator.Console.Commands;

/// <summary>
/// Represents an interactive command-line processor for handling user queries
/// </summary>
internal interface ICommandLineProcessor
{
    /// <summary>
    /// Starts the interactive mode, reading commands from standard input
    /// </summary>
    /// <returns>Console Exit code</returns>
    Task<int> Process();
}