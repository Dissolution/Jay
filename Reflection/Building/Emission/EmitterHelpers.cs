using System.Runtime.CompilerServices;

namespace Jay.Reflection.Emission;

internal static class EmitterHelpers
{
    public static Lazy<MethodInfo> RuntimeHelpersGetUninitializedObject { get; }
    public static Lazy<MethodInfo> TypeGetTypeFromHandleMethod { get; }

    static EmitterHelpers()
    {
        RuntimeHelpersGetUninitializedObject = new Lazy<MethodInfo>(() =>
        {
            var method = typeof(RuntimeHelpers).GetMethod(nameof(RuntimeHelpers.GetUninitializedObject),
                Reflect.AllFlags,
                new Type[1] {typeof(Type)});
            if (method is null)
                throw new RuntimeException("Cannot find RuntimeHelpers.GetUninitializedObject(type)");
            return method;
        });
        TypeGetTypeFromHandleMethod = new Lazy<MethodInfo>(() =>
        {
            var method = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle),
                Reflect.AllFlags,
                new Type[1] {typeof(RuntimeTypeHandle)});
            if (method is null)
                throw new RuntimeException("Cannot find Type.GetTypeFromHandle(RuntimeTypeHandle)");
            return method;
        });
    }

    public static bool IsObjectArray(this ParameterInfo parameter)
    {
        return !parameter.IsIn &&
               !parameter.IsOut &&
               parameter.ParameterType == typeof(object[]);
    }
    public static bool IsObjectArray(this Type type)
    {
        return !type.IsByRef &&
               type == typeof(object[]);
    }

    private static bool CanCast(Type argType, Type paramType)
    {
        if (argType.IsByRef || paramType.IsByRef)
            throw new NotImplementedException();
        if (argType == paramType) return true;
        if (argType == typeof(object) || paramType == typeof(object)) return true;
        if (argType.Implements(paramType)) return true;
        return false;
    }

    public static IEnumerable<ConstructorInfo> FindConstructors(Type instanceType, params Type[] argTypes)
    {
        return instanceType.GetConstructors(Reflect.InstanceFlags)
                           // Limit to only ones with the same number of params
                           .Where(ctor =>
                           {
                               var ctorParams = ctor.GetParameters();
                               var len = ctorParams.Length;
                               if (len != argTypes.Length) return false;
                               for (var i = 0; i < len; i++)
                               {
                                   if (!CanCast(argTypes[i], ctorParams[i].ParameterType))
                                       return false;
                               }

                               return true;
                           }); }
}