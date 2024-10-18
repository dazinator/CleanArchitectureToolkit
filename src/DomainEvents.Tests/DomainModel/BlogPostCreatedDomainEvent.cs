using MediatR;

namespace DomainEvents.Tests.DomainModel;

public record BlogPostCreatedDomainEvent(Blog Blog, Post Post) : IDomainEvent, INotification;