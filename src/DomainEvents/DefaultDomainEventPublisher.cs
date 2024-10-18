namespace DomainEvents;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Responsibility: Provide a simple mechanism to dispatch domain events to registered handler implementations.
/// No bells or whistles. If you need those, you can swap this implementation out for your own perhaps backed by mediatr etc.
/// </summary>
/// <typeparam name="THandlerInterface">The service interface which will be resolved from the container to handle any published domain events.</typeparam>
public class DefaultDomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private const string HandlerMethodName = nameof(IHandler<IDomainEvent>.HandleAsync);

    public DefaultDomainEventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        this.HandleMethod = typeof(IHandler<IDomainEvent>).GetMethod(HandlerMethodName)!;
    }

    public MethodInfo HandleMethod { get; set; }

    public async Task SendAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // ** Locations marked with double asterix below are candidates for further optimisation, but need some benchmarks in place to be objective.
        Type eventType = domainEvent.GetType();

       // var handlerInterfaceType = IHandler<IDomainEvent>
        
        var handlerServiceType = typeof(IHandler<>).MakeGenericType(eventType);
        var handlers =_serviceProvider.GetServices(handlerServiceType).ToArray();
        
       //   // **
       // var handlers = _serviceProvider.GetServices(handlerType).ToArray();

        var args = new object[] { domainEvent, cancellationToken };
       // handlerImplementation.
        foreach (var handler in handlers)
        {
            var handlerMethod = handlerServiceType.GetMethod(HandlerMethodName); // **
            if (handlerMethod == null)
            {
                continue;
            }
            var task = (Task)handlerMethod.Invoke(handler, args)!;
            await task.ConfigureAwait(false);
        }
    }

    public void Send(IDomainEvent domainEvent)
    {
        throw new NotImplementedException();
    }
}

// /// <summary>
// /// Default implementation of <see cref="IDomainEventPublisher"/> that uses <see cref="IHandler{T}"/> as the handler interface to publish events to.
// /// </summary>
// public class DefaultDomainEventPublisher : DefaultDomainEventPublisher<IHandler<IDomainEvent>>
// {
//     public DefaultDomainEventPublisher(IServiceProvider serviceProvider) : base(serviceProvider)
//     {
//     }
// }