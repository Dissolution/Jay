namespace Jay.Collections.Pooling;

/// <summary>
/// An action which disposes a <c>ref</c> <typeparamref name="T"/> instance <paramref name="value"/>
/// </summary>
/// <typeparam name="T">An instance class type</typeparam>
/// <param name="value">The instance class to perform disposal actions upon</param>
public delegate void PoolInstanceDispose<in T>([NotNull] T value);