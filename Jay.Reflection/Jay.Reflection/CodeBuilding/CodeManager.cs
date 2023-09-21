using Jay.Collections;
using TextWriter = Jay.Text.Building.TextWriter;

namespace Jay.Reflection.CodeBuilding;

public static class CodeManager
{
    private static readonly ConcurrentTypeMap<Delegate> _writeCodeCache = new();

    private static Action<T?, TextWriter> GetWriteCode<T>()
    {
        return _writeCodeCache
            .GetOrAdd<T>(static t => CreateWriteCode<T>(t))
            .AsValid<Action<T?, TextWriter>>();
    }
    
    private static Action<T?, TextWriter> CreateWriteCode<T>(Type type)
    {
        if (type == typeof(int))
            return (i, w) => w.Write(i!.ToString());

        throw new NotImplementedException();
    }
    

    public static void WriteCodeTo<T>(T? value, TextWriter textWriter)
    {
        GetWriteCode<T>()(value, textWriter);
    }

    public static string GetCode<T>(T? value)
    {
        var writer = new TextWriter();
        WriteCodeTo<T>(value, writer);
        return writer.ToStringAndDispose();
    }
}