using System.Diagnostics;
using System.Reflection.Emit;

// ReSharper disable IdentifierTypo

namespace Jay.Reflection.Emission;

public interface IFluentEmitter<TEmitter> : IOpEmitter<TEmitter>
    where TEmitter : class, IFluentEmitter<TEmitter>
{
    TEmitter DefineAndMarkLabel(out Label label) => DefineLabel(out label).MarkLabel(label);

    TEmitter ThrowException(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);
        if (!exceptionType.Implements<Exception>())
            throw new ArgumentException("Invalid Exception Type", nameof(exceptionType));
        var ctor = exceptionType.GetConstructor(Reflect.InstanceFlags, Type.EmptyTypes);
        if (ctor is null)
        {
            return LoadUninitialized(exceptionType).Throw();
        }
        else
        {
            return Newobj(ctor).Throw();
        }
    }

    /// <summary>
    /// Emits the instructions to throw an <see cref="Exception"/>.
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/> to throw.</typeparam>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.throwexception?view=netcore-3.0"/>
    TEmitter ThrowException<TException>()
        where TException : Exception, new() => ThrowException(typeof(TException));

    TEmitter ThrowException<TException>(params object?[] ctorArgs)
        where TException : Exception
    {
        {
            var argTypes = ctorArgs.ToTypeArray();
            var ctor = EmitterHelpers.FindConstructors(typeof(TException), argTypes)
                                      .FirstOrDefault();
            throw new NotImplementedException();
        }
    }

    TEmitter Br(out Label label) => DefineLabel(out label).Br(label);
    TEmitter Leave(out Label label) => DefineLabel(out label).Leave(label);
    TEmitter Brtrue(out Label label) => DefineLabel(out label).Brtrue(label);
    TEmitter Brfalse(out Label label) => DefineLabel(out label).Brfalse(label);
    TEmitter Beq(out Label label) => DefineLabel(out label).Beq(label);
    TEmitter Bne_Un(out Label label) => DefineLabel(out label).Bne_Un(label);
    TEmitter Bge(out Label label) => DefineLabel(out label).Bge(label);
    TEmitter Bge_Un(out Label label) => DefineLabel(out label).Bge_Un(label);
    TEmitter Bgt(out Label label) => DefineLabel(out label).Bgt(label);
    TEmitter Bgt_Un(out Label label) => DefineLabel(out label).Bgt_Un(label);
    TEmitter Ble(out Label label) => DefineLabel(out label).Ble(label);
    TEmitter Ble_Un(out Label label) => DefineLabel(out label).Ble_Un(label);
    TEmitter Blt(out Label label) => DefineLabel(out label).Blt(label);
    TEmitter Blt_Un(out Label label) => DefineLabel(out label).Blt_Un(label);

    TEmitter Cge() => Clt().Not();
    TEmitter Cge_Un() => Clt_Un().Not();
    TEmitter Cle() => Cgt().Not();
    TEmitter Cle_Un() => Cgt_Un().Not();

    Result CanLoad(Type? type)
    {
        if (type is null)
            return true;
        if (type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte) ||
            type == typeof(short) || type == typeof(ushort) ||
            type == typeof(int) || type == typeof(uint) ||
            type == typeof(long) || type == typeof(ulong) ||
            type == typeof(float) || type == typeof(double) ||
            type == typeof(string) || type == typeof(Type)
           )
        {
            return true;
        }


        return false;
    }

    TEmitter Load<T>(T value)
    {
        if (value is null)
            return Ldnull();
        if (value is bool boolean)
            return boolean ? Ldc_I4_1() : Ldc_I4_0();
        if (value is byte b)
            return Ldc_I4(b);
        if (value is sbyte sb)
            return Ldc_I4(sb);
        if (value is short s)
            return Ldc_I4(s);
        if (value is ushort us)
            return Ldc_I4(us);
        if (value is int i)
            return Ldc_I4(i);
        if (value is uint ui)
            return Ldc_I8((long)ui);
        if (value is long l)
            return Ldc_I8(l);
        if (value is ulong ul)
            return Ldc_I8((long)ul);
        if (value is float f)
            return Ldc_R4(f);
        if (value is double d)
            return Ldc_R8(d);
        if (value is string str)
            return Ldstr(str);
        if (value is Type type)
            return LoadType(type);

        throw new NotImplementedException();
    }

    TEmitter LoadType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Ldtoken(type).Call(EmitterHelpers.TypeGetTypeFromHandleMethod.Value);
    }
    TEmitter LoadType<T>() => LoadType(typeof(T));

    TEmitter LoadUninitialized(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return LoadType(type)
               .Call(EmitterHelpers.RuntimeHelpersGetUninitializedObject.Value)
               .Cast(typeof(object), type);
    }
    TEmitter LoadUninitialized<T>() => LoadUninitialized(typeof(T));

    //Always the most up-to-date
    TEmitter LoadAs(ParameterInfo input, Type? outputType)
    {
        if (outputType is null || outputType == typeof(void))
        {
            // Output requires nothing, we have nothing loaded
            return (TEmitter)this;
        }

        if (outputType == typeof(void*))
        {
            throw new NotImplementedException("Does not support void*");
        }

        if (input.ParameterType == typeof(void))
        {
            // We're expecting a value that we're not given
            throw new ArgumentException("We're expecting a value that we're not given", nameof(input));
        }

        if (input.ParameterType == typeof(void*))
        {
            throw new NotImplementedException("Does not support void*");
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
                Ldarg(input.Position);
            }
            else if (!inputIsRef)
            {
                // out is ref
                // todo: safety shit
                Ldarga(input.Position);
            }
            else
            {
                Debug.Assert(!outputIsRef);
                // in is ref
                // todo: safety shit
                Ldarg(input.Position).Ldind(outputType);
            }
            return (TEmitter)this;
        }

        // Coming from object?
        if (inputType == typeof(object) && !inputIsRef)
        {
            if (outputType.IsValueType)
            {
                if (!outputIsRef)
                {
                    Ldarg(input.Position)
                           .Unbox_Any(outputType);
                }
                else
                {
                    Ldarg(input.Position)
                           .Unbox(outputType);
                }
            }
            else
            {
                if (!outputIsRef)
                {
                    Ldarg(input.Position)
                           .Castclass(outputType);
                }
                else
                {
                    throw new NotImplementedException("Non-default outputs are not yet supported");
                }
            }
            return (TEmitter)this; ;
        }

        if (inputIsRef)
        {
            throw new NotImplementedException("Non-default inputs are not yet supported");
        }

        if (outputIsRef)
        {
            throw new NotImplementedException("Non-default outputs are not yet supported");
        }

        // Going to object?
        if (outputType == typeof(object))
        {
            if (inputType.IsValueType)
            {
                Ldarg(input.Position)
                       .Box(inputType);
            }
            else
            {
                // Is already object
                Ldarg(input.Position);
            }
            return (TEmitter)this; ;
        }

        // Implements?
        // TODO: More advanced logic here?
        if (inputType.IsAssignableTo(outputType))
        {
            Debug.Assert(inputType.IsClass);
            Debug.Assert(outputType.IsClass);
            Ldarg(input.Position)
                   .Castclass(outputType);
            return (TEmitter)this; ;
        }

        throw new NotImplementedException($"Cannot cast from {input} to {outputType}");

    }

    TEmitter Cast(Type? inputType, Type? outputType)
    {
        if (outputType is null || outputType == typeof(void))
        {
            // Output requires nothing, we have nothing loaded
            return (TEmitter)this;
        }

        if (outputType == typeof(void*))
        {
            throw new NotImplementedException("Does not support void*");
        }

        if (inputType is null || inputType == typeof(void))
        {
            // We're expecting a value that we're not given
            throw new ArgumentException("We're expecting a value that we're not given", nameof(inputType));
        }

        if (inputType == typeof(void*))
        {
            throw new NotImplementedException("Does not support void*");
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
                throw new NotImplementedException("Non-default outputs are not yet supported");
            }
            else
            {
                Debug.Assert(!outputIsRef);
                // in is ref
                // todo: safety shit
                Ldind(outputType);
            }
            return (TEmitter)this;
        }

        // Coming from object?
        if (inputType == typeof(object) && !inputIsRef)
        {
            if (outputType.IsValueType)
            {
                if (!outputIsRef)
                {
                    Unbox_Any(outputType);
                }
                else
                {
                    Unbox(outputType);
                }
            }
            else
            {
                if (!outputIsRef)
                {
                    Castclass(outputType);
                }
                else
                {
                    throw new NotImplementedException("Non-default outputs are not yet supported");
                }
            }
            return (TEmitter)this;
        }

        if (inputIsRef)
        {
            throw new NotImplementedException("Non-default inputs are not yet supported");
        }

        if (outputIsRef)
        {
            throw new NotImplementedException("Non-default outputs are not yet supported");
        }

        // Going to object?
        if (outputType == typeof(object))
        {
            if (inputType.IsValueType)
            {
                Box(inputType);
            }
            else
            {
                // Is already object
            }
            return (TEmitter)this;
        }

        // Implements?
        // TODO: More advanced logic here?
        if (inputType.IsAssignableTo(outputType))
        {
            Debug.Assert(inputType.IsClass);
            Debug.Assert(outputType.IsClass);
            Castclass(outputType);
            return (TEmitter)this;
        }

        throw new NotImplementedException($"Cannot cast from {inputType} to {outputType}");
    }

    TEmitter LoadParams(ParameterInfo paramsParameter, ParameterInfo[] parameters)
    {
        if (paramsParameter is null)
            throw new ArgumentNullException(nameof(paramsParameter));
        if (/*!paramsParameter.IsParams() || */paramsParameter.ParameterType != typeof(object[]))
            throw new ArgumentException("Parameter is not params", nameof(paramsParameter));
        if (parameters is null)
            throw new ArgumentNullException(nameof(parameters));
        var count = parameters.Length;

        DefineLabel(out Label lblOk)
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
            Ldarg(paramsParameter.Position)
                   // Load element index
                   .Ldc_I4(i);
            var parameter = parameters[i];
            var access = parameter.GetAccess(out var parameterType);
            // TODO: Test this!
            if (access == ParameterInfoExtensions.Access.Default)
            {
                // Load the element
                Ldelem(parameterType);
            }
            else
            {
                // TODO: Safety checks
                // Load the element reference
                Ldelema(parameterType);
            }
        }
        // All params are loaded in order with nothing extra laying on the stack
        return (TEmitter)this;
    }


    TEmitter LoadDefault(Type type)
    {
        if (type.IsValueType)
        {
            return DeclareLocal(type, out var def)
                   .Ldloca(def)
                   .Initobj(type)
                   .Ldloc(def);
        }
        return Ldnull();
    }
    TEmitter LoadDefault<T>() => LoadDefault(typeof(T));


  

    TEmitter LoadInstance(ParameterInfo? possibleInstanceParameter,
                          MemberInfo member,
                          out int offset)
    {
        // Assume offset 0 for fast return
        offset = 0;

        // Static method?
        if (member.IsStatic())
        {
            // Null possible is okay
            if (possibleInstanceParameter is null)
                return (TEmitter)this;

            // Fast get actual instance type minus in/out/ref
            possibleInstanceParameter.GetAccess(out var instanceType);

            // Look for a throwaway instance type
            // TODO: Allow for throwaway object/Type   [null, typeof(member.OwnerType()]
            if (instanceType == typeof(Static) || instanceType == typeof(VOID) ||
                instanceType == typeof(void) || instanceType == member.OwnerType())
            {
                // This is a throwaway
                offset = 1;
                return (TEmitter)this;
            }

            // Assume there is no throwaway
            return (TEmitter)this;
        }
        else
        {
            if (possibleInstanceParameter is null)
                throw new ArgumentNullException(nameof(possibleInstanceParameter));

            member.TryGetInstanceType(out var methodInstanceType).ThrowIfFailed();
            LoadAs(possibleInstanceParameter, methodInstanceType!);

            // We loaded the instance, the rest of the parameters are used
            offset = 1;
            return (TEmitter)this;
        }

    }
}