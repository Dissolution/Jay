using System.Reflection;

namespace Jay.Reflection;

public static class MethodBaseExtensions
{
    public static Visibility Access(this MethodBase? method)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (method is null)
            return visibility;
        if (method.IsPrivate)
            visibility |= Reflection.Visibility.Private;
        if (method.IsFamily)
            visibility |= Reflection.Visibility.Protected;
        if (method.IsAssembly)
            visibility |= Reflection.Visibility.Internal;
        if (method.IsPublic)
            visibility |= Reflection.Visibility.Public;
        return visibility;
    }

    public static bool IsStatic(this MethodBase? method)
    {
        return method is not null && method.IsStatic;
    }

    public static Type ReturnType(this MethodBase? method)
    {
        if (method is null)
            return typeof(void);
        if (method is MethodInfo methodInfo)
            return methodInfo.ReturnType;
        if (method is ConstructorInfo constructorInfo)
            return constructorInfo.DeclaringType!;
        return typeof(void);
    }
}