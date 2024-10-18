using DomainEvents.EFCore;
using Microsoft.EntityFrameworkCore;

namespace DomainEvents.Tests.DomainModel;

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Tweet> Tweets { get; set; }

    public BloggingContext(DbContextOptions options) : base(options)
    {
       
    }
}