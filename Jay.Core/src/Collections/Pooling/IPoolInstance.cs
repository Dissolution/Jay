namespace Jay.Collections.Pooling;

/// <summary>
/// A <see cref="IPoolInstance{T}"/> is an <see cref="IDisposable"/> that holds onto a
/// <typeparamref name="T" /> <see cref="Instance"/>.<br/>
/// When it is disposed, the <see cref="Instance"/> is returned to its source <see cref="IObjectPool{T}"/>.
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of <see cref="Instance"/></typeparam>
public interface IPoolInstance<out T> : IDisposable
    where T : class
{
    /// <summary>
    /// Gets the temporarily borrowed <typeparamref name="T"/> instance value
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this <see cref="IPoolInstance{T}" /> has been disposed
    /// </exception>
    T Instance { get; }
}