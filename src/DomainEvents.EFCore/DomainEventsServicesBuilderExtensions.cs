namespace DomainEvents.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DomainEventsServicesBuilderExtensions
{
    public static DomainEventsServicesBuilder<TDbContext> AddEntityFrameworkCore<TDbContext>(this DomainEventsServicesBuilder builder)
        where TDbContext : DbContext
    {
        // These services are responsible for picking up domain events created by the domain and dispatching them to the domain event handlers.
        var services = builder.Services;

        services
            .AddScoped<ISaveChangesCallbacks<TDbContext>, SaveChangesCallbacks<TDbContext>>() // allows you to register callbacks to be executed after SaveChanges is called.
            .AddScoped<EnsureDomainEventsAreProcessedBeforeSaveChangesInterceptor<TDbContext>>()
            .AddScoped<IUnitOfWork, EfCoreUnitOfWork<TDbContext>>();

        return new DomainEventsServicesBuilder<TDbContext>(services);
    }
}

/// <summary>
/// Maker class for adding services to the DI container in the context of having configured EF Core.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public class DomainEventsServicesBuilder<TDbContext> : DomainEventsServicesBuilder
    where TDbContext : DbContext
{
    private readonly IServiceCollection _services;

    public DomainEventsServicesBuilder(IServiceCollection services) : base(services)
    {
        _services = services;
    }

    /// <summary>
    /// Adds an implementation of <see cref="IUnitOfWork"/> for EF Core that will ensure domain events are processed before SaveChanges is called.
    /// </summary>
    /// <remarks>Usage of this <see cref="IUnitOfWork"/></remarks> is optional. You can define your own unit of work interface and implementation and use that instead, but the implementation must call <see cref="IDomainEventsService.ProcessAsync"/> prior to saving changes.
    public void AddUnitOfWork()
    {
        _services.AddScoped<IUnitOfWork, EfCoreUnitOfWork<TDbContext>>();
    }
}