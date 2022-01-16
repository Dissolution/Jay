using System.Reflection;
using Jay.Reflection.Building.Deconstruction;
using Jay.Reflection.Building.Emission;

namespace Jay.Reflection;

public static class MethodBaseExtensions
{
    public static Visibility Visibility(this MethodBase? method)
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

    public static InstructionStream GetInstructions(this MethodBase method)
    {
        return MethodBodyReader.GetInstructions(method);
    }
}