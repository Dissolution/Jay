namespace Jay.Reflection.Adapters;

/// <summary>
/// Represents a placeholder <see cref="Type"/>
/// for accessing <see langword="static"/> <see cref="MemberInfo"/>s
/// using the standard member access delegates.
/// </summary>
public struct NoInstance
{
    private static NoInstance _instance;

    /// <summary>
    /// Gets a <see langword="ref"/> to an instance of <see cref="NoInstance"/> for use in accessing <see langword="static"/> methods
    /// </summary>
    public static ref NoInstance Ref
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _instance;
    }
}