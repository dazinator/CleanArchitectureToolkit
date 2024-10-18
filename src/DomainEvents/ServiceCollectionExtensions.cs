namespace DomainEvents;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddDomainEvents(this IServiceCollection services, Action<DomainEventsServicesBuilder> configure = null)
    {
        var builder = new DomainEventsServicesBuilder(services);
        configure?.Invoke(builder);
        return services;
    }
}
