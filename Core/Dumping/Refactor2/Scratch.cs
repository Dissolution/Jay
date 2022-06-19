using System.Collections.Concurrent;
using Jay.Collections;
using Jay.Reflection.Building;
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
            emitter =>
            {
                
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

    private static Delegate GetValueDump(Type valueType)
    {
        
    }

    private static ValueDump<T> GetValueDump<T>()
    {
        return (GetValueDump(typeof(T)) as ValueDump<T>)!;
    }

    public static void DumpValueTo<T>(T? value, TextBuilder text)
    {
        if (typeof(T) == typeof(object))
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