using System.Diagnostics;
using Jay.Collections;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Caching;
using Jay.Reflection.Extensions;

namespace Jay.Reflection.Cloning;

public static class Cloner
{
    private static readonly ConcurrentTypeDictionary<Delegate> _valueCloneCache;
    private static readonly ConcurrentTypeDictionary<DeepClone<object?>> _objectCloneCache;
    
    static Cloner()
    {
        _valueCloneCache = new()
        {
            [typeof(string)] = (DeepClone<string>)FastClone,
        };
        _objectCloneCache = new()
        {
            [typeof(object)] = (DeepClone<object?>)ObjectClone,
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T FastClone<T>(T value) => value;

    [return: NotNullIfNotNull(nameof(obj))]
    private static object? ObjectClone(object? obj)
    {
        if (obj is null) return null;
        var type = obj.GetType();
        return GetObjectDeepClone(type).Invoke(obj);
    }
    
   
    internal static DeepClone<T> GetDeepClone<T>()
    {
        return (_deepCloneCache.GetOrAdd<T>(CreateDeepClone) as DeepClone<T>)!;
    }
    

    public static T[] DeepClone1DArray<T>(T[] array)
    {
        int len = array.Length;
        T[] clone = new T[len];
        var deepClone = GetDeepClone<T>();
        for (int i = 0; i < len; i++)
        {
            clone[i] = deepClone(array[i]);
        }
        return clone;
    }
    public static T[,] DeepClone2DArray<T>(T[,] array)
    {
        int arrayLen0 = array.GetLength(0);
        int arrayLen1 = array.GetLength(1);
        T[,] clone = new T[arrayLen0, arrayLen1];
        var deepClone = GetDeepClone<T>();
        for (var x = 0; x < array.GetLength(0); x++)
        {
            for (var y = 0; y < array.GetLength(1); y++)
            {
                clone[x, y] = deepClone(array[x, y]);
            }
        }
        return clone;
    }

    [return: NotNullIfNotNull(nameof(array))]
    public static Array? DeepCloneArray(Array? array)
    {
        if (array is null) return null;
        var arrayWrapper = new ArrayWrapper(array);
        Array clone = Array.CreateInstance(arrayWrapper.ElementType, arrayWrapper.RankLengths, arrayWrapper.LowerBounds);
        var cloner = GetObjectDeepClone(arrayWrapper.ElementType);
        var cloneWrapper = new ArrayWrapper(clone);
        using var e = arrayWrapper.GetEnumerator();
        while (e.MoveNext())
        {
            int[] index = e.Indices;
            cloneWrapper.SetValue(index, cloner(e.Current!));
        }
        return clone;
    }



    private static Delegate CreateDeepClone(Type type)
    {
        var builder = RuntimeBuilder.CreateRuntimeDelegateBuilder(
            typeof(DeepClone<>).MakeGenericType(type),
            Dump($"clone_{type}"));
        var emitter = builder.Emitter;

        /*
        // Null check for non-value types
        if (!type.IsValueType)
        {
            emitter.Ldarg(0)
                .Ldind(type)
                .Brfalse(out var notNull)
                .Ldnull()
                .Ret()
                .MarkLabel(notNull);
        }
        */

        // unmanaged or string we just dup + return
        if (type == typeof(string) || type.IsUnmanaged())
        {
            emitter.Ldarga(0).Ldind(type).Ret();
        }
        // Special Array handling
        else if (type.IsArray)
        {
            emitter.Ldarg(0)
                .EmitCast(type, typeof(Array))
                .Call(Searching.MemberSearch.FindMethod(typeof(Cloner),
                    new(nameof(DeepCloneArray), Visibility.Public | Visibility.Static, typeof(Array), typeof(Array))))
                .EmitCast(typeof(Array), type)
                .Ret();
        }
        // Everything else has some sort of reference down the chain
        else
        {
            // Create a raw value
            emitter.DeclareLocal(type, out var copy);

            if (type.IsValueType)
            {
                // init the copy, we'll clone the fields
                emitter.Ldloca(copy)
                    .Initobj(type);

                // copy each instance field
                var fields = type.GetFields(Reflect.Flags.Instance);
                foreach (var field in fields)
                {
                    if (field.FieldType.Implements<FieldInfo>())
                        Debugger.Break();

                    emitter.Ldloca(copy)
                        .Ldarg(0)
                        .Ldfld(field)
                        .Call(GetDeepCloneMethod(field.FieldType))
                        .Stfld(field);
                }
            }
            else
            {
                // Uninitialized object
                // We don't want to call the constructor, that may have side effects
                emitter.LoadType(type)
                    .Call(MemberCache.Methods.RuntimeHelpers_GetUninitializedObject)
                    .Unbox_Any(type)
                    .Stloc(copy);

                // copy each instance field
                var fields = type.GetFields(Reflect.Flags.Instance);
                foreach (var field in fields)
                {
                    var fieldDeepClone = GetDeepCloneMethod(field.FieldType);
                    emitter.Ldloc(copy)
                        .Ldarg(0)
                        .Ldfld(field)
                        .Call(fieldDeepClone)
                        .Stfld(field);
                }
            }

            // Load our clone and return!
            emitter.Ldloc(copy)
                .Ret();
        }

        return builder.CreateDelegate();
    }


  

    [return: NotNullIfNotNull(nameof(value))]
    public static T DeepClone<T>(this T value)
    {
        if (value is null) return default!;
        return GetDeepClone<T>().Invoke(value);
    }

    private static MethodInfo GetDeepCloneMethod(Type type)
    {
        return Searching.MemberSearch.FindMethod(typeof(Cloner),
                new(
                    nameof(DeepClone),
                    Visibility.Public | Visibility.Static))
            .MakeGenericMethod(type);
    }

    private static DeepClone<object> CreateObjectClone(Type type)
    {
        if (type == typeof(object)) // prevent recursion
            return FastClone<object>;

        return RuntimeBuilder.CreateDelegate<DeepClone<object>>(
            Dump($"clone_object_{type}"),
            emitter => emitter
                .Ldarg(0)
                .Unbox_Any(type)
                .Call(GetDeepCloneMethod(type))
                .Box(type)
                .Ret());
    }

    internal static DeepClone<object> GetObjectDeepClone(Type objectType)
    {
        return _objectCloneCache.GetOrAdd(objectType, CreateObjectClone);
    }

   
}