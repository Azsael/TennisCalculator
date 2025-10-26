using Microsoft.Extensions.DependencyInjection;
using TennisCalculator.DataAccess.IO;

namespace TennisCalculator.DataAccess.Ioc;

public static class DataAccessBindings
{
    /// <summary>
    /// Configures dependency injection container with all services and interfaces
    /// </summary>
    /// <returns>Configured service provider</returns>
    public static IServiceCollection BindDataAccess(this IServiceCollection services)
    {
        // Register data access components
        return services
            .AddSingleton<IFileReader, FileReader>()
            .AddSingleton<ITournamentFileParser, TournamentFileParser>()
            .AddSingleton<IMatchRepository, InMemoryMatchRepository>()
            // Register data loaders
            .AddSingleton<IDataLoader, FileDataLoader>();
    }
}
