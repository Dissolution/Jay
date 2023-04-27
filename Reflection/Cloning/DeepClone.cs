namespace Jay.Reflection.Cloning;

/// <summary>
/// Deep-clones the given <paramref name="value"/>
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of value to deep clone</typeparam>
[return: NotNullIfNotNull(nameof(value))]
public delegate T? DeepClone<T>(T? value);