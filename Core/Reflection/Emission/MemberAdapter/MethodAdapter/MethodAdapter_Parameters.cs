using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Debugging;
using Jay.Debugging.Dumping;

namespace Jay.Reflection.Emission
{
    public static partial class MethodAdapter
    {

        public static IFluentILEmitter EmitLoadParameter(this IFluentILEmitter emitter,
                                                         ParameterType delegateParameter,
                                                         ArgumentType methodParameter,
                                                         out Action<IFluentILEmitter>? postCall,
                                                         Safety safety = Safety.Safe)
        {
            var result = TryEmitLoadParameter(emitter,
                                              delegateParameter,
                                              methodParameter,
                                              out postCall,
                                              safety);
            result.ThrowIfFailed();
            return emitter;
        }

        
        public static Result TryEmitLoadParameter(IFluentILEmitter? emitter,
                                                  ParameterType? delegateParameter,
                                                  ArgumentType? methodParameter,
                                                   out Action<IFluentILEmitter>? postCall,
                                                   Safety safety = Safety.Safe)
        {
            // All parameters can be loaded as value or pointer/ref (pretty much)

            postCall = null;
            if (emitter is null)
                return new ArgumentNullException(nameof(emitter));
            if (delegateParameter is null)
                return new ArgumentNullException(nameof(delegateParameter));
            if (methodParameter is null)
                return new ArgumentNullException(nameof(methodParameter));
            
            // ?T? -> ?T?
            if (delegateParameter.RootType == methodParameter.RootType)
            {
                // Determine what to do via modifiers
                switch (delegateParameter.Modifier, methodParameter.Modifier)
                {
                    case (ParameterModifier.In, ParameterModifier.In):
                    {
                        emitter.LoadArgument(delegateParameter.Index);
                        return true;
                    }
                    case (ParameterModifier.In, ParameterModifier.Ref):
                    case (ParameterModifier.In, ParameterModifier.Pointer):
                    {
                        if (safety.LacksFlag(Safety.AllowInToRef))
                            return GetConversionSafetyException(Safety.AllowInToRef,
                                                                delegateParameter,
                                                                methodParameter);
                        emitter.LoadArgument(delegateParameter.Index);
                        return true;
                    }
                    case (ParameterModifier.In, ParameterModifier.Out):
                    {
                        if (safety.LacksFlag(Safety.AllowInToOut))
                            return GetConversionSafetyException(Safety.AllowInToOut,
                                                                delegateParameter,
                                                                methodParameter);
                        emitter.LoadArgument(delegateParameter.Index);
                        return true;
                    }
                    case (ParameterModifier.In, ParameterModifier.Default):
                    {
                        // Load from the pointer the value
                        emitter.LoadArgument(delegateParameter.Index)
                               .LoadFromAddress(delegateParameter.RootType);
                        return true;
                    }
                    // In just wants any sort of input pointer
                    case (ParameterModifier.Ref, ParameterModifier.In):
                    case (ParameterModifier.Pointer, ParameterModifier.In):
                    // All these are the same    
                    case (ParameterModifier.Ref, ParameterModifier.Ref):
                    case (ParameterModifier.Pointer, ParameterModifier.Ref):
                    case (ParameterModifier.Ref, ParameterModifier.Pointer):
                    case (ParameterModifier.Pointer, ParameterModifier.Pointer):
                    // We just ignore what's in there (overwrite)    
                    case (ParameterModifier.Ref, ParameterModifier.Out):
                    case (ParameterModifier.Pointer, ParameterModifier.Out):
                    {
                        emitter.LoadArgument(delegateParameter.Index);
                        return true;
                    }
                    case (ParameterModifier.Ref, ParameterModifier.Default):
                    case (ParameterModifier.Pointer, ParameterModifier.Default):
                    {
                        // Load from the pointer the value
                        emitter.LoadArgument(delegateParameter.Index)
                               .LoadFromAddress(delegateParameter.RootType);
                        return true;
                    }
                    case (ParameterModifier.Out, ParameterModifier.Ref):
                    case (ParameterModifier.Out, ParameterModifier.Pointer):
                    {
                        if (safety.LacksFlag(Safety.AllowInToOut))
                            return GetConversionSafetyException(Safety.AllowOutToRef,
                                                                delegateParameter,
                                                                methodParameter);
                        emitter.LoadArgument(delegateParameter.Index);
                        return true;
                    }
                    case (ParameterModifier.Out, ParameterModifier.Out):
                    {
                        emitter.LoadArgument(delegateParameter.Index);
                        return true;
                    }
                    case (ParameterModifier.Out, ParameterModifier.In):
                    case (ParameterModifier.Out, ParameterModifier.Default):
                    {
                        return GetConversionException(delegateParameter, methodParameter);
                    }
                    case (ParameterModifier.Default, ParameterModifier.In):
                    case (ParameterModifier.Default, ParameterModifier.Ref):
                    case (ParameterModifier.Default, ParameterModifier.Pointer):
                    case (ParameterModifier.Default, ParameterModifier.Out):
                    {
                        emitter.LoadArgumentAddress(delegateParameter.Index);
                        return true;
                    }
                    case (ParameterModifier.Default, ParameterModifier.Default):
                    {
                        emitter.LoadArgument(delegateParameter.Index);
                        return true;
                    }
                    default:
                        return GetConversionException(delegateParameter, methodParameter);
                }
            }
            // ?object? -> ?T?
            else if (delegateParameter.RootType == typeof(object))
            {
                switch (delegateParameter.Modifier, methodParameter.Modifier)
                {
                    case (ParameterModifier.Default, ParameterModifier.Default):
                    {
                        if (methodParameter.IsValueType)
                        {
                            emitter.LoadArgument(delegateParameter.Index)
                                   .Unbox(methodParameter.RootType);
                        }
                        else
                        {
                            emitter.LoadArgument(delegateParameter.Index)
                                   .CastClass(methodParameter.RootType);
                        }
                        return true;
                    }
                    case (ParameterModifier.Default, ParameterModifier.In):
                    case (ParameterModifier.Default, ParameterModifier.Ref):
                    case (ParameterModifier.Default, ParameterModifier.Pointer):
                    {
                        if (methodParameter.IsValueType)
                        {
                            emitter.LoadArgument(delegateParameter.Index)
                                   .UnboxToPointer(methodParameter.RootType);
                        }
                        else
                        {
                            // In order to get a ref to a class, we have to store an instance of that class
                            // TODO: Possible to just ref?
                            emitter.LoadArgument(delegateParameter.Index)
                                   .CastClass(methodParameter.RootType)
                                   .DeclareLocal(methodParameter.RootType, out var asClass)
                                   .StoreInLocal(asClass)
                                   .LoadLocalAddress(asClass);
                        }
                        return true;
                    }
                    case (ParameterModifier.Default, ParameterModifier.Out):
                    {
                        if (methodParameter.IsValueType)
                        {
                            emitter.LoadArgument(delegateParameter.Index)
                                   .UnboxToPointer(methodParameter.RootType);
                        }
                        else
                        {
                            return new NotImplementedException();
                        }
                        return true;
                    }
                    case (ParameterModifier.In, ParameterModifier.Default):
                    case (ParameterModifier.Ref, ParameterModifier.Default):
                    case (ParameterModifier.Pointer, ParameterModifier.Default):
                    {
                        if (methodParameter.IsValueType)
                        {
                            emitter.LoadArgument(delegateParameter.Index)
                                   .LoadFromAddress(delegateParameter.RootType)
                                   .Unbox(methodParameter.RootType);
                        }
                        else
                        {
                            emitter.LoadArgument(delegateParameter.Index)
                                   .LoadFromAddress(delegateParameter.RootType)
                                   .CastClass(methodParameter.RootType);
                        }
                        return true;
                    }
                    case (ParameterModifier.In, ParameterModifier.In):
                    case (ParameterModifier.In, ParameterModifier.Ref):
                    case (ParameterModifier.In, ParameterModifier.Pointer):
                    case (ParameterModifier.In, ParameterModifier.Out):
                    
                    case (ParameterModifier.Ref, ParameterModifier.In):
                    case (ParameterModifier.Ref, ParameterModifier.Ref):
                    case (ParameterModifier.Ref, ParameterModifier.Pointer):
                    case (ParameterModifier.Ref, ParameterModifier.Out):
                    
                    case (ParameterModifier.Pointer, ParameterModifier.In):
                    case (ParameterModifier.Pointer, ParameterModifier.Ref):
                    case (ParameterModifier.Pointer, ParameterModifier.Pointer):
                    case (ParameterModifier.Pointer, ParameterModifier.Out):
                    {
                        return new NotImplementedException();
                    }
                    case (ParameterModifier.Out, ParameterModifier.Default):
                    case (ParameterModifier.Out, ParameterModifier.In):
                    case (ParameterModifier.Out, ParameterModifier.Ref):
                    case (ParameterModifier.Out, ParameterModifier.Pointer):
                    case (ParameterModifier.Out, ParameterModifier.Out):
                    default:
                    {
                        return new NotImplementedException();
                    }
                }
            }
            // ?class? -> ?base?
            else if (delegateParameter.RootType.IsAssignableTo(methodParameter.RootType))
            {
                switch (delegateParameter.Modifier, methodParameter.Modifier)
                {
                    case (ParameterModifier.Default, ParameterModifier.Default):
                    {
                        emitter.LoadArgument(delegateParameter.Index)
                               .CastClass(methodParameter.RootType);
                        return true;
                    }
                    case (ParameterModifier.Default, ParameterModifier.In):
                    case (ParameterModifier.Default, ParameterModifier.Ref):
                    case (ParameterModifier.Default, ParameterModifier.Pointer):
                    case (ParameterModifier.Default, ParameterModifier.Out):
                    case (ParameterModifier.In, ParameterModifier.Default):
                    case (ParameterModifier.In, ParameterModifier.In):
                    case (ParameterModifier.In, ParameterModifier.Ref):
                    case (ParameterModifier.In, ParameterModifier.Pointer):
                    case (ParameterModifier.In, ParameterModifier.Out):
                    case (ParameterModifier.Ref, ParameterModifier.Default):
                    case (ParameterModifier.Ref, ParameterModifier.In):
                    case (ParameterModifier.Ref, ParameterModifier.Ref):
                    case (ParameterModifier.Ref, ParameterModifier.Pointer):
                    case (ParameterModifier.Ref, ParameterModifier.Out):
                    case (ParameterModifier.Pointer, ParameterModifier.Default):
                    case (ParameterModifier.Pointer, ParameterModifier.In):
                    case (ParameterModifier.Pointer, ParameterModifier.Ref):
                    case (ParameterModifier.Pointer, ParameterModifier.Pointer):
                    case (ParameterModifier.Pointer, ParameterModifier.Out):
                    case (ParameterModifier.Out, ParameterModifier.Default):
                    case (ParameterModifier.Out, ParameterModifier.In):
                    case (ParameterModifier.Out, ParameterModifier.Ref):
                    case (ParameterModifier.Out, ParameterModifier.Pointer):
                    case (ParameterModifier.Out, ParameterModifier.Out):
                    default:
                    {
                        return new NotImplementedException();
                    }
                }
            }
            // ?
            else
            {
                Hold.Debug(delegateParameter, methodParameter);
                return new NotImplementedException();
            }
        }

