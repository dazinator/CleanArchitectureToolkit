using DomainEvents.EFCore;
using DomainEvents.Tests.DomainModel;
using DomainEvents.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit.Categories;

namespace DomainEvents.Tests;

[IntegrationTest]
public class ScenarioTests
{
    public ScenarioTests()
    {
        DefaultServices = new ServiceCollection();
        DefaultServices.AddLogging();
    }

    public IServiceCollection DefaultServices { get; set; }


    [Fact]
    public async Task TransactionalParticipant_Success()
    {
        var sp = await DefaultServices
            .AddInMemorySqliteDbContext<BloggingContext>()
            .AddDomainEvents(a =>
            {
                a.AddEntityFrameworkCore<BloggingContext>()
                    .AddUnitOfWork();
            })
            .AddScoped<IHandler<BlogPostCreatedDomainEvent>, CreateTweetWhenBlogPostCreatedHandler>()
            .BuildServiceProvider()
            .StartupTasksAsync(
                default); // this is here to initialise the sqlite in memory connection so the in memory db doesn't disappear between di scopes.

        await sp.CreateInNewScope<BloggingContext, IUnitOfWork>(async (context, unitOfWork) =>
        {
            var blog = new Blog { Url = "http://blogs.msdn.com/adonet" };
            var post = blog.CreateBlogPost();
            post.Content = "cool stuff";
            post.Title = "readme";
            context.Add(blog);
            await unitOfWork.CompleteAsync();
        });

        await sp.CreateInNewScope<BloggingContext>(async (context) =>
        {
            var count = await context.Tweets.CountAsync();
            count.ShouldBe(1);
        });
    }

    [Fact]
    public async Task TransactionalParticipant_Failure()
    {
        var sp = await DefaultServices
            .AddInMemorySqliteDbContext<BloggingContext>()
            .AddDomainEvents(a =>
            {
                a.AddEntityFrameworkCore<BloggingContext>()
                    .AddUnitOfWork();
            })
            .AddScoped<IHandler<BlogPostCreatedDomainEvent>, CreateTweetWhenBlogPostCreatedHandler>()
            .AddScoped<IHandler<BlogPostCreatedDomainEvent>, ThrowExceptionHandler<BlogPostCreatedDomainEvent>>()
            .BuildServiceProvider()
            .StartupTasksAsync(
                default); // this is here to initialise the sqlite in memory connection so the in memory db doesn't disappear between di scopes.

        await sp.CreateInNewScope<BloggingContext, IUnitOfWork>(async (context, unitOfWork) =>
        {
            var blog = new Blog { Url = "http://blogs.msdn.com/adonet" };
            var post = blog.CreateBlogPost();
            post.Content = "cool stuff";
            post.Title = "readme";
            context.Add(blog);

            await Should.ThrowAsync<Exception>(async () => await unitOfWork.CompleteAsync());

        });
        
        await sp.CreateInNewScope<BloggingContext>(async context =>
        {
            var count = await context.Tweets.CountAsync();
            count.ShouldBe(0);
            
            // also the blog and blogpost should not be saved
            var blogCount = await context.Blogs.CountAsync();
            blogCount.ShouldBe(0);
            
            var blogPostCount = await context.Posts.CountAsync();
            blogPostCount.ShouldBe(0);
        });
    }
    
    /// <summary>
    /// Tests an end to end integration test involving everything up to the <see cref="IDomainEventPublisher"/> which is mocked, and would otherwise be responsible for actually invoking handlers for the domain event.
    /// This verifies that the basic flow of domain events is working: the <see cref="IDomainEventPublisher"/> is left as an implementation detail to be tested in isolation.
    /// The test involves:
    /// 1. An entity that is pushing a domain event to the domain events context (ambient queue).
    /// 2. An <see cref="IUnitOfWork"/> implementation that calls <see cref="IDomainEventsService.ProcessAsync"/> to process the queued domain events so that they are included in the transaction prior to SaveChanges.
    /// 3. The default <see cref="IDomainEventsService"/> which is <see cref="DomainEventsService"/> which should dequeue all the domain events from the queue and publish them with the <see cref="IDomainEventPublisher"/>
    /// </summary>
    [Fact]
    public async Task EFCore_UnitOfWorkExample_PublishesDomainEvents()
    {
        var publisher = NSubstitute.Substitute.For<IDomainEventPublisher>();
        publisher.SendAsync(Arg.Any<BlogPostCreatedDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var sp = await DefaultServices
            .AddInMemorySqliteDbContext<BloggingContext>()
            .AddDomainEvents(a =>
            {
                a.AddEntityFrameworkCore<BloggingContext>()
                    .AddUnitOfWork();
            })
            .AddScoped<IDomainEventPublisher>(sp => publisher)
            .BuildServiceProvider()
            .StartupTasksAsync(default);

        await sp.CreateInNewScope<BloggingContext, IUnitOfWork>(async (context, unitOfWork) =>
        {
            var blog = new Blog { Url = "http://blogs.msdn.com/adonet" };
            var post = blog.CreateBlogPost();

            // 1. verify that we received the domain event in the ambient domain events queue.
            Assert.True(DomainEventsContext.TryPeek(out var domainEvent));
            domainEvent.ShouldNotBeNull();
            
            post.Content = "cool stuff";
            post.Title = "readme";
            context.Add(blog);

            await unitOfWork.CompleteAsync();
            // 2 + 3. verify that the domain event was processed and removed from the queue.
            Assert.False(DomainEventsContext.Any());
            // 3. and that the publisher was called.
            await publisher.Received().SendAsync(domainEvent, default);
        });
    }
 
}