using TennisCalculator.GamePlay;
using TennisCalculator.Domain;
using Microsoft.Extensions.DependencyInjection;
using TennisCalculator;
using TennisCalculator.Ioc;


try
{
    // Validate command-line arguments
    var validationResult = ValidateArguments(args);
    if (validationResult != 0)
    {
        return validationResult;
    }

    var filePath = args[0];

    // Validate file existence
    if (!File.Exists(filePath))
    {
        Console.WriteLine($"Error: Tournament file '{filePath}' not found");
        return 1;
    }

    // Configure dependency injection
    var serviceProvider = ConsoleBindings.ConfigureServices();
            
    // Load tournament data
    var tournamentProcessor = serviceProvider.GetRequiredService<TournamentProcessor>();
    try
    {
        await tournamentProcessor.ProcessTournamentDataAsync(filePath);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading tournament data: {ex.Message}");
        return 1;
    }

    // Start interactive mode
    var commandLineInterface = serviceProvider.GetRequiredService<CommandLineInterface>();
    commandLineInterface.StartInteractiveMode();

    return 0;
}
catch (FileProcessingException ex)
{
    Console.WriteLine($"File processing error: {ex.Message}");
    if (ex.LineNumber.HasValue)
    {
        Console.WriteLine($"At line {ex.LineNumber.Value} in file '{ex.FilePath}'");
    }
    return 1;
}
catch (UnsupportedDataSourceException ex)
{
    Console.WriteLine($"Unsupported data source: {ex.Message}");
    Console.WriteLine($"Source: {ex.DataSource}");
    return 1;
}
catch (QueryProcessingException ex)
{
    Console.WriteLine($"Query processing error: {ex.Message}");
    Console.WriteLine($"Query: {ex.Query}");
    return 1;
}
catch (TennisCalculatorException ex)
{
    Console.WriteLine($"Tennis Calculator error: {ex.Message}");
    return 1;
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
    return 1;
}

/// <summary>
/// Validates command-line arguments
/// </summary>
/// <param name="args">Command-line arguments</param>
/// <returns>0 if valid, error code otherwise</returns>
static int ValidateArguments(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Error: Please provide a tournament file path");
        Console.WriteLine("Usage: TennisCalculator <tournament-file-path>");
        return 1;
    }

    if (args.Length > 1)
    {
        Console.WriteLine("Error: Too many arguments provided");
        Console.WriteLine("Usage: TennisCalculator <tournament-file-path>");
        return 1;
    }

    var filePath = args[0];
    if (string.IsNullOrWhiteSpace(filePath))
    {
        Console.WriteLine("Error: Tournament file path cannot be empty");
        return 1;
    }

    return 0;
}

