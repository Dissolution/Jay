using System.Diagnostics;
using System.Reflection;

namespace Jay.Reflection.Emission;

internal static class EmitterCastExtensions
{
    private static Result TryEmitCast<TEmitter>(TEmitter emitter,
                                                ParameterInfo input, ParameterInfo output)
        where TEmitter : IOpCodeEmitter<TEmitter>
    {
        return new NotImplementedException();
    }

    //Always the most up-to-date
    private static Result TryEmitCast<TEmitter>(TEmitter emitter,
                                                ParameterInfo input, Type? outputType)
        where TEmitter : IOpCodeEmitter<TEmitter>
    {
        if (outputType is null || outputType == typeof(void))
        {
            // Output requires nothing, we have nothing loaded
            return true;
        }

        if (outputType == typeof(void*))
        {
            return new NotImplementedException("Does not support void*");
        }

        if (input.ParameterType == typeof(void))
        {
            // We're expecting a value that we're not given
            return new ArgumentException("We're expecting a value that we're not given", nameof(input));
        }

        if (input.ParameterType == typeof(void*))
        {
            return new NotImplementedException("Does not support void*");
        }

        var inputAccess = input.GetAccess(out var inputType);
        bool inputIsRef = inputAccess != ParameterInfoExtensions.Access.Default;

        bool outputIsRef = outputType.IsByRef;
        if (outputIsRef)
        {
            outputType = outputType.GetElementType()!;
        }

        // If we have exactly what we need, it's fairly easy
        if (inputType == outputType)
        {
            if (inputIsRef == outputIsRef)
            {
                // Exactly what we want
                emitter.Ldarg(input.Position);
            }
            else if (!inputIsRef)
            {
                // out is ref
                // todo: safety shit
                emitter.Ldarga(input.Position);
            }
            else
            {
                Debug.Assert(!outputIsRef);
                // in is ref
                // todo: safety shit
                emitter.Ldarg(input.Position)
                       .Ldind(outputType);
            }
            return true;
        }
        
        // Coming from object?
        if (inputType == typeof(object) && !inputIsRef)
        {
            if (outputType.IsValueType)
            {
                if (!outputIsRef)
                {
                    emitter.Ldarg(input.Position)
                           .Unbox_Any(outputType);
                }
                else
                {
                    emitter.Ldarg(input.Position)
                           .Unbox(outputType);
                }
            }
            else
            {
                if (!outputIsRef)
                {
                    emitter.Ldarg(input.Position)
                           .Castclass(outputType);
                }
                else
                {
                    return new NotImplementedException("Non-default outputs are not yet supported");
                }
            }
            return true;
        }

        if (inputIsRef)
        {
            return new NotImplementedException("Non-default inputs are not yet supported");
        }

        if (outputIsRef)
        {
            return new NotImplementedException("Non-default outputs are not yet supported");
        }

        // Going to object?
        if (outputType == typeof(object))
        {
            if (inputType.IsValueType)
            {
                emitter.Ldarg(input.Position)
                       .Box(inputType);
            }
            else
            {
                // Is already object
                emitter.Ldarg(input.Position);
            }
            return true;
        }

        // Implements?
        // TODO: More advanced logic here?
        if (inputType.IsAssignableTo(outputType))
        {
            Debug.Assert(inputType.IsClass);
            Debug.Assert(outputType.IsClass);
            emitter.Ldarg(input.Position)
                   .Castclass(outputType);
            return true;
        }

        return new NotImplementedException($"Cannot cast from {input} to {outputType}");

    }
    private static Result TryEmitCast<TEmitter>(TEmitter emitter,
                                                Type input, ParameterInfo output)
        where TEmitter : IOpCodeEmitter<TEmitter>
    {
        return new NotImplementedException();
    }
    private static Result TryEmitCast<TEmitter>(TEmitter emitter,
                                                Type? inputType, Type? outputType)
        where TEmitter : IOpCodeEmitter<TEmitter>
    {
        if (outputType is null || outputType == typeof(void))
        {
            // Output requires nothing, we have nothing loaded
            return true;
        }

        if (outputType == typeof(void*))
        {
            return new NotImplementedException("Does not support void*");
        }

        if (inputType is null || inputType == typeof(void))
        {
            // We're expecting a value that we're not given
            return new ArgumentException("We're expecting a value that we're not given", nameof(inputType));
        }

        if (inputType == typeof(void*))
        {
            return new NotImplementedException("Does not support void*");
        }

        bool inputIsRef = inputType.IsByRef;
        if (inputIsRef)
        {
            inputType = inputType.GetElementType()!;
        }

        bool outputIsRef = outputType.IsByRef;
        if (outputIsRef)
        {
            outputType = outputType.GetElementType()!;
        }

        // If we have exactly what we need, it's fairly easy
        if (inputType == outputType)
        {
            if (inputIsRef == outputIsRef)
            {
                // Exactly what we want
            }
            else if (!inputIsRef)
            {
                // out is ref
                return new NotImplementedException("Non-default outputs are not yet supported");
            }
            else
            {
                Debug.Assert(!outputIsRef);
                // in is ref
                // todo: safety shit
                emitter.Ldind(outputType);
            }
            return true;
        }

        // Coming from object?
        if (inputType == typeof(object) && !inputIsRef)
        {
            if (outputType.IsValueType)
            {
                if (!outputIsRef)
                {
                    emitter.Unbox_Any(outputType);
                }
                else
                {
                    emitter.Unbox(outputType);
                }
            }
            else
            {
                if (!outputIsRef)
                {
                    emitter.Castclass(outputType);
                }
                else
                {
                    return new NotImplementedException("Non-default outputs are not yet supported");
                }
            }
            return true;
        }

        if (inputIsRef)
        {
            return new NotImplementedException("Non-default inputs are not yet supported");
        }

        if (outputIsRef)
        {
            return new NotImplementedException("Non-default outputs are not yet supported");
        }

        // Going to object?
        if (outputType == typeof(object))
        {
            if (inputType.IsValueType)
            {
                emitter.Box(inputType);
            }
            else
            {
                // Is already object
            }
            return true;
        }

        // Implements?
        // TODO: More advanced logic here?
        if (inputType.IsAssignableTo(outputType))
        {
            Debug.Assert(inputType.IsClass);
            Debug.Assert(outputType.IsClass);
            emitter.Castclass(outputType);
            return true;
        }

        return new NotImplementedException($"Cannot cast from {inputType} to {outputType}");
    }

