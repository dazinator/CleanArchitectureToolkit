namespace DomainEvents;
using Microsoft.Extensions.DependencyInjection;

public class DomainEventsServicesBuilder
{
    public DomainEventsServicesBuilder(IServiceCollection services)
    {
        Services = services;
        AddDefaultServices();
    }

    private void AddDefaultServices()
    {
        var services = Services;
        services.AddScoped<IDomainEventsService, DomainEventsService>();
        AddDefaultPublisher();
    }

    public void AddPublisher<TPublisher>()
        where TPublisher : class, IDomainEventPublisher
    {
        Services.AddScoped<IDomainEventPublisher, TPublisher>();
    }

    public DomainEventsServicesBuilder AddDefaultPublisher(Action<DefaultPublisherServicesBuilder> configure = null)
    {
        Services.AddScoped<IDomainEventPublisher, DefaultDomainEventPublisher>();
        var builder = new DefaultPublisherServicesBuilder(Services);
        configure?.Invoke(builder);
        return this;
    }


    public IServiceCollection Services { get; }
}