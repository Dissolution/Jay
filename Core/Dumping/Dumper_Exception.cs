using Jay.Text;

namespace Jay.Dumping;

public static partial class Dumper
{
    private static void DumpExceptionTo(Exception? exception, TextBuilder text)
    {
        if (TryDumpNull(exception, text)) return;
        DumpTypeTo(exception.GetType(), text);
        text.Append(": ").Append(exception.Message);
    }
}