namespace Jay.Disposable;

#if !(NET48 || NETSTANDARD2_0)
public static partial class Disposable
{
    public static IAsyncDisposable NoneAsync => _unDisposable;
    
    public static IAsyncDisposable FromTask(Task? task)
    {
        return new TaskAsyncDisposable(task);
    }

    public static IAsyncDisposable FromTask(Func<Task>? asyncAction)
    {
        return new FuncTaskAsyncDisposable(asyncAction);
    }

    public static IAsyncDisposable Combine(params IAsyncDisposable?[]? disposables)
    {
        if (disposables is null)
            return _unDisposable;
        return MultiDisposable.Create(disposables);
    }

    public static IAsyncDisposable Combine(IEnumerable<IAsyncDisposable?>? disposables)
    {
        if (disposables is null)
            return _unDisposable;
        return MultiDisposable.Create(disposables);
    }
}
#endif

public static partial class Disposable
{
    private static readonly UnDisposable _unDisposable = new();

    /// <summary>
    /// An <see cref="IDisposable" /> that does nothing when <see cref="M:IDisposable.Dispose" /> is called.
    /// </summary>
    public static IDisposable None => _unDisposable;

    public static IDisposable FromAction(Action? action)
    {
        return new ActionDisposable(action);
    }

    public static IDisposable Combine(params IDisposable?[]? disposables)
    {
        if (disposables is null)
            return _unDisposable;
        return MultiDisposable.Create(disposables);
    }

    public static IDisposable Combine(IEnumerable<IDisposable?>? disposables)
    {
        if (disposables is null)
            return _unDisposable;
        return MultiDisposable.Create(disposables);
    }
}