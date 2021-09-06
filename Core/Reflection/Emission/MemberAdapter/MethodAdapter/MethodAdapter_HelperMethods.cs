using System;
using System.Diagnostics;
using System.Reflection;

namespace Jay.Reflection.Emission
{
    public static partial class MethodAdapter
    {
        internal static ArgumentType StaticInstanceType = new ArgumentType(typeof(Reflect.Static));
        
        private static ArgumentType GetInstanceType(MethodBase method)
        {
            if (method.IsStatic)
            {
                return StaticInstanceType;
            }
            Type? instanceType = method.ReflectedType;
            if (instanceType is null)
            {
                instanceType = method.DeclaringType;
                if (instanceType is null)
                {
                    return StaticInstanceType;
                }
            }
            Debug.Assert(instanceType.IsByRef == false);
            Debug.Assert(instanceType.IsByRefLike == false);
            if (instanceType.IsValueType)
            {
                return new ArgumentType(instanceType.MakeByRefType());
            }
            else
            {
                return new ArgumentType(instanceType);
            }
        }

        public static ArgumentType[] GetArgumentTypes(this MethodBase method)
        {
            var parameters = method.GetParameters();
            var len = parameters.Length;
            ArgumentType[] delegateParams = new ArgumentType[len];
            for (var i = 0; i < len; i++)
            {
                delegateParams[i] = new ParameterType(parameters[i]);
            }
            return delegateParams;
        }
        
        public static ParameterType[] GetParameterTypes(this MethodBase method)
        {
            var parameters = method.GetParameters();
            var len = parameters.Length;
            ParameterType[] delegateParams = new ParameterType[len];
            for (var i = 0; i < len; i++)
            {
                delegateParams[i] = new ParameterType(parameters[i]);
            }
            return delegateParams;
        }
        
        public static Type[] GetParametersTypes(this MethodBase method)
        {
            var parameters = method.GetParameters();
            var len = parameters.Length;
            Type[] parameterTypes = new Type[len];
            for (var i = 0; i < len; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
            }
            return parameterTypes;
        }
    }
}