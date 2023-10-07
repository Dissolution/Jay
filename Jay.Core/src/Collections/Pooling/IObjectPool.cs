namespace Jay.Collections.Pooling;

/// <summary>
/// A pool of <typeparamref name="T"/> instances
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <c>instance class</c> stored in this pool
/// </typeparam>
/// <remarks>
/// - The main purpose of an <see cref="IObjectPool{T}"/> is to help re-use a limited number of
/// <typeparamref name="T"/> instances rather than continuously <c>new</c>-ing them up.<br/>
/// - It is not the goal to keep all returned instances.<br/>
///     - The pool is not meant for storage (short nor long).<br/>
///     - If there is no space in the pool, extra returned instances will be disposed.<br/>
/// - It is implied that if an instance is obtained from a pool, the caller will return it back in a relatively short time.<br/>
///     - Keeping checked out instances for long durations is _ok_, but it reduces the usefulness of pooling.<br/>
///     - Not returning instances to the pool in not detrimental to its work, but is a bad practice.<br/>
///     - If there is no intent to return or re-use the instance, do not use a pool.<br/>
/// - When this pool is Disposed, all instances will also be disposed.<br/>
///     - Any further returned instances will be cleaned, disposed, and discarded.<br/>
/// </remarks>
public interface IObjectPool<T> : IDisposable
    where T : class
{
    /// <summary>
    /// Gets the maximum number of instances this pool can store
    /// </summary>
    int MaxCapacity { get; }

    /// <summary>
    /// Gets the current number of instances in this pool
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Rents a <typeparamref name="T"/> instance that should be <see cref="Return"/>ed
    /// </summary>
    T Rent();

    /// <summary>
    /// Returns a <see cref="Rent"/>ed <typeparamref name="T"/> instance to the pool to be cleaned and re-used
    /// </summary>
    void Return(T? instance);


    /// <summary>
    /// Borrows a <typeparamref name="T"/> <paramref name="instance"/>
    /// that will be <see cref="Return"/>ed when the returned <see cref="IDisposable"/> is disposed
    /// </summary>
    /// <param name="instance">
    /// A <see cref="M:Borrow"/>ed <typeparamref name="T"/> instance,
    /// it will be returned to its origin <see cref="IObjectPool{T}"/> when disposed
    /// </param>
    /// <returns>An <see cref="IDisposable"/> that will return the <paramref name="instance"/></returns>
    /// <remarks>
    /// <paramref name="instance"/> must not be used after this is disposed
    /// </remarks>
    IDisposable GetInstance(out T instance);

    /// <summary>
    /// Gets a new <see cref="IPoolInstance{T}"/>
    /// </summary>
    IPoolInstance<T> GetInstance();


    /// <summary>
    /// <see cref="Rent"/>s a <typeparamref name="T"/> instance,
    /// performs <paramref name="instanceAction"/> on it,
    /// and then <see cref="Return"/>s it to this pool
    /// </summary>
    /// <param name="instanceAction">
    /// The <see cref="Action{T}"/> perform on the rented instance
    /// </param>
    void Borrow(Action<T> instanceAction);

    /// <summary>
    /// <see cref="Rent"/>s a <typeparamref name="T"/> instance,
    /// performs <paramref name="instanceFunc"/> on it,
    /// <see cref="Return"/>s it to this pool,
    /// and then returns the result of the <paramref name="instanceFunc"/>
    /// </summary>
    /// <param name="instanceFunc">
    /// The <see cref="Func{T, TResult}"/> perform on the rented instance and get the result of
    /// </param>
    TResult Borrow<TResult>(Func<T, TResult> instanceFunc);
}