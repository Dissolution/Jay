namespace Jay.Collections.Pooling;

/// <summary>
/// A function which creates a <typeparamref name="T" /> instance
/// </summary>
/// <typeparam name="T">An instance class type</typeparam>
/// <returns>
/// A non-<c>null</c> <typeparamref name="T"/> instance
/// </returns>
[return: NotNull]
public delegate T PoolInstanceFactory<out T>()
    where T : class;

/// <summary>
/// An action which cleans a <typeparamref name="T" /> instance <paramref name="value" />
/// </summary>
/// <typeparam name="T">An instance class type</typeparam>
/// <param name="value">The instance class to perform cleanup actions upon</param>
public delegate void PoolInstanceClean<in T>([NotNull] T value);

/// <summary>
/// An action which disposes a <c>ref</c> <typeparamref name="T" /> instance <paramref name="value" />
/// </summary>
/// <typeparam name="T">An instance class type</typeparam>
/// <param name="value">The instance class to perform disposal actions upon</param>
public delegate void PoolInstanceDispose<in T>([NotNull] T value);