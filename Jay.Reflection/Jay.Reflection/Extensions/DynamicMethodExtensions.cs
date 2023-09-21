namespace Jay.Reflection.Extensions;

public static class DynamicMethodExtensions
{
    #if !NET6_0_OR_GREATER
    public static TDelegate CreateDelegate<TDelegate>(this DynamicMethod dynamicMethod)
        where TDelegate : Delegate
    {
        return (TDelegate)dynamicMethod.CreateDelegate(typeof(TDelegate));
    }
    #endif
}