using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TennisCalculator.Console.Commands;
using TennisCalculator.Console.Input;
using TennisCalculator.Domain.Ioc;
using TennisCalculator.Processing.Ioc;

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
    public static IServiceProvider ConfigureConsole(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
            })
            .BindConsole()
            .BindDomain()
            .BindProcessing()
            .BuildServiceProvider();
    }

    public static IServiceCollection BindConsole(this IServiceCollection services)
    {
        return services
            .AddSingleton<IUserInputProcessor, UserInputProcessor>()
            .AddSingleton<IUserInputHandler, UserInputHandler>()
            // Register user commands
            .AddSingleton<IUserCommand, ScoreMatchCommand>()
            .AddSingleton<IUserCommand, GamesPlayerCommand>();
    }
}