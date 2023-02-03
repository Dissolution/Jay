using System.Diagnostics;
using Jay.Collections;
using Jay.Dumping.Interpolated;

namespace Jay.Dumping;

public static class DumpHome
{
    public delegate void DumpValueTo<in T>(T? value, DumpStringHandler stringHandler, DumpFormat format = default);

    private static readonly ConcurrentTypeDictionary<Delegate> _cache = new();


    

    private static Delegate GetOrCreate(Type type)
    {
        return _cache.GetOrAdd(type, CreateDumpValueTo);
    }

    private static Delegate CreateDumpValueTo(Type type)
    {
        if (type == typeof(object))
            Debugger.Break();
       // ?
       throw new NotImplementedException();
    }


    public static string DumpToString<T>(T? value, DumpFormat format = default)
    {
        var tType = typeof(T);
        var valueType = value?.GetType();
        if (tType != valueType)
            Debugger.Break();

        var del = GetOrCreate(tType);
        if (del is DumpValueTo<T> dumpValueTo)
        {
            var txt = new DumpStringHandler();
            dumpValueTo(value, txt, format);
            return txt.ToStringAndDispose();
        }
        throw new NotImplementedException();
    }
}