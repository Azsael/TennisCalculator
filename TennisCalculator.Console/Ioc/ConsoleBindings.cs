using Microsoft.Extensions.DependencyInjection;
using TennisCalculator.DataAccess.Ioc;
using TennisCalculator.GamePlay.Ioc;

namespace TennisCalculator.Console.Ioc;

/// <summary>
/// Dependency injection bindings for Console application
/// </summary>
public static class ConsoleBindings
{
    /// <summary>
    /// Configures dependency injection container with all services and interfaces
    /// </summary>
    /// <returns>Configured service provider</returns>
    public static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        return services
            // Register data access components
            .BindDataAccess()
            // Register gameplay components
            .BindGamePlay()
            // Register console interface components
            .AddSingleton<IQueryParser, QueryParser>()
            .AddSingleton<ICommandLineInterface, CommandLineInterface>()
            .BuildServiceProvider();
    }
}