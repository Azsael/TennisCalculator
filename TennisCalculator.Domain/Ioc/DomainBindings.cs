using Microsoft.Extensions.DependencyInjection;

namespace TennisCalculator.Domain.Ioc;

public static class DomainBindings
{
    /// <summary>
    /// Configures dependency injection for current project services and interfaces
    /// </summary>
    /// <returns>Configured service provider</returns>
    public static IServiceCollection BindDomain(this IServiceCollection services)
    {
        return services
            .AddSingleton<ITennisMatchRepository, TennisMatchMemoryBank>()
        ;
    }
}
