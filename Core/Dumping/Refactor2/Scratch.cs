using System.Collections.Concurrent;
using Jay.Collections;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Emission;
using Jay.Text;

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

    private static void DumpObject(object? obj, TextBuilder textBuilder)
    {
        if (obj is null)
        {
            textBuilder.Write("null");
        }
        else
        {
            var valueDump = _cache.GetOrAdd(obj.GetType(), CreateValueDumpDelegate);
        
            
            todo
                emit all of this
            
        }
        
        
    }
}

public static partial class Dump_Cache
{
    private static readonly ConcurrentTypeDictionary<Delegate> _cache;

    static Dump_Cache()
    {
        _cache = new();
        _cache[typeof(Type)] = null!;
        _cache[typeof(object)] = null!;
    }

    private static Delegate CreateValueDumpDelegate(Type valueType)
    {
        
    }

    private static ValueDump<T> CreateValueDumpDelegate<T>()
    {
        return (CreateValueDumpDelegate(typeof(T)) as ValueDump<T>)!;
    }

    public static void DumpValue<T>(T? value, TextBuilder text)
    {
        var valueDump = _cache.GetOrAdd<T>(CreateValueDumpDelegate) as ValueDump<T>;
        if (valueDump is null)
            throw new InvalidOperationException();
        valueDump(value, text);
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