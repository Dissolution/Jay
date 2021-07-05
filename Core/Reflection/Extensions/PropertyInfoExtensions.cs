using System;
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

        public static Visibility GetVisibility(this PropertyInfo? propertyInfo,
                                               bool getters = true,
                                               bool setters = true)
        {
            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));
            Visibility visibility = default;
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
            return propertyInfo.GetInstanceType()
                               .GetField($"<{propertyInfo.Name}>k__BackingField", Reflect.AllFlags);
        }

    
    }
}