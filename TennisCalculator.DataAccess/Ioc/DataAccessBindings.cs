using Microsoft.Extensions.DependencyInjection;
using TennisCalculator.DataAccess.Loaders;
using TennisCalculator.DataAccess.Processor;
using TennisCalculator.DataAccess.Scoring;

namespace TennisCalculator.DataAccess.Ioc;

public static class DataAccessBindings
{
    /// <summary>
    /// Configures dependency injection container with all services and interfaces
    /// </summary>
    /// <returns>Configured service provider</returns>
    public static IServiceCollection BindDataAccess(this IServiceCollection services)
    {
        return services
            .AddSingleton<ITournamentDataProcessor, TournamentDataProcessor>()
            // Data Loaders and Parsers
            .AddSingleton<ITennisMatchDataLoader, TennisMatchDataLoader>()
            .AddSingleton<ITennisDataLoader, TennisDataFileLoader>()
            .AddSingleton<ITennisDataLoader, TennisDataStreamLoader>()
            .AddSingleton<ITennisDataParser, TennisDataParser>()
            // Register scoring strategies
            .AddSingleton<IGameScorer, StandardGameScorer>()
            .AddSingleton<ISetScorer, StandardSetScorer>()
            .AddSingleton<IMatchScorer, StandardMatchScorer>()
        ;
    }
}
