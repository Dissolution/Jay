using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using InlineIL;
using Jay.Collections;
using Jay.Dumping.Refactor;
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

public static class Dump_Cache
{
    internal static readonly ConcurrentTypeDictionary<Delegate> _valueDumpCache;
    internal static readonly ConcurrentTypeDictionary<Delegate> _objectDumpCache;

    static Dump_Cache()
    {
        _objectDumpCache = new();
        _valueDumpCache = new();
        _valueDumpCache[typeof(object)] = (ValueDump<object>)(DumpObjectTo);

    }

    internal static void DumpObjectTo(object? obj, TextBuilder text)
    {
        if (obj is null)
        {
            text.Write("null");
            return;
        }

        var del = _objectDumpCache.GetOrAdd(obj.GetType(), type =>
        {
            var valueDumpDel = GetDumpValueDelegate(type);
            var valueDumpDelInvokeMethod = valueDumpDel.Method;
            return RuntimeBuilder.CreateDelegate<ValueDump<object>>($"Dump_Object_{type}",
                runtimeMethod =>
                {
                    var emitter = runtimeMethod.Emitter;

                    emitter.Ldarg(runtimeMethod.Parameters[0])
                        .Cast(typeof(object), type)
                        .Ldarg(runtimeMethod.Parameters[1])
                        .Call(valueDumpDelInvokeMethod)
                        .Ret();
                });
        });

        if (del is ValueDump<object> objectDump)
        {
            objectDump(obj, text);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    internal static void DumpDefaultTo<T>(T? value, TextBuilder text)
    {
        if (value is IDumpable dumpable)
        {
            dumpable.DumpTo(text);
            return;
        }

        var attr = typeof(T).GetCustomAttribute<DumpAsAttribute>();
        if (attr?.DumpString != null)
        {
            text.Write(attr.DumpString);
            return;
        }

        // Trust in TextBuilder
        text.Write<T>(value);
    }

    internal static Delegate CreateValueDumpDelegate(Type valueType)
    {
        // TODO: LOTS
        var method = typeof(Dump_Cache).GetMethod(nameof(DumpDefaultTo),
                BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull()
            .MakeGenericMethod(valueType);
        return Delegate.CreateDelegate(typeof(ValueDump<>).MakeGenericType(valueType), method);
    }

    private static ValueDump<T> CreateValueDumpDelegate<T>()
    {
        return (CreateValueDumpDelegate(typeof(T)) as ValueDump<T>)!;
    }
    
    internal static Delegate GetDumpValueDelegate(Type type)
    {
        var del = _valueDumpCache.GetOrAdd(type, CreateValueDumpDelegate);
        return del;
    }
    
    internal static ValueDump<T> GetDumpValueDelegate<T>()
    {
        var del = _valueDumpCache.GetOrAdd<T>(CreateValueDumpDelegate);
        if (del is ValueDump<T> valueDump)
            return valueDump;
        throw new InvalidOperationException();
    }

    public static void DumpValueTo<T>(T? value, TextBuilder text)
    {
        GetDumpValueDelegate<T>()(value, text);
    }
}

public static class DumpExtensions
{
    public static string Dump<T>(this T? value)
    {
        using var text = TextBuilder.Borrow();
        Dump_Cache.DumpValueTo<T>(value, text);
        return text.ToString();
    }
}