using Jay.Text;

namespace Jay.Dumping;

public static partial class Dumper
{
    private static bool TryDumpNull<T>([NotNullWhen(false)] T? value, TextBuilder text)
    {
        if (value is null)
        {
            text.Write("null");
            return true;
        }
        return false;
    }
}