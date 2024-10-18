namespace DomainEvents.Tests.DomainModel;
using MediatR;

public record BlogPostCreatedDomainEvent(Blog Blog, Post Post) : IDomainEvent, INotification;