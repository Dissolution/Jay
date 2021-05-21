using System.Reflection;

namespace Jay.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static MethodInfo? GetGetter(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetGetMethod(false) ??
                   propertyInfo.GetGetMethod(true);
        }
        
        public static MethodInfo? GetSetter(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetSetMethod(false) ??
                   propertyInfo.GetSetMethod(true);
        }
        
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            return GetGetter(propertyInfo)?.IsStatic == true ||
                   GetSetter(propertyInfo)?.IsStatic == true;
        }
    }
}