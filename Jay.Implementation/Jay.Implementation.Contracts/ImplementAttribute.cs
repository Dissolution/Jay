namespace IMPL.Contracts;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class ImplementAttribute : Attribute
{
    /// <summary>
    /// Specifies declaration keywords to be added to the implementation
    /// </summary>
    public string? Declaration { get; init; } = null;

    /// <summary>
    /// Specifies an override name for the implementation (if applied to an <c>interface</c>)
    /// </summary>
    public string? Name { get; init; } = null;

    public ImplementAttribute() { }
}



public enum Nullability
{
    /// <summary>
    /// #nullable false
    /// </summary>
    False = 0,

    /// <summary>
    /// <c>#nullable true</c> <br/>
    /// </summary>
    True = 1 << 0,

    /// <summary>
    /// <c>#nullable true</c> <br/>
    /// Also includes <c>null</c> checks around [NotNull] parameters
    /// </summary>
    Defensive = 1 << 0 | True,
}