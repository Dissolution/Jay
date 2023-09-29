namespace Jay.Reflection.Adapters;

/// <summary>
/// This <c>Parameter</c> is an instance parameter in a generic method intended for adapting
/// (and thus will be ignored for static member interactions)
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class InstanceAttribute : Attribute
{
    
}