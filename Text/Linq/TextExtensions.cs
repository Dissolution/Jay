namespace Jay.Text.Linq;

public static class TextExtensions
{
    public static void SkipWhiteSpace(this text text, ref int index)
    {
        if (index < 0) return;
        while (index < text.Length && char.IsWhiteSpace(text[index]))
        {
            index++;
        }
    }

    public static void SkipWhile(this text text, ref int index, Func<char, bool> skipPredicate)
    {
        if (index < 0) return;
        while (index < text.Length && skipPredicate(text[index]))
        {
            index++;
        }
    }

    public static void SkipUntil(this text text, ref int index, Func<char, bool> stopPredicate)
    {
        if (index < 0) return;
        while (index < text.Length && !stopPredicate(text[index]))
        {
            index++;
        }
    }

    public static text TakeWhiteSpace(this text text, ref int index)
    {
        if (index < 0) return default;
        int start = index;
        while (index < text.Length && char.IsWhiteSpace(text[index]))
        {
            index++;
        }
        return text.Slice(start, index - start);
    }

    public static text TakeWhile(this text text, ref int index, Func<char, bool> skipPredicate)
    {
        if (index < 0) return default;
        int start = index;
        while (index < text.Length && skipPredicate(text[index]))
        {
            index++;
        }
        return text.Slice(start, index - start);
    }

    public static text TakeUntil(this text text, ref int index, Func<char, bool> stopPredicate)
    {
        if (index < 0) return default;
        int start = index;
        while (index < text.Length && !stopPredicate(text[index]))
        {
            index++;
        }
        return text.Slice(start, index - start);
    }
}