using System.Reflection;

namespace Jay.Reflection;

public static class PropertyInfoExtensions
{
    public static MethodInfo? GetGetter(this PropertyInfo? propertyInfo)
    {
        return propertyInfo?.GetGetMethod(false) ??
               propertyInfo?.GetGetMethod(true);
    }
        
    public static MethodInfo? GetSetter(this PropertyInfo? propertyInfo)
    {
        return propertyInfo?.GetSetMethod(false) ??
               propertyInfo?.GetSetMethod(true);
    }
        
    public static Visibility Access(this PropertyInfo? propertyInfo)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (propertyInfo is null)
            return visibility;
        visibility |= propertyInfo.GetGetter().Access();
        visibility |= propertyInfo.GetSetter().Access();
        return visibility;
    }
}