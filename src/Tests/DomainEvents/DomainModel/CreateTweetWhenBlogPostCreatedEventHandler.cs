namespace DomainEvents.Tests.DomainModel;
using MediatR;

public class CreateTweetWhenBlogPostCreatedEventHandler
    : INotificationHandler<BlogPostCreatedDomainEvent>
{
    private readonly BloggingContext _dbContext;

    public CreateTweetWhenBlogPostCreatedEventHandler(BloggingContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(BlogPostCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var tweet = new Tweet()
        {
            Content = $"Check out my new blog post! {notification.Post.Title}"
        };
        _dbContext.Tweets.Add(tweet);
        return Task.CompletedTask;
        // we are participating in a transaction before SaveChanges() so we don't need to call SaveChanges() ourselves.
    }
}