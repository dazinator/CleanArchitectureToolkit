namespace DomainEvents.Tests.DomainModel;

public class Blog : DomainEntity
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    // DDD Patterns comment
    // Using a private collection field, better for DDD Aggregate's encapsulation
    // so items cannot be added from "outside the AggregateRoot" directly to the collection,
    // but only through the method OrderAggregateRoot.AddXYZ() which includes behavior.
    private readonly List<Post> _posts = new List<Post>();
    public IReadOnlyCollection<Post> Posts => _posts;

    public Post CreateBlogPost()
    {
        var post = new Post();
        _posts.Add(post);
        AddBlogPostCreatedDomainEvent(post);
        return post;
    }

    private void AddBlogPostCreatedDomainEvent(Post post)
    {
        var orderStartedDomainEvent = new BlogPostCreatedDomainEvent(this, post);
        this.AddDomainEvent(orderStartedDomainEvent);
    }
}