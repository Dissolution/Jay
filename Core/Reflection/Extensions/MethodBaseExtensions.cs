using System;
using System.Reflection;

namespace Jay.Reflection
{
    public static class MethodBaseExtensions
    {
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
        
        public static Type GetReturnType(this MethodBase? method)
        {
            if (method is null)
                return typeof(void);
            if (method is MethodInfo methodInfo)
                return methodInfo.ReturnType;
            if (method is ConstructorInfo constructorInfo)
                return constructorInfo.DeclaringType ?? typeof(void);
            return typeof(void);
        }
    }
}