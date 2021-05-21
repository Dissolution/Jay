using System;
using System.Reflection;

namespace Jay.Reflection
{
    public static class MethodBaseExtensions
    {
        public static Type[] GetParameterTypes(this MethodBase? method)
        {
            if (method is null)
                return Type.EmptyTypes;
            var parameters = method.GetParameters();
            var types = new Type[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].ParameterType;
            }
            return types;
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