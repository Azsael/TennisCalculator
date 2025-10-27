namespace TennisCalculator.Console.Input;

/// <summary>
/// Represents an interactive command-line processor for handling user queries
/// </summary>
internal interface IUserInputProcessor
{
    /// <summary>
    /// Starts the interactive mode, reading commands from standard input
    /// </summary>
    /// <returns>Console Exit code</returns>
    Task<int> Process(CancellationToken cancellationToken = default);
}