#if !NETSTANDARD2_0

namespace Jay.Utilities;

public sealed class FuncTaskAsyncDisposable : IAsyncDisposable
{
    private Func<Task>? _taskFunc;

    public FuncTaskAsyncDisposable(Func<Task>? taskFunc)
    {
        _taskFunc = taskFunc;
    }

    public async ValueTask DisposeAsync()
    {
        if (_taskFunc is not null)
        {
            await (_taskFunc.Invoke());
            _taskFunc = null;
        }
    }
}

#endif