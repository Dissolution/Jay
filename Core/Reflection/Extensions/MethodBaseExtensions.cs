using System;
using System.Reflection;

namespace Jay.Reflection
{
    public static class MethodBaseExtensions
    {
        // public static Type[] GetParameterTypes(this MethodBase? method)
        // {
        //     if (method is null)
        //         return Type.EmptyTypes;
        //     var parameters = method.GetParameters();
        //     var types = new Type[parameters.Length];
        //     ParameterInfo parameterInfo;
        //     Type parameterType;
        //     for (var i = 0; i < parameters.Length; i++)
        //     {
        //         parameterInfo = parameters[i];
        //         parameterType = parameterInfo.ParameterType;
        //         if (parameterInfo.IsIn || parameterInfo.IsOut)
        //             parameterType = parameterType.MakeByRefType();
        //         types[i] = parameterType;
        //     }
        //     return types;
        // }

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

        public static Type GetOwnerType(this MethodBase method)
        {
            return method.DeclaringType ??
                   method.ReflectedType ??
                   typeof(void);
        }
    }
}