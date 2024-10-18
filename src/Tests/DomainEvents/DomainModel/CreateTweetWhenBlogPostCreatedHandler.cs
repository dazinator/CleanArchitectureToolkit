namespace DomainEvents.Tests;
using DomainEvents.Tests.DomainModel;

public class CreateTweetWhenBlogPostCreatedHandler : IHandler<BlogPostCreatedDomainEvent>
{
    private readonly BloggingContext _dbContext;
    
    public CreateTweetWhenBlogPostCreatedHandler(BloggingContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task HandleAsync(BlogPostCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var tweet = new Tweet()
        {
            Content = $"Check out my new blog post! {domainEvent.Post.Title}"
        };
        _dbContext.Tweets.Add(tweet);
        return Task.CompletedTask;
    }
}