#if !NETSTANDARD2_0

namespace Jay.Utilities;

public sealed class TaskAsyncDisposable : IAsyncDisposable
{
    private Task? _task;

    public TaskAsyncDisposable(Task? task)
    {
        _task = task;
    }

    public async ValueTask DisposeAsync()
    {
        if (_task is not null)
        {
            await _task;
            _task = null;
        }
    }
}

#endif