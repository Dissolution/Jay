namespace Jay.Collections.Pools;

/// <summary>
/// A pool of <typeparamref name="T"/> instances
/// </summary>
/// <typeparam name="T">An instance class</typeparam>
public interface IObjectPool<T> : IDisposable
    where T : class
{
    /// <summary>
    /// Gets the maximum number of items retained by this pool.
    /// </summary>
    int Capacity { get; }

    /// <summary>
    /// Gets the current number of items being stored in this pool.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Rent a <typeparamref name="T"/> instance that should be <see cref="Return"/>ed.
    /// </summary>
    T Rent();

    /// <summary>
    /// Returns a <see cref="Rent"/>ed <typeparamref name="T"/> instance to the pool to be cleaned and re-used.
    /// </summary>
    void Return(T? instance);

    /// <summary>
    /// Rents a <typeparamref name="T"/> <paramref name="instance"/>
    /// that will be returned when the returned <see cref="IDisposable"/> is disposed.
    /// </summary>
    /// <param name="instance">A <typeparamref name="T"/> instance,
    /// it will be returned to its origin <see cref="IObjectPool{T}"/> when disposed.</param>
    /// <returns>An <see cref="IDisposable"/> that will return the <paramref name="instance"/>.</returns>
    /// <remarks>
    /// <paramref name="instance"/> must not be used after this is disposed.
    /// </remarks>
    IPoolInstance<T> Borrow(out T instance)
    {
        instance = Rent();
        return new PoolInstance<T>(this, instance);
    }

    /// <summary>
    /// Borrows a <typeparamref name="T"/> instance, performs <paramref name="instanceAction"/> on it, and returns it to this pool
    /// </summary>
    /// <param name="instanceAction">
    /// The <see cref="Action{T}"/> to perform on a temporary <typeparamref name="T"/> instance
    /// </param>
    void Borrow(Action<T> instanceAction)
    {
        var instance = Rent();
        try
        {
            instanceAction(instance);
        }
        finally
        {
            Return(instance);
        }
    }

    TReturn Borrow<TReturn>(Func<T, TReturn> instanceFunc)
    {
        var instance = Rent();
        try
        {
            return instanceFunc(instance);
        }
        finally
        {
            Return(instance);
        }
    }
}