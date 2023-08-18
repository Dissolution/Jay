#if !(NET48 || NETSTANDARD2_0)

namespace Jay.Disposable;

public sealed class FuncTaskAsyncDisposable : IAsyncDisposable
{
    private Func<Task>? _taskFunc;

    public FuncTaskAsyncDisposable(Func<Task>? taskFunc)
    {
        _taskFunc = taskFunc;
    }

    public async ValueTask DisposeAsync()
    {
        var taskFunc = Interlocked.Exchange(ref _taskFunc, null);
        if (taskFunc is not null)
        {
            await taskFunc.Invoke();
        }
    }
}

#endif