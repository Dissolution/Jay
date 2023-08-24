#if !(NET48 || NETSTANDARD2_0)

namespace Jay.Utilities;

/// <summary>
/// An <see cref="IAsyncDisposable"/> that <c>awaits</c> a <see cref="Task"/> when <see cref="DisposeAsync"/> is called
/// </summary>
public sealed class ActionAsyncDisposable : IAsyncDisposable
{
    private Func<Task>? _taskFunc;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="taskFunc">
    /// An awaitable function that returns a <see cref="Task"/>, will be executed during <see cref="DisposeAsync"/>
    /// </param>
    public ActionAsyncDisposable(Func<Task>? taskFunc)
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