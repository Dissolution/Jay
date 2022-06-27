namespace Jay.Reflection;

/// <summary>
/// A cache of <see cref="IEqualityComparer{T}"/>.Default and <see cref="IComparer{T}"/>.Default instances accessible
/// with a <see cref="Type"/>.
/// </summary>
public static class TypeCache
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Default<T>() => default(T);

    public static object? Default(Type? type)
    {
        if (type is null)
            return null;
        if (type.IsClass)
            return null;
        return Activator.CreateInstance(type);
    }

    /// <summary>
    /// Create a new, Uninitialized object of the specified type.
    /// </summary>
    [return: NotNull]
    public static T CreateRaw<T>()
    {
        return (T)RuntimeHelpers.GetUninitializedObject(typeof(T))!;
    }

    /// <summary>
    /// Create a new, Uninitialized object of the specified type.
    /// </summary>
    [return: NotNullIfNotNull("type")]
    public static object? CreateRaw(Type? type)
    {
        if (type is null) return null;
        return RuntimeHelpers.GetUninitializedObject(type);
    }
}