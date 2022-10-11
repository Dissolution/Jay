using Jay.Comparision;
using Jay.Reflection;
using Jay.Text;

namespace Jay;

internal static class OptionsCodeGenerator
{
    public static string GenerateCodeFor(int count)
    {
        using var text = TextBuilder.Borrow();
        string typeName = text.Append("Options<")
            .AppendDelimit(",", Enumerable.Range(0, count), (tb, i) => tb.Append('T').Write(i))
            .Append('>').ToStringAndClear();
        text.Append("public readonly struct ").Append(typeName).AppendNewLine()
            .Append('{')
            .Indent("\t", text1 =>
            {
                throw new NotImplementedException();
            })
            .Append('}');
        throw new NotImplementedException();
    }
}

public readonly struct Options<T1, T2, T3>
{
    
}