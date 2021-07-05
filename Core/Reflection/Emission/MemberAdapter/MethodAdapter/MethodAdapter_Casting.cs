using System;

namespace Jay.Reflection.Emission
{
    public static partial class MethodAdapter
    {
            public static IFluentILEmitter EmitCast(this IFluentILEmitter emitter,
                                                    ArgumentType sourceType,
                                                    ArgumentType destinationType,
                                                    Safety safety = Safety.Safe)
        {
            var result = TryEmitCast(emitter, sourceType, destinationType, safety);
            result.ThrowIfFailed();
            return emitter;
        }
   

        public static Result TryEmitCast(IFluentILEmitter? emitter,
                                                   ArgumentType? sourceType,
                                                   ArgumentType? destinationType,
                                                   Safety safety = Safety.Safe)
        {
            if (emitter is null)
                return new ArgumentNullException(nameof(emitter));
            if (sourceType is null)
                return new ArgumentNullException(nameof(sourceType));
            if (destinationType is null)
                return new ArgumentNullException(nameof(destinationType));
            
            // ?T? -> ?T?
            if (sourceType.RootType == destinationType.RootType)
            {
                if (sourceType.Modifier == ParameterModifier.Default)
                {
                    if (destinationType.Modifier == ParameterModifier.Default)
                    {
                        // Do nothing
                    }
                    else
                    {
                        // We have to have a local in order to get a ref to something
                        emitter.DeclareLocal(destinationType.RootType, out var local)
                               .StoreInLocal(local)
                               .LoadLocalAddress(local);
                    }
                }
                else
                {
                    if (destinationType.Modifier == ParameterModifier.Default)
                    {
                        emitter.LoadFromAddress(destinationType.RootType);
                    }
                    else
                    {
                        // Do nothing
                    }
                }
                return true;
            }
             // void -> ?T?
            else if (sourceType == typeof(void))
            {
                if (safety.HasFlag<Safety>(Safety.AllowReturnDefaultFromVoid))
                {
                    if (destinationType.Modifier == ParameterModifier.Default)
                    {
                        if (destinationType.IsValueType)
                        {
                            // We can create a local, init it (set all to default), and load it
                            emitter.DeclareLocal(destinationType.RootType, out var local)
                                   .LoadLocalAddress(local)
                                   .Generate(g => g.Initobj(destinationType.RootType))
                                   .LoadFromLocal(local);
                        }
                        else
                        {
                            // Default(!value) == null
                            emitter.LoadNull();
                        }
                    }
                    else
                    {
                        if (destinationType.IsValueType)
                        {
                            // We can create a local, init it (set all to default), and load its address
                            emitter.DeclareLocal(destinationType.RootType, out var local)
                                   .LoadLocalAddress(local)
                                   .Generate(g => g.Initobj(destinationType.RootType))
                                   .LoadLocalAddress(local);
                        }
                        else
                        {
                            // TODO: Verify this works
                            emitter.LoadFieldAddress(_nullObjectField);
                        }
                    }
                    return true;
                }
                else
                {
                    return GetConversionSafetyException(Safety.AllowReturnDefaultFromVoid,
                                                    sourceType,
                                                    destinationType);
                }
            }
            // ?T? -> void
            else if (destinationType == typeof(void))
            {
                emitter.Pop();
                return true;
            }
            // ?object? ->?T?
            else if (sourceType.RootType == typeof(object))
            {
                if (sourceType.Modifier == ParameterModifier.Default)
                {
                    if (destinationType.Modifier == ParameterModifier.Default)
                    {
                        if (destinationType.IsValueType)
                        {
                            emitter.Unbox(destinationType.RootType);
                        }
                        else
                        {
                            emitter.CastClass(destinationType.RootType);
                        }
                    }
                    else
                    {
                        if (destinationType.IsValueType)
                        {
                            emitter.UnboxToPointer(destinationType.RootType);
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
                return true;
            }
            // ?T? -> ?object?
            else if (destinationType.RootType == typeof(object))
            {
                if (sourceType.Modifier == ParameterModifier.Default)
                {
                    if (destinationType.Modifier == ParameterModifier.Default)
                    {
                        if (sourceType.IsValueType)
                        {
                            emitter.Box(sourceType.RootType);
                        }
                        else
                        {
                            // All classes are already objects
                            // TODO: Verify this
                        }
                    }
                    else
                    {
                        return new NotImplementedException();
                    }
                }
                else
                {
                    return new NotImplementedException();
                }
                return true;
            }
            // ?class? -> ?base?
            else if (sourceType.RootType.IsAssignableTo(destinationType.RootType))
            {
                if (sourceType.Modifier == ParameterModifier.Default)
                {
                    if (destinationType.Modifier == ParameterModifier.Default)
                    {
                        emitter.CastClass(destinationType.RootType);
                    }
                    else
                    {
                        return new NotImplementedException();
                    }
                }
                else
                {
                    return new NotImplementedException();
                }
                return true;
            }
            // ?
            else
            {
                return new NotImplementedException();
            }
        }
    }
}