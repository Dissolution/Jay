using System;
using System.Diagnostics;

namespace Jay.Reflection.Emission
{
    public static partial class MethodAdapter
    {
        public static IFluentILEmitter EmitLoad(this IFluentILEmitter emitter,
                                                ArgumentType inputArg,
                                                ArgumentType destType,
                                                Safety safety = Safety.Safe)
        {
            Result result = TryEmitLoad(emitter, inputArg, destType, safety);
            result.ThrowIfFailed();
            return emitter;
        }

        public static Result TryEmitLoad(IFluentILEmitter emitter,
                                         ArgumentType argumentType,
                                         ArgumentType destType,
                                         Safety safety = Safety.Safe)
        {
            if (emitter is null)
                return new ArgumentNullException(nameof(emitter));
            if (argumentType is null)
                return new ArgumentNullException(nameof(argumentType));
            if (destType is null)
                return new ArgumentNullException(nameof(destType));
            
            // T? -> T?
            if (argumentType.RootType == destType.RootType)
            {
                // T -> T?
                if (argumentType.Modifier == ParameterModifier.Default)
                {
                    // T -> T
                    if (destType.Modifier == ParameterModifier.Default)
                    {
                        if (argumentType is ParameterType parameter)
                        {
                            emitter.LoadArgument(parameter.Index);
                        }
                        else
                        {
                            // Nothing
                        }
                        return true;
                    }
                    // T -> T*
                    else
                    {
                        // A parameter is a source
                        if (argumentType is ParameterType parameter)
                        {
                            emitter.LoadArgumentAddress(parameter.Index);
                        }
                        // Otherwise we have to have a local to source to
                        else
                        {
                            // We have to have a local in order to get a ref to something
                            emitter.DeclareLocal(destType.RootType, out var local)
                                   .StoreInLocal(local)
                                   .LoadLocalAddress(local);
                        }
                        return true;
                    }
                }
                // T* -> T?
                else
                {
                    // T* -> T
                    if (destType.Modifier == ParameterModifier.Default)
                    {
                        if (argumentType is ParameterType parameter)
                        {
                            emitter.LoadArgument(parameter.Index)
                                   .LoadFromAddress(destType);
                        }
                        else
                        {
                            emitter.LoadFromAddress(destType);
                        }
                        return true;
                    }
                    // T* -> T*
                    else
                    {
                        if (argumentType is ParameterType parameter)
                        {
                            emitter.LoadArgument(parameter.Index);
                        }
                        else
                        {
                            // Do nothing
                        }
                        return true;
                    }
                }
            }
            // void? -> T?
            else if (argumentType.RootType == typeof(void))
            {
                if (argumentType.Modifier == ParameterModifier.Pointer)
                {
                    Debugger.Break();
                    return new NotImplementedException();
                }
                
                if (safety.LacksFlag(Safety.AllowReturnDefaultFromVoid))
                {
                    return GetConversionSafetyException(Safety.AllowReturnDefaultFromVoid,
                                                        argumentType,
                                                        destType);
                }
                
                if (destType.Modifier == ParameterModifier.Default)
                {
                    if (destType.IsValueType)
                    {
                        // We can create a local, init it (set all to default), and load it
                        emitter.DeclareLocal(destType.RootType, out var local)
                               .LoadLocalAddress(local)
                               .Generate(g => g.Initobj(destType.RootType))
                               .LoadFromLocal(local);
                    }
                    else
                    {
                        // Default(!value) == null
                        emitter.LoadNull();
                    }
                    return true;
                }
                else
                {
                    Debugger.Break();
                    return new NotImplementedException();
                }

                return true;
            }
            // T? -> void?
            else if (destType.RootType == typeof(void))
            {
                if (destType.Modifier == ParameterModifier.Pointer)
                {
                    Debugger.Break();
                    return new NotImplementedException();
                }
               
                if (argumentType is ParameterType parameterType)
                {
                    // Do nothing
                }
                else
                {
                    if (safety.LacksFlag(Safety.AllowPopToVoid))
                    {
                        return GetConversionSafetyException(Safety.AllowPopToVoid,
                                                            argumentType,
                                                            destType);
                    }

                    // Remove the value from the stack
                    emitter.Pop();
                }
                return true;
            }
            // object? -> T?
            else if (argumentType.RootType == typeof(object))
            {
                // object -> T?
                if (argumentType.Modifier == ParameterModifier.Default)
                {
                    // object -> T
                    if (destType.Modifier == ParameterModifier.Default)
                    {
                        if (argumentType is ParameterType parameter)
                        {
                            emitter.LoadArgument(parameter.Index);
                        }
                        
                        if (destType.IsValueType)
                        {
                            emitter.Unbox(destType.RootType);
                        }
                        else
                        {
                            emitter.CastClass(destType.RootType);
                        }
                        return true;
                    }
                    // object -> T*
                    else
                    {
                        if (argumentType is ParameterType parameter)
                        {
                            emitter.LoadArgument(parameter.Index);
                        }
                        if (destType.IsValueType)
                        {
                            emitter.UnboxToPointer(destType.RootType);
                        }
                        else
                        {
                            emitter.CastClass(destType.RootType)
                                   .DeclareLocal(destType.RootType, out var local)
                                   .StoreInLocal(local)
                                   .LoadLocalAddress(local);
                        }
                        return true;
                    }
                }
                // object? -> T?
                else
                {
                    Debugger.Break();
                    return new NotImplementedException();
                }
            }
            // class? -> base?
            else if (argumentType.RootType.IsAssignableTo(destType.RootType))
            {
                // class -> base?
                if (argumentType.Modifier == ParameterModifier.Default)
                {
                    // class -> base
                    if (destType.Modifier == ParameterModifier.Default)
                    {
                        if (argumentType is ParameterType parameter)
                        {
                            emitter.LoadArgument(parameter.Index);
                        }
                        emitter.CastClass(destType);
                        return true;
                    }
                    // class -> base*
                    else
                    {
                        Debugger.Break();
                        return new NotImplementedException();
                    }
                }
                // class* -> base?
                else
                {
                    Debugger.Break();
                    return new NotImplementedException();
                }
            }
            // Conv_ instructions
            // else if (argumentType.Modifier == ParameterModifier.Default &&
            //          destType.Modifier == ParameterModifier.Default)
            // {
            //     if (destType.RootType.EqualsAny(typeof()))
            // }
            
            Debugger.Break();
            return new NotImplementedException();
        }
    }
}