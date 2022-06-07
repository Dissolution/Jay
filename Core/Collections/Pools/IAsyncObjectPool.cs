namespace Jay.Collections.Pools;

/// <summary>
/// An async-pool of <typeparamref name="T"/> instances
/// </summary>
/// <typeparam name="T">An instance class</typeparam>
public interface IAsyncObjectPool<T> : IObjectPool<T>, IAsyncDisposable
    where T : class
{
    /// <summary>
    /// Asynchronously rent a <typeparamref name="T"/> instance that should be <see cref="Return"/>ed.
    /// </summary>
    Task<T> RentAsync();

    /// <summary>
    /// Asynchronously returns a <see cref="Rent"/>ed <typeparamref name="T"/> instance to the pool to be cleaned and re-used.
    /// </summary>
    Task ReturnAsync(T? instance);

    /// <summary>
    /// Asynchronouslyn rents a <typeparamref name="T"/> <paramref name="instance"/>
    /// that will be returned when the returned <see cref="IAsyncDisposable"/> is disposed.
    /// </summary>
    /// <returns>
    /// An <see cref="IAsyncDisposable"/> that will return the <paramref name="instance"/>.
    /// </returns>
    /// <remarks>
    /// <paramref name="instance"/> must not be used after this is disposed.
    /// </remarks>
    async Task<IAsyncPoolInstance<T>> BorrowAsync()
    {
        return new AsyncPoolInstance<T>(this, await RentAsync());
    }

    /// <summary>
    /// Borrows a <typeparamref name="T"/> instance, performs <paramref name="instanceAction"/> on it, and returns it to this pool
    /// </summary>
    /// <param name="instanceAction">
    /// The <see cref="Action{T}"/> to perform on a temporary <typeparamref name="T"/> instance
    /// </param>
    async Task BorrowAsync(Func<T, Task> instanceTask)
    {
        var instance = await RentAsync();
        try
        {
            await instanceTask(instance);
        }
        finally
        {
            await ReturnAsync(instance);
        }
    }

    async Task<TReturn> BorrowAsync<TReturn>(Func<T, Task<TReturn>> instanceTask)
    {
        var instance = await RentAsync();
        try
        {
            return await instanceTask(instance);
        }
        finally
        {
            await ReturnAsync(instance);
        }
    }
}