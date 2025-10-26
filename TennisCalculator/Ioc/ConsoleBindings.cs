using Microsoft.Extensions.DependencyInjection;
using TennisCalculator.DataAccess;
using TennisCalculator.DataAccess.IO;
using TennisCalculator.DataAccess.NewFolder;
using TennisCalculator.GamePlay;

namespace TennisCalculator.Ioc;

public static class ConsoleBindings
{
    /// <summary>
    /// Configures dependency injection container with all services and interfaces
    /// </summary>
    /// <returns>Configured service provider</returns>
    public static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register data access components
        services
            .BindDataAccess()
        // Register scoring strategies
        .AddSingleton<IGameScorer, StandardGameScorer>()
        .AddSingleton<ISetScorer, StandardSetScorer>()
        .AddSingleton<IMatchScorer, StandardMatchScorer>()

        // Register application services
        .AddSingleton<TournamentProcessor>()

        // Register query handlers
        .AddSingleton<ScoreMatchQueryHandler>()
        .AddSingleton<GamesPlayerQueryHandler>()

        // Register console interface components
        .AddSingleton<QueryParser>()
        .AddSingleton<CommandLineInterface>();

        return services.BuildServiceProvider();
    }
}
