namespace Jay;

using text = System.ReadOnlySpan<char>;

public static class TextExtensions
{
    public static text TakeUntil(this text text, ref int index, char match)
    {
        int start = index;
        int len = text.Length;
        if (start < 0 || start >= len)
            return default;
        while (index < len && text[index] != match)
        {
            index++;
        }
        return text.Slice(start, index - start);
    }

    public static text TakeUntil(this text text, ref int index, Func<char, bool> match)
    {
        int start = index;
        int len = text.Length;
        if (start < 0 || start >= len)
            return default;
        while (index < len && !match(text[index]))
        {
            index++;
        }
        return text.Slice(start, index - start);
    }

}