using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Adapting;
using Jay.Reflection.Search;
using Jay.Validation;

namespace Jay.Reflection.Cloning;

public static class Cloner
{
    [return: NotNullIfNotNull("value")]
    public delegate T CloneDelegate<T>(in T value);

    private static readonly DelegateMemberCache _cloneDelegateCache;

    static Cloner()
    {
        _cloneDelegateCache = new DelegateMemberCache();
    }


    private static CloneDelegate<T> CreateCloneDelegate<T>(Type type)
    {
        var dm = RuntimeBuilder.CreateDynamicMethod<CloneDelegate<T>>($"clone_{type.Name}");
        var emitter = dm.Emitter;
        // Start with a blank clone
        emitter.DeclareLocal<T>(out var clone);

        // Special Array handler
        if (type.IsArray)
        {
            var elementType = type.GetElementType().ThrowIfNull();
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
                emitter.LoadInstanceFor(dm.Parameters[0], field, out int offset)
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
    public static T Clone<T>(in T value)
    {
        if (value is null) return default;
        var del = _cloneDelegateCache.GetOrAdd(typeof(T), type => CreateCloneDelegate<T>(type));
        return del(in value);
    }
}