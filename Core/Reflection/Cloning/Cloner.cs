using System.Diagnostics;
using System.Reflection;
using Jay.Collections;
using Jay.Dumping;
using Jay.Reflection.Building;
using Jay.Reflection.Caching;
using Jay.Reflection.Internal;
using Jay.Reflection.Search;
using Jay.Validation;

namespace Jay.Reflection.Cloning;

/*public static class Cloner
{
    public static T[] Clone<T>(in T[] array)
    {
        int len = array.Length;
        T[] clone = new T[len];
        for (int i = 0; i < len; i++)
        {
            clone[i] = Clone<T>(in array[i])!;
        }

        return clone;
    }


    [return: NotNullIfNotNull("value")]
    public delegate T CloneDelegate<T>(in T value);

    private static readonly DelegateMemberCache _cloneDelegateCache;

    static Cloner()
    {
        _cloneDelegateCache = new DelegateMemberCache();
        _cloneILCache = new();
    }

    private static readonly ConcurrentTypeDictionary<Action<IILGeneratorEmitter>> _cloneILCache;

    private static void EmitCloneIL(IILGeneratorEmitter emitter, Type? type)
    {
        if (type is null)
        {
            CloneNull(emitter);
        }
        else
        {
            var emission = _cloneILCache.GetOrAdd(type, CreateCloneEmission);
            emission(emitter);
        }
    }

    private static Action<IILGeneratorEmitter> GetCloneEmission(Type? type)
    {
        if (type is null)
        {
            return CloneNull;
        }
        else
        {
            return _cloneILCache.GetOrAdd(type, CreateCloneEmission);
        }
    }

    private static void CloneNull(IILGeneratorEmitter emitter) => emitter.Ldnull();

    private record Source(Action<IILGeneratorEmitter> LoadValue, Action<IILGeneratorEmitter> LoadValueAddress);

    

    // We are either working with Ldarg(a) or Ldfld(a)
    private static void EmitClone(IILGeneratorEmitter emitter, Type? type, Source source)
    {
        // null is probably (object?)null
        if (type == null)
        {
            // do nothing
            return;
        }

        // string and true value types are always copied as we move them around
        if (type == typeof(string) || MethodInfoCache.IsNonReferenced(type))
        {
            source.LoadValue(emitter);
            return;
        }

        // array
        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var elementCloneEmit = GetCloneEmission(elementType);
            var rank = type.GetArrayRank();
            if (rank == 1)
            {
                // The clone we have to write to
                emitter.DeclareLocal(type, out var clone)
                       .DeclareLocal<int>(out var i)
                       .DeclareLocal<int>(out var len)

                       // Load and store len
                       .Ldarg(0)
                       .Ldind_Ref()
                       .Ldlen()
                       .Conv_I4()
                       .Stloc(len)

                       // Create clone = new T[len]
                       .Ldloc(len)
                       .Newarr(elementType)
                       .Stloc(clone)

                       // i = 0
                       .Ldc_I4(0)
                       .Stloc(i)

                       // Goto Check
                       .Br(out var lblLoopCheck)

                       // Start of loop
                       .DefineAndMarkLabel(out var lblLoopStart)

                       // Setup clone[i] =
                       .Ldloc(clone)
                       .Ldloc(i)
                       // in T[i]
                       .Ldarg(0)
                       .Ldind_Ref()
                       .Ldloc(i)
                       .Readonly()
                       .Ldelema(elementType);
                // Emit the code to clone the element
                elementCloneEmit(emitter);
                // Store in clone[i]
                emitter.Stelem(elementType)

                       // i++
                       .Ldloc(i)
                       .Ldc_I4(1)
                       .Add()
                       .Stloc(i)

                       // check that i < len
                       .MarkLabel(lblLoopCheck)
                       .Ldloc(i)
                       .Ldloc(len)
                       .Blt(lblLoopStart)

                       // Load clone and done
                       .Ldloc(clone);
                ;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        // At this point, we have to construct and set fields
        return emitter =>
        {
            // First, we have to create clone
            emitter.DeclareLocal(type, out var clone);
            
            if (type.IsValueType)
            {
                emitter.Ldloca(clone)
                       .Initobj(type);
            }
            else if (type.HasDefaultConstructor(out var ctor))
            {
                emitter.Newobj(ctor)
                       .Stloc(clone);
            }
            else
            {
                emitter.LoadUninitialized(type)
                       .Stloc(clone);
            }

            // Copy each instance field
            var fields = type.GetFields(Reflect.InstanceFlags);
            foreach (var field in fields)
            {
                // ref clone (to set field value of)
                if (type.IsValueType)
                {
                    emitter.Ldloca(clone);
                }
                else
                {
                    emitter.Ldloc(clone);
                }
                // Load 
                emitter.Ldflda(field)
                       // Clone it using Clone<T> so it will cache
                       .Call(GetCloneMethod(field.FieldType))
                       // Set the clone's field value to the cloned value
                       .Stfld(field);
            }
            // Done
            emitter.Ret();
        }
        
    }
                

    private static CloneDelegate<T> CreateCloneDelegate<T>(Type type)
    {
        var dm = RuntimeBuilder.CreateDynamicMethod<CloneDelegate<T>>(Dumper.Dump($"clone_{type}"));
        var emitter = dm.Emitter;
        // Start with a blank clone
        emitter.DeclareLocal<T>(out var clone);

        // String is special
        if (type == typeof(string))
        {
            emitter.Ldarg(0)
                   .Ldind<string>()
                   .Ret();
        }
        // Special Array handler
        else if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var elementCloneMethod = GetCloneMethod(elementType);
            var rank = type.GetArrayRank();
            if (rank == 1)
            {
                emitter.DefineLabel(out var lblStart)
                       .DefineLabel(out var lblCheck);

                emitter.Ldarg(0)
                       .Ldind(type)
                       .Ldlen()
                       .DeclareLocal<int>(out var len)
                       .Stloc(len)
                       .Ldloc(len)
                       .Newarr(elementType)
                       .Stloc(clone);

                // int i = 0
                emitter.DeclareLocal<int>(out var i)
                       .Ldc_I4_0()
                       .Stloc(i);

                emitter.Br(lblCheck);

                // Start of loop
                emitter.MarkLabel(lblStart)
                       .Ldloca(clone)
                       .Ldloc(i)
                       .Ldarg(0)
                       .Ldloc(i)
                       .Ldelema(elementType)
                       .Call(elementCloneMethod)
                       .Stelem(elementType);
                // i++
                emitter.Ldloc(i)
                       .Ldc_I4_1()
                       .Add()
                       .Stloc(i);

                // While i < array.Len
                emitter.MarkLabel(lblCheck)
                       .Ldloc(i)
                       .Ldloc(len)
                       .Clt()
                       .Brtrue(lblStart);
                // We've loaded them all
                emitter.Ret();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        // Non-easy clone?
        else if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            if (type.IsValueType)
            {
                emitter.Ldloca(clone)
                       .Initobj(type);

            }
            else if (type.HasDefaultConstructor(out var ctor))
            {
                emitter.Newobj(ctor)
                       .Stloc(clone);
            }
            else
            {
                emitter.LoadUninitialized(type)
                       .Stloc(clone);
            }

            // Copy each field in turn
            var fields = type.GetFields(Reflect.InstanceFlags);
            foreach (var field in fields)
            {
                // Get the clone we'll be setting this field value of
                if (type.IsValueType)
                {
                    emitter.Ldloca(clone);
                }
                else
                {
                    emitter.Ldloc(clone);
                }
                // Load the original value's field's value
                emitter.LoadInstanceFor(field, dm.Parameters[0], out int offset)
                       .Assert(() => offset == 1)
                       .Ldflda(field)
                       // Clone it using Clone<T> so it will cache
                       .Call(GetCloneMethod(field.FieldType))
                       // Set the clone's field value to the cloned value
                       .Stfld(field);
            }
            // Done
            emitter.Ret();
        }
        else
        {
            // Plain value type, will be copied anyways
            emitter.Ldarg(0)
                   .Ldind<T>()
                   .Ret();
        }
        return dm.CreateDelegate();
    }

    private static MethodInfo GetCloneMethod(Type type)
    {
        return typeof(Cloner).GetMethod(nameof(Clone), BindingFlags.Public | BindingFlags.Static)
                             .ThrowIfNull()
                             .MakeGenericMethod(type);
    }


    public static CloneDelegate<T> CreateCloneDelegate<T>() => CreateCloneDelegate<T>(typeof(T));

    [return: NotNullIfNotNull("value")]
    public static T? Clone<T>(in T? value)
    {
        if (value is null) return default;
        var del = _cloneDelegateCache.GetOrAdd(typeof(T), type => CreateCloneDelegate<T>(type));
        return del(in value);
    }
}*/



