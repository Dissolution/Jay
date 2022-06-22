using Jay.Collections;
using Jay.Reflection.Building;
using Jay.Text;

namespace Jay.Dumping;

public static partial class Dumper
{
    private static readonly ConcurrentTypeDictionary<DumpValueTo<object>> _objectDumpCache = new();

    private static void DumpObjectTo(object? obj, TextBuilder text)
    {
        if (obj is null)
        {
            text.Write("null");
            return;
        }

        // Get the cached delegate
        var objectDump = _objectDumpCache.GetOrAdd(obj.GetType(), CreateDumpObjectDelegateFor);

        // Dump it
        objectDump(obj, text);
    }

    private static DumpValueTo<object> CreateDumpObjectDelegateFor(Type objType)
    {
        // Rare case
        if (objType == typeof(object))
        {
            throw new NotImplementedException();
        }

        return RuntimeBuilder.CreateDelegate<DumpValueTo<object>>($"Dump_Object_{objType}", emitter => emitter
            // Load value object
            .Ldarg(0)
            // Cast (unbox/castclass) to i
            .Cast(typeof(object), objType)
            .Ldarg(1)
            .Call(GetDumpValueDelegate(objType).Method)
            .Ret());
    }
}