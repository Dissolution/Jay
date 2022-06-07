namespace Jay.Collections.Pools;

/// <summary>
/// An <see cref="IAsyncDisposable"/> that returns a <typeparamref name="T"/> instance value to its source <see cref="IAsyncObjectPool{T}"/> when it is disposed.
/// </summary>
public interface IAsyncPoolInstance<T> : IPoolInstance<T>, IAsyncDisposable
    where T : class
{
    /// <summary>
    /// Gets the <see cref="IAsyncObjectPool{T}"/> the <see cref="Instance"/> was obtained from
    /// </summary>
    IAsyncObjectPool<T> AsyncPool { get; }
}