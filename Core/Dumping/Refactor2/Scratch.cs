using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using InlineIL;
using Jay.Collections;
using Jay.Expressions;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Emission;
using Jay.Text;
using Jay.Validation;

namespace Jay.Dumping.Refactor2;

public static class Scratch
{
    static Scratch()
    {
        var thing = new ConcurrentStack<int>();
    }
}

public delegate void ValueDump<in T>(T? value, TextBuilder text);

public interface IDumpable
{
    void DumpTo(TextBuilder text);

    string Dump()
    {
        using var text = TextBuilder.Borrow();
        DumpTo(text);
        return text.ToString();
    }
}

public static partial class Dump_Cache
{
    private static Delegate CreateDumpNull(Type valueType)
    {
        return RuntimeBuilder.CreateDelegate(typeof(ValueDump<>).MakeGenericType(valueType),
            runtimeMethod =>
            {
                var emitter = runtimeMethod.Emitter;
                emitter.Ldarg_1()
                    .Ldstr("null")
                    .Call(TextBuilderReflections.WriteString)
                    .Ret();
            });
    }

    private static MethodInfo _objectGetTypeMethod = Reflect.On<object>().Get<MethodInfo>(obj => obj.GetType());


    private static T UnboxOrCastClass<T>(object obj)
    {
        IL.Emit.Ldarg(nameof(obj));
        IL.Emit.Unbox_Any<T>();
        return IL.Return<T>();
    }
    
    private static ValueDump<object> CreateObjectDump()
    {
        return RuntimeBuilder.CreateDelegate<ValueDump<object>>("Dump_Object",
            runtimeMethod =>
            {
                var emitter = runtimeMethod.Emitter;
                var valueArg = runtimeMethod.Parameters[0];
                var textBuilderArg = runtimeMethod.Parameters[1];

                /* Roughly:
                 * if (obj is null)
                 *      text.Write("null");
                 *      return;                 
                 * GetDumpValueDelegate(obj.GetType())((T)obj, text)
                 */
                
                // Null check
                emitter.Ldarg(valueArg)
                    .Brtrue(out var lblNotNull)
                    .Ldarg(textBuilderArg)
                    .Ldstr("null")
                    .Call(TextBuilderReflections.WriteString)
                    .Ret();
                
                // Get delegate
                emitter.MarkLabel(lblNotNull)
                    .Ldarg(valueArg)
                    .Call(_objectGetTypeMethod)
                    .Call(GetDumpValueDelegateMethod)
                    // Add value and text
                    .Ldarg(valueArg)
                    .Call(Reflect.Get<MethodInfo>(() => UnboxOrCastClass<int>(null!)).Mak
                    
                
                
                // Load type
              
              
                // Get the DumpValue Delegate for that type
                throw new NotImplementedException();


            });
    }
}

public static partial class Dump_Cache
{
    private static readonly ConcurrentTypeDictionary<Delegate> _cache;

    static Dump_Cache()
    {
        _cache = new();
        //_cache[typeof(Type)] = null!;
        //_cache[typeof(object)] = null!;
    }

    internal static Delegate CreateValueDumpDelegate(Type valueType)
    {
        throw new NotImplementedException();
    }

    private static ValueDump<T> CreateValueDumpDelegate<T>()
    {
        return (CreateValueDumpDelegate(typeof(T)) as ValueDump<T>)!;
    }

    internal static MethodInfo GetDumpValueDelegateMethod { get; } = Reflect.Get<MethodInfo>(() => GetDumpValueDelegate((Type)null!));
    internal static MethodInfo GetDumpValueDelegateInvokeMethod { get; } = GetDumpValueDelegateMethod
    
    internal static Delegate GetDumpValueDelegate(Type type)
    {
        var del = _cache.GetOrAdd(type, CreateValueDumpDelegate);
        return del;
    }
    
    internal static ValueDump<T> GetDumpValueDelegate<T>()
    {
        var del = _cache.GetOrAdd<T>(CreateValueDumpDelegate);
        if (del is ValueDump<T> valueDump)
            return valueDump;
        throw new InvalidOperationException();
    }

    public static void DumpValue<T>(T? value, TextBuilder text)
    {
        GetDumpValueDelegate<T>()(value, text);
    }
}

public static class DumpExtensions
{
    public static string Dump<T>(this T? value)
    {
        using var text = TextBuilder.Borrow();
        Dump_Cache.DumpValue<T>(value, text);
        return text.ToString();
    }
}