using Microsoft.Extensions.DependencyInjection;
using TennisCalculator.Processing.Loaders;
using TennisCalculator.Processing.Processor;
using TennisCalculator.Processing.RawData;
using TennisCalculator.Processing.Scoring;

namespace TennisCalculator.Processing.Ioc;

public static class ProcessingBindings
{
    /// <summary>
    /// Configures dependency injection container with all services and interfaces
    /// </summary>
    /// <returns>Configured service provider</returns>
    public static IServiceCollection BindProcessing(this IServiceCollection services)
    {
        return services
            .AddSingleton<ITournamentDataProcessor, TournamentDataProcessor>()
            // Data Loaders and Parsers
            .AddSingleton<ITennisMatchDataLoader, TennisMatchDataLoader>()
            .AddSingleton<ITennisDataLoader, TennisDataFileLoader>()
            .AddSingleton<ITennisDataLoader, TennisDataStreamLoader>()
            .AddSingleton<ITennisDataParser, TennisDataParser>()
            // Data Processing
            .AddSingleton<IMatchDataProcessor, MatchDataProcessor>()
            // Register scoring strategies
            .AddSingleton<IGameScorer, StandardGameScorer>()
            .AddSingleton<ISetScorer, StandardSetScorer>()
            .AddSingleton<IMatchScorer, StandardMatchScorer>()
        ;
    }
}
