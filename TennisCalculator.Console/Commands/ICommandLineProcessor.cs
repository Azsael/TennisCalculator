namespace TennisCalculator.Console.Commands;

/// <summary>
/// Respresents an nteractive command-line processor
/// </summary>
internal interface ICommandLineProcessor
{
    /// <summary>
    /// Starts the interactive mode, reading commands from standard input
    /// </summary>
    /// <returns>Console Exit code</returns>
    Task<int> Process();
}