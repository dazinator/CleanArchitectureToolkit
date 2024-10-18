namespace DomainEvents.EFCore;
using Microsoft.EntityFrameworkCore;

public interface ISaveChangesCallbacks<TDbContext>
    where TDbContext : DbContext
{
    void RegisterAfterSaveChangesAsyncCallback(Func<CancellationToken, Task> callback);

    // void RegisterAfterSaveCallback<TState>(Action<TState> callback, TState state);
    Task InvokeAfterSaveChangesCallbacksAsync(CancellationToken cancellationToken);
    // void InvokeAfterSaveCallbacks();
}

// Infrastructure Layer