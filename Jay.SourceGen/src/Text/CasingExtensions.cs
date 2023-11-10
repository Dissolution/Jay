namespace Jay.SourceGen.Text;

public static class CasingExtensions
{
    public static string ToCase(this string? text, Casing casing)
    {
        if (text is null) return string.Empty;
        int textLen = text.Length;
        if (textLen == 0) return string.Empty;
        switch (casing)
        {
            case Casing.Lower:
            {
                return text.ToLower();
            }
            case Casing.Upper:
            {
                return text.ToUpper();
            }
            case Casing.Camel:
            {
                Span<char> buffer = stackalloc char[textLen];
                buffer[0] = char.ToLower(text[0]);
                text.AsSpan(1).CopyTo(buffer.Slice(1));
                return buffer.ToString();
            }
            case Casing.Pascal:
            {
                Span<char> buffer = stackalloc char[textLen];
                buffer[0] = char.ToUpper(text[0]);
                text.AsSpan(1).CopyTo(buffer.Slice(1));
                return buffer.ToString();
            }
            default:
                return text;
        }
    }
}