        public static Result TryEmitLoadParams(IFluentILEmitter? emitter,
                                               ParameterType? delegateParams,
                                                         MethodBase? targetMethod,
                                                         int targetParameterOffset,
                                                         Safety safety = Safety.Safe)
        {
            if (emitter is null) return new ArgumentNullException(nameof(emitter));
            if (delegateParams is null) return new ArgumentNullException(nameof(delegateParams));
            Debug.Assert(delegateParams.IsParams);
            Debug.Assert(delegateParams.Modifier == ParameterModifier.Default);
            Debug.Assert(delegateParams.Type.Implements<Array>());
            if (targetMethod is null) return new ArgumentNullException(nameof(targetMethod));
            if (targetParameterOffset < 0)
                return new ArgumentException("Invalid Method Parameter Offset", nameof(targetParameterOffset));

            var targetParameters = targetMethod.GetArgumentTypes();
            int neededArgs = targetParameters.Length - targetParameterOffset;

            emitter.LoadArgument(delegateParams.Index)
                   .Array.GetLength()
                   .LoadConstant(neededArgs);
            Label good;
            if (safety.HasFlag<Safety>(Safety.IgnoreExtraParams))
            {
                emitter.BranchIf(CompareCondition.GreaterThanOrEqual, out good);
            }
            else
            {
                emitter.BranchIf(CompareCondition.Equal, out good);
            }

            var exCtor = typeof(ConversionException)
                         .GetConstructor(new Type[] {typeof(string), typeof(Exception)})
                         .ThrowIfNull();
            var resultCtor = typeof(Result)
                             .GetConstructor(Reflect.InstanceFlags,
                                             null,
                                             new Type[] {typeof(bool), typeof(Exception)},
                                             null).ThrowIfNull();
            emitter.LoadConstant(false)
                   .LoadString(Dumper.Format($"Invalid number of params to fill {targetMethod}'s parameters"))
                   .LoadNull()
                   .New(exCtor)
                   .New(resultCtor)
                   .Return();

            emitter.MarkLabel(good);
            Type arrayElementType = delegateParams.Type.GetElementType()
                                                  .ThrowIfNull();
            
            for (var i = 0; i < neededArgs; i++)
            {
                ArgumentType targetParam = targetParameters[i + targetParameterOffset];
                emitter.LoadArgument(delegateParams.Index)
                       .LoadConstant(i);
                if (targetParam.RootType == arrayElementType)
                {
                    if (targetParam.Modifier == ParameterModifier.Default)
                    {
                        emitter.Array.LoadElement(arrayElementType);
                    }
                    else
                    {
                        emitter.Array.LoadElementAddress(arrayElementType);
                    }
                }
                else if (arrayElementType == typeof(object))
                {
                    if (targetParam.IsValueType)
                    {
                        if (targetParam.Modifier == ParameterModifier.Default)
                        {
                            emitter.Array.LoadElement(arrayElementType)
                                   .Unbox(targetParam.RootType);
                        }
                        else
                        {
                            emitter.Array.LoadElement(arrayElementType)
                                   .UnboxToPointer(targetParam.RootType);
                        }
                    }
                    else
                    {
                        if (targetParam.Modifier == ParameterModifier.Default)
                        {
                            emitter.Array.LoadElement(arrayElementType)
                                   .CastClass(targetParam.RootType);
                        }
                        else
                        {
                            return new NotImplementedException();
                        }
                    }
                }
                else
                {
                    return new NotImplementedException();
                }
            }

            return true;
        }
    }
}