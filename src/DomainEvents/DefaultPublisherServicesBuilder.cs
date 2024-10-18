namespace DomainEvents;
using Microsoft.Extensions.DependencyInjection;

public class DefaultPublisherServicesBuilder
{
    public DefaultPublisherServicesBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public DefaultPublisherServicesBuilder AddOpenGenericHandler(Type type)
    {
        Services.AddScoped(typeof(IHandler<>), type);
        return this;
    }


    public IServiceCollection Services { get; }
}