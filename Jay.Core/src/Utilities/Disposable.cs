namespace Jay.Utilities;

public static class Disposable
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

#if !(NET48 || NETSTANDARD2_0)
    public static IAsyncDisposable NoneAsync => _unDisposable;

    public static IAsyncDisposable FromTask(Func<Task>? asyncAction)
    {
        return new ActionAsyncDisposable(asyncAction);
    }
#endif

    /// <summary>
    /// An <see cref="IDisposable" /> / <c>IAsyncDisposable</c> that doesn't do anything.
    /// </summary>
    internal sealed class UnDisposable : IDisposable
#if !(NET48 || NETSTANDARD2_0)
        , IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            return default; // == ValueTask.CompletedTask;
        }
#else
    {
#endif
        public void Dispose()
        {
            // Do nothing
        }
    }
}