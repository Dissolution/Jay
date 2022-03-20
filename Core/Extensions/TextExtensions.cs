using Jay.Reflection;

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

    public static void ConvertToLower(this ref Span<char> text)
    {
        ref char ch = ref Danger.NullRef<char>();
        for (var i = text.Length - 1; i >= 0; i--)
        {
            ch = ref text[i];
            ch = char.ToLower(ch);
        }
    }
    
    public static void ConvertToUpper(this ref Span<char> text)
    {
        ref char ch = ref Danger.NullRef<char>();
        for (var i = text.Length - 1; i >= 0; i--)
        {
            ch = ref text[i];
            ch = char.ToUpper(ch);
        }
    }

    public static void Convert(this Span<char> text, Func<char, char> transform)
    {
        ref char ch = ref Danger.NullRef<char>();
        for (var i = 0; i < text.Length; i++)
        {
            ch = ref text[i];
            ch = transform(ch);
        }
    }
        
    public static void Convert(this Span<char> text, Func<char, int, char> transform)
    {
        ref char ch = ref Danger.NullRef<char>();
        for (var i = 0; i < text.Length; i++)
        {
            ch = ref text[i];
            ch = transform(ch, i);
        }
    }
    
}