using System.Reflection;

namespace Jay.Reflection.Extensions;

public static class TypeExtensions
{
    public static MethodInfo? GetInvokeMethod(this Type? type)
    {
        return type?.GetMethod("Invoke", Reflect.PublicFlags);
    }
}