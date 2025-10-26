using Microsoft.Extensions.DependencyInjection;
using TennisCalculator.Console.Commands;
using TennisCalculator.Console.Ioc;
using TennisCalculator.DataAccess.Processor;
using TennisCalculator.DataAccess.RawData;
using TennisCalculator.Domain;

try
{
    // Configure dependency injection - 
    var services = new ServiceCollection()
        .ConfigureConsole();

    var tournamentProcessor = services.GetRequiredService<ITournamentDataProcessor>();
    var commandProcessor = services.GetRequiredService<ICommandLineProcessor>();

    var data = args.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(data))
    {
        Console.WriteLine("Error: Please provide a tournament data as file path or data stream");
        return 1;
    }

    // Load tournament data
    try
    {
        await tournamentProcessor.ProcessTournamentData(data, CancellationToken.None);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading tournament data: {ex.Message}");
        return 1;
    }

    // Start interactive mode
    return await commandProcessor.Process();
}
catch (TennisDataSourceException ex)
{
    Console.WriteLine($"Data Source processing error: {ex.Message}");
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
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
    return 1;
}
