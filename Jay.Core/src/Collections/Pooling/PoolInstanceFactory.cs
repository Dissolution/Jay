namespace Jay.Collections.Pooling;

/// <summary>
/// A function which creates a <typeparamref name="T"/> instance
/// </summary>
/// <typeparam name="T">An instance class type</typeparam>
/// <returns>
/// A non-<c>null</c> <typeparamref name="T"/> instance
/// </returns>
[return: NotNull]
public delegate T PoolInstanceFactory<out T>()
    where T : class;