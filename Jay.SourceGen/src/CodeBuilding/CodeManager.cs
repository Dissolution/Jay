using TextWriter = Jay.Text.Building.TextWriter;

namespace Jay.SourceGen.CodeBuilding;

public static class CodeManager
{
    public static void WriteCodeTo<T>(T? value, TextWriter textWriter)
    {
        throw new NotImplementedException();
    }

    public static string GetCode<T>(T? value)
    {
        var writer = new TextWriter();
        WriteCodeTo<T>(value, writer);
        return writer.ToStringAndDispose();
    }
}