namespace Jay;

/// <summary>
/// Extensions on all <see langword="class"/>es.
/// </summary>
public static class ClassExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull<TClass>([NotNullWhen(false)] this TClass? @class)
        where TClass : class
        => @class is null;
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNull<TClass>([NotNullWhen(true)] this TClass? @class)
        where TClass : class
        => @class is not null;
}