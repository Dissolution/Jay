namespace Jay.Collections.Pools;

/// <summary>
/// An <see cref="IDisposable"/> that returns a <typeparamref name="T"/> instance value to its source <see cref="IObjectPool{T}"/> when it is disposed.
/// </summary>
public interface IPoolInstance<T> : IDisposable
    where T : class
{
    /// <summary>
    /// Gets the temporary instance value
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this <see cref="IPoolInstance{T}"/> has been disposed
    /// </exception>
    T Instance { get; }
    
    /// <summary>
    /// Gets the <see cref="IObjectPool{T}"/> the <see cref="Instance"/> was obtained from
    /// </summary>
    IObjectPool<T> Pool { get; }
}