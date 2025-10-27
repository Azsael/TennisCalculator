using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TennisCalculator.Console.Input;
using TennisCalculator.Console.Ioc;
using TennisCalculator.Domain;
using TennisCalculator.Processing.Processor;
using TennisCalculator.Processing.RawData;

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection()
    .ConfigureConsole(configuration);

var data = args.FirstOrDefault();
if (string.IsNullOrWhiteSpace(data))
{
    Console.WriteLine("Error: Please provide a tournament data as file path or data stream");
    return 1;
}

var tournamentProcessor = services.GetRequiredService<ITournamentDataProcessor>();
var commandProcessor = services.GetRequiredService<IUserInputProcessor>();
using var cancellationTokenSource = new CancellationTokenSource();

try
{
    await tournamentProcessor.ProcessTournamentData(data, cancellationTokenSource.Token);
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading tournament data: {ex.Message}");
    return 1;
}

try
{
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        cancellationTokenSource.Cancel();
    };
    // Start interactive mode
    return await commandProcessor.Process(cancellationTokenSource.Token);
}
catch (TennisDataSourceException ex)
{
    Console.WriteLine($"Data Source processing error: {ex.Message}");
    if (ex.LineNumber.HasValue)
    {
        Console.WriteLine($"At line {ex.LineNumber.Value}");
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
