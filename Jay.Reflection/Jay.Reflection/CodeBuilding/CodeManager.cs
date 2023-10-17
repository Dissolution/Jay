using Jay.Collections;
using Jay.Text.Building;
using TextWriter = Jay.Text.Building.TextWriter;

namespace Jay.Reflection.CodeBuilding;

public static class CodeManager
{
    private static readonly ConcurrentTypeMap<Delegate> _writeCodeCache = new();

    private static Action<T?, ITextBuffer> GetWriteCode<T>()
    {
        return _writeCodeCache
            .GetOrAdd<T>(static t => CreateWriteCode<T>(t))
            .AsValid<Action<T?, ITextBuffer>>();
    }
    
    private static Action<T?, ITextBuffer> CreateWriteCode<T>(Type type)
    {
        if (type == typeof(int))
            return (i, w) => w.Write(i!.ToString());

        throw new NotImplementedException();
    }
    

    public static void WriteCodeTo<T>(T? value, ITextBuffer textWriter)
    {
        GetWriteCode<T>()(value, textWriter);
    }

    public static string GetCode<T>(T? value)
    {
        var textBuilder = new TextBuilder();
        WriteCodeTo<T>(value, textBuilder);
        return textBuilder.ToStringAndDispose();
    }
}