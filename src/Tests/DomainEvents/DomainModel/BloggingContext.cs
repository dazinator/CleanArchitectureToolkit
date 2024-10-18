namespace DomainEvents.Tests.DomainModel;
using DomainEvents.EFCore;
using Microsoft.EntityFrameworkCore;

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Tweet> Tweets { get; set; }

    public BloggingContext(DbContextOptions options) : base(options)
    {
       
    }
}
