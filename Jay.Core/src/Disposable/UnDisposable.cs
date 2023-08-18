namespace Jay.Disposable;

/// <summary>
/// An <see cref="IDisposable" /> / <see cref="IAsyncDisposable" /> that doesn't do anything.
/// </summary>
internal sealed class UnDisposable : IDisposable
#if !(NET48 || NETSTANDARD2_0)
    , IAsyncDisposable
#endif
{
#if !(NET48 || NETSTANDARD2_0)
    public ValueTask DisposeAsync()
    {
        return default; // == ValueTask.CompletedTask;
    }
#endif
    
    public void Dispose()
    {
        // Do nothing
    }
}