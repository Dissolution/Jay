using System;
using System.Reflection;

namespace Jay.Reflection
{
    public static class MemberInfoExtensions
    {
        public static Visibility GetVisibility(this MemberInfo? memberInfo)
        {
            if (memberInfo is null)
                throw new ArgumentNullException(nameof(memberInfo));
            if (memberInfo is FieldInfo fieldInfo)
                return GetVisibility(fieldInfo);
            if (memberInfo is PropertyInfo propertyInfo)
                return GetVisibility(propertyInfo);
            if (memberInfo is EventInfo eventInfo)
                return GetVisibility(eventInfo);
            if (memberInfo is MethodBase methodBase)
                return GetVisibility(methodBase);
            return default;
        }
        
        public static Visibility GetVisibility(this FieldInfo? fieldInfo)
        {
            Visibility visibility = default;
            if (fieldInfo is null)
                return visibility;
            if (fieldInfo.IsPrivate)
                visibility |= Visibility.Private;
            if (fieldInfo.IsFamily)
                visibility |= Visibility.Protected;
            if (fieldInfo.IsAssembly)
                visibility |= Visibility.Internal;
            if (fieldInfo.IsFamilyAndAssembly || fieldInfo.IsFamilyOrAssembly)
                visibility |= (Visibility.Protected | Visibility.Internal);
            if (fieldInfo.IsPublic)
                visibility |= Visibility.Public;
            return visibility;
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
                visibility |= GetVisibility(propertyInfo.GetGetMethod(false) ?? propertyInfo.GetGetMethod(true));
            }

            if (setters)
            {
                visibility |= GetVisibility(propertyInfo.GetSetMethod(false) ?? propertyInfo.GetSetMethod(true));
            }

            return visibility;
        }
        
        public static Visibility GetVisibility(this EventInfo? eventInfo)
        {
            if (eventInfo is null)
                throw new ArgumentNullException(nameof(eventInfo));
            return GetVisibility(eventInfo.GetAddMethod(false)) |
                   GetVisibility(eventInfo.GetAddMethod(true)) |
                   GetVisibility(eventInfo.GetRemoveMethod(false)) |
                   GetVisibility(eventInfo.GetRemoveMethod(true));
        }
        
        public static Visibility GetVisibility(this MethodBase? methodBase)
        {
            Visibility visibility = default;
            if (methodBase is null)
                return visibility;
            if (methodBase.IsPrivate)
                visibility |= Visibility.Private;
            if (methodBase.IsFamily)
                visibility |= Visibility.Protected;
            if (methodBase.IsAssembly)
                visibility |= Visibility.Internal;
            if (methodBase.IsFamilyAndAssembly || methodBase.IsFamilyOrAssembly)
                visibility |= (Visibility.Protected | Visibility.Internal);
            if (methodBase.IsPublic)
                visibility |= Visibility.Public;
            return visibility;
        }
    }
}