namespace Jay;

public static class Disposable
{
    /// <summary>
    /// An <see cref="IDisposable"/> / <see cref="IAsyncDisposable"/> that doesn't do anything.
    /// </summary>
    private sealed class NoDisposable : IDisposable, IAsyncDisposable
    {
        public void Dispose() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class ActionDisposable : IDisposable
    {
        private readonly Action? _action;

        public ActionDisposable(Action? action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action?.Invoke();
        }
    }

    private sealed class TaskDisposable : IAsyncDisposable
    {
        private readonly Task? _task;

        public TaskDisposable(Task? task)
        {
            _task = task;
        }

        public async ValueTask DisposeAsync()
        {
            if (_task is not null)
            {
                await _task;
            }
        }
    }

    private sealed class CombinedDisposable : IDisposable
    {
        private readonly IDisposable?[] _disposables;

        public CombinedDisposable(params IDisposable?[]? disposables)
        {
            _disposables = disposables ?? Array.Empty<IDisposable>();
        }

        public CombinedDisposable(IEnumerable<IDisposable?>? disposables)
        {
            _disposables = disposables?.ToArray() ?? Array.Empty<IDisposable>();
        }

        public void Dispose()
        {
            for (var i = _disposables.Length - 1; i >= 0; i--)
            {
                _disposables[i]?.Dispose();
            }
        }
    }

    private sealed class CombinedAsyncDisposable : IAsyncDisposable
    {
        private readonly IAsyncDisposable?[] _disposables;

        public CombinedAsyncDisposable(params IAsyncDisposable?[]? disposables)
        {
            _disposables = disposables ?? Array.Empty<IAsyncDisposable>();
        }

        public CombinedAsyncDisposable(IEnumerable<IAsyncDisposable?>? disposables)
        {
            _disposables = disposables?.ToArray() ?? Array.Empty<IAsyncDisposable>();
        }

        public async ValueTask DisposeAsync()
        {
            for (var i = _disposables.Length - 1; i >= 0; i--)
            {
                var disposable = _disposables[i];
                if (disposable != null)
                {
                    await disposable.DisposeAsync();
                }
            }
        }
    }


    public static IDisposable Action(Action? action)
    {
        return new ActionDisposable(action);
    }

    public static IAsyncDisposable Task(Task? task)
    {
        return new TaskDisposable(task);
    }

    private static readonly NoDisposable _noDisposable = new NoDisposable();

    /// <summary>
    /// An <see cref="IDisposable"/> that does nothing when <see cref="M:IDisposable.Dispose"/> is called.
    /// </summary>
    public static IDisposable None => _noDisposable;

    public static IAsyncDisposable NoneAsync => _noDisposable;

    public static IDisposable Combine(params IDisposable?[]? disposables)
    {
        if (disposables is null)
            return _noDisposable;
        return new CombinedDisposable(disposables);
    }

    public static IDisposable Combine(IEnumerable<IDisposable?>? disposables)
    {
        if (disposables is null)
            return _noDisposable;
        return new CombinedDisposable(disposables);
    }

    public static IAsyncDisposable Combine(params IAsyncDisposable?[]? disposables)
    {
        if (disposables is null)
            return _noDisposable;
        return new CombinedAsyncDisposable(disposables);
    }

    public static IAsyncDisposable Combine(IEnumerable<IAsyncDisposable?>? disposables)
    {
        if (disposables is null)
            return _noDisposable;
        return new CombinedAsyncDisposable(disposables);
    }
}