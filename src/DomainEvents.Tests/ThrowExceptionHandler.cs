using DomainEvents.Tests.DomainModel;

namespace DomainEvents.Tests;

public class ThrowExceptionHandler<TDomainEvent> : IHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    private readonly BloggingContext _dbContext;
    
    public ThrowExceptionHandler(BloggingContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        throw new Exception("Oh Dear");
    }
}