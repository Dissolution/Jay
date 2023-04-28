namespace Jay.Utilities;

/// <summary>
/// An <see cref="IDisposable"/> / <see cref="IAsyncDisposable"/> that doesn't do anything.
/// </summary>
internal sealed class UnDisposable : IDisposable
#if !NETSTANDARD2_0                                     
                                     , IAsyncDisposable
#endif
{
    public void Dispose()
    {
    }

#if !NETSTANDARD2_0
    public ValueTask DisposeAsync() => default;  // == ValueTask.CompletedTask;
#endif
}