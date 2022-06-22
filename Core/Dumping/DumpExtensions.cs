using System.Diagnostics;
using Jay.Text;

namespace Jay.Dumping;

public static class DumpExtensions
{
    public static string Dump<T>(this T? value)
    {
        using var text = TextBuilder.Borrow();
        Dumper.DumpValueTo<T>(value, text);
        return text.ToString();
    }

    public static void DumpTo<T>(this T? value, TextBuilder textBuilder)
    {
        Dumper.DumpValueTo<T>(value, textBuilder);
    }

    public static TextBuilder AppendDump<T>(this TextBuilder textBuilder, T? value)
    {
        Dumper.DumpValueTo<T>(value, textBuilder);
        return textBuilder;
    }

    public static TextBuilder AppendDump<T>(this TextBuilder textBuilder,
        [InterpolatedStringHandlerArgument("textBuilder")]
        ref InterpolatedDumpHandler dumpString)
    {
        // dumpString evaluation will write to textbuilder
        dumpString.ToString();
        Debugger.Break();
        return textBuilder;
    }
}