    public static Result TryEmitCast<TEmitter>(TEmitter emitter,
                                     Either<ParameterInfo, Type> input,
                                     Either<Type, ParameterInfo> output)
    where TEmitter : IOpCodeEmitter<TEmitter>
    {
        if (input.Is<ParameterInfo>(out var inputParameter))
        {
            if (output.Is<ParameterInfo>(out var outputParameter))
            {
                return TryEmitCast(emitter, inputParameter, outputParameter);
            }
            else
            {
                return TryEmitCast(emitter, inputParameter, (Type)output);
            }
        }
        else
        {
            if (output.Is<ParameterInfo>(out var outputParameter))
            {
                return TryEmitCast(emitter, (Type)input, outputParameter);
            }
            else
            {
                return TryEmitCast(emitter, (Type)input, (Type)output);
            }
        }
    }

    public static TEmitter LoadOrCast<TEmitter>(this TEmitter emitter,
                                              Either<ParameterInfo, Type> input,
                                              Either<Type, ParameterInfo> output)
        where TEmitter : IOpCodeEmitter<TEmitter>
    {
        var result = TryEmitCast(emitter, input, output);
        result.ThrowIfFailed();
        return emitter;
    }

    public static TEmitter LoadAs<TEmitter>(this TEmitter emitter,
                                            ParameterInfo input,
                                            Type output)
        where TEmitter : IOpCodeEmitter<TEmitter>
    {
        var result = TryEmitCast(emitter, input, output);
        result.ThrowIfFailed();
        return emitter;
    }

    public static TEmitter Cast<TEmitter>(this TEmitter emitter,
                                          Type input,
                                          Type output)
        where TEmitter : IOpCodeEmitter<TEmitter>
    {
        var result = TryEmitCast(emitter, input, output);
        result.ThrowIfFailed();
        return emitter;
    }
}