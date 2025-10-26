using Microsoft.Extensions.DependencyInjection;

namespace TennisCalculator.GamePlay.Ioc;

/// <summary>
/// Dependency injection bindings for GamePlay services
/// </summary>
public static class GamePlayBindings
{
    /// <summary>
    /// Configures dependency injection container with GamePlay services and interfaces
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <returns>The configured service collection</returns>
    public static IServiceCollection BindGamePlay(this IServiceCollection services)
    {
        return services
            // Register query handlers
            .AddSingleton<IScoreMatchQueryHandler, ScoreMatchQueryHandler>()
            .AddSingleton<IGamesPlayerQueryHandler, GamesPlayerQueryHandler>();
    }
}