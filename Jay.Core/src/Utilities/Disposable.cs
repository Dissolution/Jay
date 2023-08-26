namespace Jay.Utilities;

#if !(NET48 || NETSTANDARD2_0)
public static partial class Disposable
{
    public static IAsyncDisposable NoneAsync => _unDisposable;

    public static IAsyncDisposable FromTask(Func<Task>? asyncAction)
    {
        return new ActionAsyncDisposable(asyncAction);
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



    /// <summary>
    /// An <see cref="IDisposable" /> / <see cref="IAsyncDisposable" /> that doesn't do anything.
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