using System;
using System.Reflection;
using Jay.Reflection.Emission;

namespace Jay.Reflection
{
    public class MethodAdapter
    {
        private static void WriteAdapter<TDelegate>(ILEmitter emitter, MethodInfo method)
            where TDelegate : Delegate
        {
            MethodSig methodSig = MethodSig.For(method);
            var methodParams = methodSig.ParameterTypes;
            MethodSig delegateSig = MethodSig.For<TDelegate>();
            var delegateParams = delegateSig.ParameterTypes;
            // Static method?
            if (methodSig.IsStatic)
            {
                // TODO: Warning on dropping args?
                // Same or more arguments?
                if (methodParams.Length >= delegateParams.Length)
                {
                    // TODO: params support
                    // Process each in turn
                    for (var p = 0; p < delegateParams.Length; p++)
                    {
                        var methodParam = methodParams[p];
                        var delegateParam = delegateParams[p];
                        if (methodParam == delegateParam)
                        {
                            emitter.Ldarg(p);
                        }
                        else if (delegateParam == typeof(object))
                        {
                            emitter.Ldarg(p)
                                   .BoxIfNeeded(delegateParam);
                        }
                        else if (methodParam == typeof(object))
                        {
                            emitter.Ldarg(p)
                                   .UnboxOrCastclass(methodParam);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        
        public static TDelegate Adapt<TDelegate>(MethodInfo method)
            where TDelegate : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return DelegateBuilder.Build<TDelegate>(emitter => WriteAdapter<TDelegate>(emitter, method));
        }
    }
}