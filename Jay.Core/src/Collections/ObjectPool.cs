using Jay.Utilities;

namespace Jay.Collections;

/// <summary>
/// Static methods for creating <see cref="ObjectPool{T}"/> instances
/// </summary>
public static class ObjectPool
{
    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/>
    /// </summary>
    /// <typeparam name="T">An instance class type</typeparam>
    /// <param name="factory">A function to create a new <typeparamref name="T"/> instance</param>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T"/> when it is returned</param>
    /// <param name="dispose">An optional action to perform on a <typeparamref name="T"/> if it is discarded</param>
    /// <returns>A new <see cref="ObjectPool{T}"/></returns>
    public static ObjectPool<T> Create<T>(
        Func<T> factory,
        Action<T>? clean = null,
        Action<T>? dispose = null)
        where T : class
    {
        return new ObjectPool<T>(factory, clean, dispose);
    }


    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/> for classes with default constructors.
    /// </summary>
    /// <typeparam name="T">A class with a default constructor.</typeparam>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T"/> when it is returned.</param>
    /// <param name="dispose">An optional action to perform on a <typeparamref name="T"/> if it is disposed.</param>
    /// <returns>A new <see cref="ObjectPool{T}"/> instance.</returns>
    public static ObjectPool<T> Create<T>(
        Action<T>? clean = null,
        Action<T>? dispose = null, 
        // ReSharper disable once InvalidXmlDocComment
        Constraints.IsNew<T> _ = default)
        where T : class, new()
    {
        return new ObjectPool<T>(static () => new(), clean, dispose);
    }

    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/> for <see cref="IDisposable"/> classes.
    /// </summary>
    /// <typeparam name="T">An <see cref="IDisposable"/> class.</typeparam>
    /// <param name="factory">A function to create a new <typeparamref name="T"/> instance.</param>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T"/> when it is returned.</param>
    /// <returns>A new <see cref="ObjectPool{T}"/> instance.</returns>
    public static ObjectPool<T> Create<T>(
        Func<T> factory,
        Action<T>? clean = null, 
        // ReSharper disable once InvalidXmlDocComment
        Constraints.IsDisposable<T> _ = default)
        where T : class, IDisposable
    {
        return new ObjectPool<T>(factory, clean, static item => item.Dispose());
    }

    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/> for <see cref="IDisposable"/> classes with a default constructor.
    /// </summary>
    /// <typeparam name="T">An <see cref="IDisposable"/> class with a default constructor.</typeparam>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T"/> when it is returned.</param>
    /// <returns>A new <see cref="ObjectPool{T}"/> instance.</returns>
    public static ObjectPool<T> Create<T>(
        Action<T>? clean = null, 
        // ReSharper disable once InvalidXmlDocComment
        Constraints.IsNewDisposable<T> _ = default)
        where T : class, IDisposable, new()
    {
        return new ObjectPool<T>(static () => new(), clean, static item => item.Dispose());
    }
}