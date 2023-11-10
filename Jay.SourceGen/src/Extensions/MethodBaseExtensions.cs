namespace Jay.SourceGen.Extensions;

public static class MethodBaseExtensions
{
    public static Type ReturnType(this MethodBase method)
    {
        if (method is MethodInfo methodInfo)
        {
            return methodInfo.ReturnType;
        }
        else if (method is ConstructorInfo constructorInfo)
        {
            return constructorInfo.DeclaringType!;
        }
        else
        {
            throw new ArgumentException("", nameof(method));
        }
    }
}