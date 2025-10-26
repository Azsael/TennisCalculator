using Microsoft.Extensions.DependencyInjection;
using TennisCalculator.Console.Commands;
using TennisCalculator.Processing.Ioc;
using TennisCalculator.GamePlay.Ioc;
using TennisCalculator.DataAccess.Ioc;

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
    public static IServiceProvider ConfigureConsole(this IServiceCollection services)
    {
        return services
            .BindConsole()
            .BindDomain()
            .BindProcessing()
            .BindGamePlay()
            .BuildServiceProvider();
    }

    public static IServiceCollection BindConsole(this IServiceCollection services)
    {
        return services
            .AddSingleton<IQueryParser, QueryParser>()
            .AddSingleton<ICommandLineProcessor, CommandLineProcessor>();
    }
}