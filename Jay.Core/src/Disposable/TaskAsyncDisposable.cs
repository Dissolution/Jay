#if !(NET48 || NETSTANDARD2_0)

namespace Jay.Disposable;

public sealed class TaskAsyncDisposable : IAsyncDisposable
{
    private Task? _task;

    public TaskAsyncDisposable(Task? task)
    {
        _task = task;
    }

    public async ValueTask DisposeAsync()
    {
        var task = Interlocked.Exchange(ref _task, null);
        if (task is not null)
        {
            await task;
        }
    }
}

#endif