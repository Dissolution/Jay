namespace Jay.Reflection.Cloning;

[return: NotNullIfNotNull(nameof(value))]
public delegate T? DeepClone<T>(T? value);