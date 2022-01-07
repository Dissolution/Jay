using System.Diagnostics;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission;

internal static class EmitterCastExtensions
{
    //Always the most up-to-date
    public static Result TryLoadAs<TEmitter>(this TEmitter emitter,
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
   
    public static Result TryCast<TEmitter>(this TEmitter emitter,
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

    public static TEmitter LoadAs<TEmitter>(this TEmitter emitter,
                                            ParameterInfo input,
                                            Type output)
        where TEmitter : IOpCodeEmitter<TEmitter>
    {
        var result = TryLoadAs(emitter, input, output);
        result.ThrowIfFailed();
        return emitter;
    }

    public static TEmitter Cast<TEmitter>(this TEmitter emitter,
                                          Type input,
                                          Type output)
        where TEmitter : IOpCodeEmitter<TEmitter>
    {
        var result = TryCast(emitter, input, output);
        result.ThrowIfFailed();
        return emitter;
    }

    public static Result TryLoadParams<TEmitter>(this TEmitter emitter,
                                                 ParameterInfo paramsParameter,
                                                 ParameterInfo[] parameters)
        where TEmitter : IFullEmitter<TEmitter>
    {
        if (emitter is null)
            return new ArgumentNullException(nameof(emitter));
        if (paramsParameter is null)
            return new ArgumentNullException(nameof(paramsParameter));
        if (/*!paramsParameter.IsParams() || */paramsParameter.ParameterType != typeof(object[]))
            return new ArgumentException("Parameter is not params", nameof(paramsParameter));
        if (parameters is null)
            return new ArgumentNullException(nameof(parameters));
        var count = parameters.Length;

        emitter.DefineLabel(out Label lblOk)
               // Load the params value (object[])
               .Ldarg(paramsParameter.Position)
               // Load its Length
               .Ldlen()
               // Check that it is equal to the number of parameters we have to fill
               .Ldc_I4(count)
               .Beq(lblOk)
               // They weren't, throw
               //TODO: Build better thrower
               .ThrowException<InvalidOperationException>()
               .MarkLabel(lblOk);
        // Load each item in turn and cast it to the parameter
        for (var i = 0; i < count; i++)
        {
            // Load object[]
            emitter.Ldarg(paramsParameter.Position)
                   // Load element index
                   .Ldc_I4(i);
            var parameter = parameters[i];
            var access = parameter.GetAccess(out var parameterType);
            // TODO: Test this!
            if (access == ParameterInfoExtensions.Access.Default)
            {
                // Load the element
                emitter.Ldelem(parameterType);
            }
            else
            {
                // TODO: Safety checks
                // Load the element reference
                emitter.Ldelema(parameterType);
            }
        }
        // All params are loaded in order with nothing extra laying on the stack
        return true;
    }

    public static TEmitter LoadParams<TEmitter>(this TEmitter emitter,
                                                ParameterInfo paramsParameter,
                                                ParameterInfo[] parameters)
        where TEmitter : IFullEmitter<TEmitter>
    {
        var result = TryLoadParams(emitter, paramsParameter, parameters);
        result.ThrowIfFailed();
        return emitter;
    }

    public static TEmitter LoadDefault<TEmitter>(this TEmitter emitter,
                                                 Type type)
        where TEmitter : IFullEmitter<TEmitter>
    {
        if (type.IsValueType)
        {
            emitter.DeclareLocal(type, out var def)
                   .Ldloca(def)
                   .Initobj(type)
                   .Ldloc(def);
        }
        else
        {
            emitter.Ldnull();
        }

        return emitter;
    }

    public static TEmitter LoadDefault<TEmitter, T>(this TEmitter emitter)
        where TEmitter : IFullEmitter<TEmitter>
        => LoadDefault<TEmitter>(emitter, typeof(T));

    internal static bool IsObjectArray(this ParameterInfo parameter)
    {
        return !parameter.IsIn &&
               !parameter.IsOut &&
               parameter.ParameterType == typeof(object[]);
    }
    internal static bool IsObjectArray(this Type type)
    {
        return !type.IsByRef &&
               type == typeof(object[]);
    }

    public static Result TryLoadInstance<TEmitter>(this TEmitter emitter,
                                                   ParameterInfo? possibleInstanceParameter,
                                                   MemberInfo member,
                                                   out int offset)
        where TEmitter : IFullEmitter<TEmitter>
    {
        // Assume offset 0 for fast return
        offset = 0;

        // Static method?
        if (member.IsStatic())
        {
            // Null possible is okay
            if (possibleInstanceParameter is null)
                return true;

            // Fast get actual instance type minus in/out/ref
            possibleInstanceParameter.GetAccess(out var instanceType);
           
            // Look for a throwaway instance type
            // TODO: Allow for throwaway object/Type   [null, typeof(member.OwnerType()]
            if (instanceType == typeof(Static) || instanceType == typeof(VOID) || 
                instanceType == typeof(void) || instanceType == member.OwnerType())
            {
                // This is a throwaway
                offset = 1;
                return true;
            }

            // Assume there is no throwaway
            return true;
        }
        else
        {
            if (possibleInstanceParameter is null)
                return new ArgumentNullException(nameof(possibleInstanceParameter));

            Result result = member.TryGetInstanceType(out var methodInstanceType);
            if (!result) return result;

            result = emitter.TryLoadAs(possibleInstanceParameter, methodInstanceType!);
            if (!result) return result;

            // We loaded the instance, the rest of the parameters are used
            offset = 1;
            return true;
        }

    }

    private static readonly Lazy<MethodInfo> _typeGetTypeFromHandleMethod
        = new Lazy<MethodInfo>(() => typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle),
                                                            BindingFlags.Public | BindingFlags.Static)!);

    public static TEmitter LoadType<TEmitter>(this TEmitter emitter,
                                              Type type)
        where TEmitter : IFullEmitter<TEmitter>
    {
        return emitter.Ldtoken(type)
               .Call(_typeGetTypeFromHandleMethod.Value);

    }
}