public static class Muq
{
    [return: NotNullIfNotNull("value")]
    private delegate T? CloneValue<T>(T? value);

    private static readonly ConcurrentTypeDictionary<Delegate> _typeCloneCache;

    static Muq()
    {
        _typeCloneCache = new();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CloneValue<T> GetCloneDelegate<T>()
    {
        return (_typeCloneCache.GetOrAdd(typeof(T), CreateCloneDelegate)
            as CloneValue<T>)!;
    }

    private static MethodInfo Muq_GetCloneMethod =
        typeof(Muq).GetMethod(nameof(GetCloneMethod),
            Reflect.StaticFlags,
            new Type[1] { typeof(Type) })!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static MethodInfo GetCloneMethod(Type type)
    {
        return typeof(Muq).GetMethod(nameof(Muq.Clone), BindingFlags.Public | BindingFlags.Static)
                          .ThrowIfNull()
                          .MakeGenericMethod(type)
                          .ThrowIfNull();
    }

    [return: NotNullIfNotNull("value")]
    public static T? Clone<T>(T? value)
    {
        if (value is null) return default;
        var del = _typeCloneCache.GetOrAdd(typeof(T), CreateCloneDelegate);
        CloneValue<T> cloner = (del as CloneValue<T>)!;
        if (cloner is null)
        {
            Debugger.Break();
        }

        return cloner!(value);
    }

    private static Delegate CreateCloneDelegate(Type type)
    {
        if (type.IsByRef || type.IsPointer)
            throw new NotImplementedException();

        var delType = typeof(CloneValue<>).MakeGenericType(type);

        var dm = RuntimeBuilder.CreateDynamicMethod(MethodSig.Of(delType), Dumper.Dump($"{type}_clone"));
            
        var emitter = dm.GetILEmitter();

        // Object we need to extract the value inside
        if (type == typeof(object))
        {
            emitter.Ldarg(0)
                   .Ldarg(0)
                   .Call(typeof(object).GetMethod(nameof(object.GetType),
                                                  Reflect.InstanceFlags,
                                                  Type.EmptyTypes)!)
                   .Call(Muq_GetCloneMethod)
                   .Call(Muq_GetCloneMethod);
            throw new NotImplementedException();

        }

        // String and true Value types are always copied as we ref them
        if (type == typeof(string) || MethodInfoCache.IsNonReferenced(type))
        {
            emitter.Ldarg(0).Ret();
        }
        else if (type.IsArray)
        {
            emitter.DeclareLocal(type, out var clone);
            var elementType = type.GetElementType()!;
            var elementCloneMethod = GetCloneMethod(elementType);
            var rank = type.GetArrayRank();
            if (rank == 1)
            {
                emitter.DefineLabel(out var lblStart)
                       .DefineLabel(out var lblCheck);

                emitter.Ldarg(0)
                       .Ldlen()
                       .DeclareLocal<int>(out var len)
                       .Stloc(len)
                       .Ldloc(len)
                       .Newarr(elementType)
                       .Stloc(clone);

                // int i = 0
                emitter.DeclareLocal<int>(out var i)
                       .Ldc_I4_0()
                       .Stloc(i);

                emitter.Br(lblCheck);

                // Start of loop
                emitter.MarkLabel(lblStart)
                       .Ldloc(clone)
                       .Ldloc(i)
                       .Ldarg(0)
                       .Ldloc(i)
                       .Ldelem(elementType)
                       .Call(elementCloneMethod)
                       .Stelem(elementType);
                // i++
                emitter.Ldloc(i)
                       .Ldc_I4_1()
                       .Add()
                       .Stloc(i);

                // While i < array.Len
                emitter.MarkLabel(lblCheck)
                       .Ldloc(i)
                       .Ldloc(len)
                       .Clt()
                       .Brtrue(lblStart);
                // We've loaded them all
                emitter.Ldloc(clone)
                    .Ret();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        else
        {
            emitter.DeclareLocal(type, out var clone);

            if (type.IsValueType)
            {
                emitter.Ldloca(clone)
                       .Initobj(type);
            }
            else if (type.HasDefaultConstructor(out var ctor))
            {
                emitter.Newobj(ctor)
                       .Stloc(clone);
            }
            else
            {
                emitter.LoadUninitialized(type)
                       .Stloc(clone);
            }

            var fields = type.GetFields(Reflect.InstanceFlags);
            Debug.Assert(fields.Length > 0);
            foreach (var field in fields)
            {
                var fieldCloneMethod = GetCloneMethod(field.FieldType);
                if (type.IsValueType)
                {
                    emitter.Ldloca(clone);
                }
                else
                {
                    emitter.Ldloc(clone);
                }
                emitter.Ldarg(0)
                       .Ldfld(field)
                       .Call(fieldCloneMethod)
                       .Stfld(field);
            }
            emitter.Ldloc(clone)
                   .Ret();
        }
        return dm.CreateDelegate(delType);
    }
}