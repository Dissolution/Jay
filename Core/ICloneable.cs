namespace Jay;

/// <summary>
/// A type-safe <see cref="ICloneable"/>
/// </summary>
/// <typeparam name="TSelf">A reference back to the type of the <see cref="ICloneable{TSelf}"/></typeparam>
public interface ICloneable<out TSelf> : ICloneable
    where TSelf : ICloneable<TSelf>
{
#if !NETSTANDARD2_0
    /// <summary>
    /// Returns a shallow clone the given <typeparamref name="TSelf"/> <paramref name="value"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    static TSelf? Clone(TSelf? value)
    {
        if (value is null) return default;
        return value.Clone();
    }

    /// <summary>
    /// Returns a deep clone the given <typeparamref name="TSelf"/> <paramref name="value"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    static TSelf? DeepClone(TSelf? value)
    {
        if (value is null) return default;
        return value.DeepClone();
    }

    /// <inheritdoc cref="ICloneable"/>
    object ICloneable.Clone() => (object)Clone();
#endif

    /// <summary>
    /// Create a shallow Clone of this instance
    /// </summary>
    new TSelf Clone();

    /// <summary>
    /// Create a deep Clone of this instance
    /// </summary>
    TSelf DeepClone();
}