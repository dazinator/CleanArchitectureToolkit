namespace DomainEvents.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    public static async Task CreateInNewScope<TService>(
        this IServiceProvider serviceProvider,
        Func<TService, Task> execute) 
        where TService : notnull
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        await execute(service);
    }
    
    public static async Task CreateInNewScope<TService1, TService2>(
        this IServiceProvider serviceProvider,
        Func<TService1, TService2, Task> execute) 
        where TService1 : notnull
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var service1 = scope.ServiceProvider.GetRequiredService<TService1>();
        var service2 = scope.ServiceProvider.GetRequiredService<TService2>();
        await execute(service1, service2);
    }
}