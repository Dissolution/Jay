using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Delegates;
using Jay.Reflection.Runtime;

namespace Jay.Reflection.Emission
{
    public static partial class MethodAdapter
    {
        public static Result<Delegate> TryAdapt(Type delegateType,
                                                MethodBase? targetMethod,
                                                Safety safety = Safety.Safe)
        {
            var dynamicMethod = DelegateBuilder.CreateDynamicMethod(null, MethodSignature.Of(delegateType));
            var emitter = new FluentILEmitter(dynamicMethod);
            var result = TryEmitAdapt(emitter, MemberDelegateCache.GetInvokeMethod(delegateType), targetMethod, safety);
            if (!result)
                return result._error;
            try
            {
                Delegate del = dynamicMethod.CreateDelegate(delegateType);
                return del;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        
        public static Result<TDelegate> TryAdapt<TDelegate>(MethodBase? targetMethod,
                                                            Safety safety = Safety.Safe)
            where TDelegate : Delegate
        {
            var dynamicMethod = DelegateBuilder.CreateDynamicMethod<TDelegate>();
            var emitter = dynamicMethod.GetEmitter();
            var result = TryEmitAdapt(emitter, MemberDelegateCache.GetInvokeMethod<TDelegate>(), targetMethod, safety);
            if (!result)
                return result._error;
            try
            {
                TDelegate del = dynamicMethod.CreateDelegate();
                return del;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        
        public static Result TryEmitAdapt(IFluentILEmitter? emitter,
                                          MethodInfo? delegateMethod,
                                          MethodBase? targetMethod,
                                          Safety safety = Safety.Safe)
        {
            if (emitter is null) return new ArgumentNullException(nameof(emitter));
            if (delegateMethod is null) return new ArgumentNullException(nameof(delegateMethod));
            if (targetMethod is null) return new ArgumentNullException(nameof(targetMethod));
            
            ParameterType[] delegateParams = delegateMethod.GetParameterTypes();
            ArgumentType targetInstance = GetInstanceType(targetMethod);
            var targetParams = targetMethod.GetParameters();
            
            Result result;
            int parameterOffset;
            var postCalls = new List<Action<IFluentILEmitter>>(0);
            
            // Calling a static method?
            if (targetMethod.IsStatic)
            {
                // Do we have a throwaway instance?
                if (delegateParams.Length > 0)
                {
                    // First param is our possible instance
                    var delegateInstance = delegateParams[0];
                    // If it's out, it couldn't possibly be
                    if (delegateInstance.Modifier == ParameterModifier.Out)
                    {
                        parameterOffset = 0;
                    }
                    // Provided Static.Instance
                    else if (delegateInstance.RootType == typeof(Reflect.Static))
                    {
                        parameterOffset = 1;
                    }
                    // object and Type are the only other throwaways we accept
                    else if (delegateInstance.RootType == typeof(object) ||
                             delegateInstance.RootType == typeof(Type))
                    {
                        // But only if we have params
                        if (delegateParams.Length > 1 && delegateParams[1].IsParams)
                        {
                            parameterOffset = 1;
                        }
                        // Or exactly the same number of params following
                        else if (delegateParams.Length == targetParams.Length + 1)
                        {
                            parameterOffset = 1;
                        }
                        else
                        {
                            parameterOffset = 0;
                        }
                    }
                    else
                    {
                        parameterOffset = 0;
                    }
                }
                else
                {
                    parameterOffset = 0;
                }
            }
            // An Instance Method
            else
            {
                // We have to have a valid instance
                if (delegateParams.Length == 0)
                {
                    return GetConversionException(delegateMethod, targetMethod);
                }
                var delegateInstance = delegateParams[0];
                if (delegateInstance.RootType == typeof(object) ||
                    delegateInstance.RootType == targetInstance.RootType ||
                    delegateInstance.RootType.IsAssignableTo(targetInstance))
                {
                    result = TryEmitLoadParameter(emitter,
                                                  delegateInstance,
                                                  targetInstance,
                                                  out var postCall,
                                                  safety);
                    if (!result) 
                        return result;
                    if (postCall != null) postCalls.Add(postCall);
                    parameterOffset = 1;
                }
                else
                {
                    // Invalid instance
                    return new NotImplementedException();
                }
            }
            
            // Now we try to load the arguments
            

            int dpIndex = parameterOffset;
            int tpIndex = 0;

            for (; dpIndex < delegateParams.Length; dpIndex++)
            {
                var delegateParam = delegateParams[dpIndex];
                // Do we only have params left?
                if (dpIndex == delegateParams.Length - 1 && delegateParam.IsParams)
                {
                    if (tpIndex >= targetParams.Length)
                    {
                        // We just ignore the params
                    }
                    else
                    {
                        // Then we can load all these params into what remains
                        result = TryEmitLoadParams(emitter, delegateParam, targetMethod, tpIndex);
                        if (!result) 
                            return result;
                    }
                    // We filled all of them
                    dpIndex = targetParams.Length;
                    break;
                }
                
                if (tpIndex >= targetParams.Length)
                {
                    // We have too many delegate parameters
                    return GetConversionException(delegateMethod, targetMethod);
                }
                
                var targetParam = targetParams[tpIndex++];

                result = TryEmitLoadParameter(emitter, delegateParam, targetParam, out var postCall, safety);
                if (!result)
                    return result;
                if (postCall != null)
                    postCalls.Add(postCall);
            }
            
            // Do we still have targetParams left?
            if (dpIndex < targetParams.Length)
            {
                // Can't fill all target Params
                // TODO: Load parameter defaults?
                return GetConversionException(delegateMethod, targetMethod);
            }

            // Now, we call the method
            if (targetMethod is MethodInfo methodInfo)
            {
                emitter.Call(methodInfo);
            }
            else if (targetMethod is ConstructorInfo constructorInfo)
            {
                emitter.New(constructorInfo);
            }
            else
            {
                Debugger.Break();
                return new NotImplementedException();
            }
            
            // Return the value
            var returnType = targetMethod.GetReturnType();
            
            // Do we have post-call operations before return?
            if (postCalls.Count > 0)
            {
                LocalBuilder? local = null;
                // Do we have to store a return value?
               
                if (returnType != typeof(void))
                {
                    emitter.DeclareLocal(returnType, out local)
                           .StoreInLocal(local);
                }
                // Do the calls
                foreach (var postCall in postCalls)
                {
                    postCall.Invoke(emitter);
                }
                // Pop it back on the stack if we need to
                if (returnType != typeof(void))
                {
                    emitter.LoadFromLocal(local!);
                }
            }
            
            // Convert return type
            result = TryEmitCast(emitter, returnType, delegateMethod.ReturnType, safety);
            if (!result)
                return result;
            
            // Done!
            emitter.Return();
            return true;
        }
    }
}