using Jay.Utilities;

namespace Jay.Collections.Pooling;

/// <summary>
/// Static methods for creating <see cref="IObjectPool{T}" /> instances
/// </summary>
public static class Pool
{
    /// <summary>
    /// The default capacity any pool should start with
    /// </summary>
    internal static readonly int DefaultCapacity = 1 + (2 * Environment.ProcessorCount);

    /// <summary>
    /// The maximum capacity for any pool
    /// </summary>
    internal const int MAX_CAPACITY = 0X7FFFFFC7; // == Array.MaxLength


    /// <summary>
    /// Creates a new <see cref="IObjectPool{T}" />
    /// </summary>
    /// <typeparam name="T">An instance class type</typeparam>
    /// <param name="factory">A function to create a new <typeparamref name="T" /> instance</param>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T" /> when it is returned</param>
    /// <param name="dispose">An optional action to perform on a <typeparamref name="T" /> if it is discarded</param>
    /// <returns>A new <see cref="IObjectPool{T}" /></returns>
    public static IObjectPool<T> Create<T>(
        PoolInstanceFactory<T> factory,
        PoolInstanceClean<T>? clean = null,
        PoolInstanceDispose<T>? dispose = null)
        where T : class
    {
        return new ObjectPool<T>(factory, clean, dispose);
    }


    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}" /> for classes with default constructors.
    /// </summary>
    /// <typeparam name="T">A class with a default constructor.</typeparam>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T" /> when it is returned.</param>
    /// <param name="dispose">An optional action to perform on a <typeparamref name="T" /> if it is disposed.</param>
    /// <returns>A new <see cref="ObjectPool{T}" /> instance.</returns>
    public static IObjectPool<T> Create<T>(
        PoolInstanceClean<T>? clean = null,
        PoolInstanceDispose<T>? dispose = null, 
        // ReSharper disable once InvalidXmlDocComment
        Constraints.IsNew<T> _ = default)
        where T : class, new()
    {
        return new ObjectPool<T>(static () => new(), clean, dispose);
    }

    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}" /> for <see cref="IDisposable" /> classes.
    /// </summary>
    /// <typeparam name="T">An <see cref="IDisposable" /> class.</typeparam>
    /// <param name="factory">A function to create a new <typeparamref name="T" /> instance.</param>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T" /> when it is returned.</param>
    /// <returns>A new <see cref="ObjectPool{T}" /> instance.</returns>
    public static IObjectPool<T> Create<T>(
        PoolInstanceFactory<T> factory,
        PoolInstanceClean<T>? clean = null, 
        // ReSharper disable once InvalidXmlDocComment
        Constraints.IsDisposable<T> _ = default)
        where T : class, IDisposable
    {
        return new ObjectPool<T>(factory, clean, static item => item.Dispose());
    }

    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}" /> for <see cref="IDisposable" /> classes with a default constructor.
    /// </summary>
    /// <typeparam name="T">An <see cref="IDisposable" /> class with a default constructor.</typeparam>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T" /> when it is returned.</param>
    /// <returns>A new <see cref="ObjectPool{T}" /> instance.</returns>
    public static IObjectPool<T> Create<T>(
        PoolInstanceClean<T>? clean = null, 
        // ReSharper disable once InvalidXmlDocComment
        Constraints.IsNewDisposable<T> _ = default)
        where T : class, IDisposable, new()
    {
        return new ObjectPool<T>(static () => new(), clean, static item => item.Dispose());
    }
}