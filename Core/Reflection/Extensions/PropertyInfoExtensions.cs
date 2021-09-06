using System;
using System.Reflection;

namespace Jay.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static MethodInfo? GetGetter(this PropertyInfo? propertyInfo)
        {
            if (propertyInfo is null) return null;
            return propertyInfo.GetGetMethod(false) ??
                   propertyInfo.GetGetMethod(true);
        }
        
        public static MethodInfo? GetSetter(this PropertyInfo? propertyInfo)
        {
            if (propertyInfo is null) return null;
            return propertyInfo.GetSetMethod(false) ??
                   propertyInfo.GetSetMethod(true);
        }
        
        public static bool IsStatic(this PropertyInfo? propertyInfo)
        {
            if (propertyInfo is null) return false;
            return GetGetter(propertyInfo)?.IsStatic == true ||
                   GetSetter(propertyInfo)?.IsStatic == true;
        }

        public static Visibility GetVisibility(this PropertyInfo? propertyInfo,
                                               bool getters = true,
                                               bool setters = true)
        {
            Visibility visibility = default;
            if (propertyInfo is null)
                return visibility;
            if (getters)
            {
                visibility |= propertyInfo.GetGetter().GetVisibility();
            }
            if (setters)
            {
                visibility |= propertyInfo.GetSetter().GetVisibility();
            }
            return visibility;
        }
        
        public static FieldInfo? GetBackingField(this PropertyInfo? propertyInfo)
        {
            if (propertyInfo is null)
                return null;
            return propertyInfo.InstanceType()
                               .GetField($"<{propertyInfo.Name}>k__BackingField", Reflect.AllFlags);
        }

    
    }
}