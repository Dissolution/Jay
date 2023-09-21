using System.Diagnostics;
using Jay.Collections;
using Jay.Reflection.Builders;
using Jay.Reflection.Emitting;
using Jay.Reflection.Emitting.Args;
using Jay.Reflection.Searching;
using Jay.Reflection.Utilities;
using Jay.Utilities;

namespace Jay.Reflection.Cloning;

public static class Cloner
{
    private static readonly MethodInfo _scaryGetUninitializedObjectMethod;
    private static readonly MethodInfo _clonerDeepCloneGenericMethod;

    private static readonly ConcurrentTypeMap<Delegate> _valueDeepCloneCache = new();
    private static readonly ConcurrentTypeMap<DeepClone<object?>> _objectDeepCloneCache = new();
    
    static Cloner()
    {
        _scaryGetUninitializedObjectMethod = MemberSearch.One<MethodInfo>(
            typeof(Scary),
            new()
            {
                Name = nameof(Scary.GetUninitializedObject),
                GenericTypeCount = 0,
                ParameterTypes = new Type[1]
                {
                    typeof(Type)
                },
                ReturnType = typeof(object),
            });
        _clonerDeepCloneGenericMethod = MemberSearch.One<MethodInfo>(
            typeof(Cloner),
            new()
            {
                Name = nameof(Cloner.DeepClone),
                GenericTypeCount = 1,
            });
        _valueDeepCloneCache.TryAdd(typeof(object), ObjectDeepClone);
        _objectDeepCloneCache.TryAdd(typeof(object), ObjectDeepClone);
    }
    
    private static Delegate CreateValueDeepClone(Type type)
    {
        // First, we create DeepClone<T>
        var builder = RuntimeBuilder.CreateRuntimeDelegateBuilder(
            typeof(DeepClone<>).MakeGenericType(type),
            $"deepclone_{type.FullName}");
        var emitter = builder.Emitter;

        /* For values that do not contain any references, enums, and strings,
         * we can just load the value and return it, since they are all by-value
         * as they move
         */
        if (!TypeHelper.IsReferenceOrContainsReferences(type))
        {
            emitter.Ldarg_0()
                .Ret();
            return builder.CreateDelegate();
        }

        /* For everything else, we're going to clone every single field.
         * This covers every possible way that an instance can store value.
         *
         * We have to start with an empty clone to fill.
         * We can NOT call new() as that may have initialization effects
         * Instead, we use an uninitialized value (basically just allocates memory for us)
         */

        // A place to store the clone
        emitter.DeclareLocal(type, out var clone)
            .LoadType(type)
            .Call(_scaryGetUninitializedObjectMethod)
            .EmitCast(typeof(object), type)
            .Stloc(clone);
        // Clone every field
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#if DEBUG
        if (fields.Length == 0)
            Debugger.Break();
#endif
        foreach (var field in fields)
        {
            emitter
                .LoadInstanceFor(clone, field)
                .Ldarg_0()
                .Ldfld(field)
                .Call(_clonerDeepCloneGenericMethod.MakeGenericMethod(field.FieldType))
                .Stfld(field);
        }
        // done
        emitter.Ldloc(clone)
            .Ret();
        return builder.CreateDelegate();
    }

    private static DeepClone<object?> CreateObjectDeepClone(Type type)
    {
        var deepClone = RuntimeBuilder.EmitDelegate<DeepClone<object?>>(
            $"deepclone_object_{type.FullName}",
            emitter => emitter
                .Ldarg_0()
                .EmitCast(typeof(object), type)
                .Call(_clonerDeepCloneGenericMethod.MakeGenericMethod(type))
                .EmitCast(type, typeof(object))
                .Ret());
        return deepClone;
    }

    private static DeepClone<T> GetDeepClone<T>()
    {
        return _valueDeepCloneCache
            .GetOrAdd<T>(static t => CreateValueDeepClone(t))
            .AsValid<DeepClone<T>>();
    }

    private static DeepClone<object?> GetDeepClone(Type type)
    {
        return _objectDeepCloneCache
            .GetOrAdd(type, static t => CreateObjectDeepClone(t));
    }

    [return: NotNullIfNotNull(nameof(obj))]
    private static object? ObjectDeepClone(object? obj)
    {
        if (obj is null)
            return null;
        var type = obj.GetType();
        if (type == typeof(object))
            return new object();
        return GetDeepClone(type)(obj);
    }
    
    [return: NotNullIfNotNull(nameof(value))]
    public static T? DeepClone<T>(T? value)
    {
        // fast null return
        if (value is null)
            return default;
        return GetDeepClone<T>()(value);
    }
    
}
