namespace Jay.Collections.Pooling;

/// <summary>
/// An action which cleans a <typeparamref name="T"/> instance <paramref name="value"/>
/// </summary>
/// <typeparam name="T">An instance class type</typeparam>
/// <param name="value">The instance class to perform cleanup actions upon</param>
public delegate void PoolInstanceClean<in T>([NotNull] T value);