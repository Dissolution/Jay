namespace Jay.Text.Linq;

public static class textExtensions
{
    public static bool TryPeek(this in text text, out char ch)
    {
        if (text.Length > 0)
        {
            ch = text[0];
            return true;
        }
        ch = default;
        return false;
    }
    public static bool TryPeek(this in text text, int count, out text peeked)
    {
        if ((uint)count <= text.Length)
        {
            peeked = text[..count];
            return true;
        }
        peeked = default;
        return false;
    }

    public static bool TrySkip(this ref text text)
    {
        if (text.Length > 0)
        {
            text = text[1..];
            return true;
        }
        return false;
    }
    public static bool TrySkip(this ref text text, int count)
    {
        if ((uint)count <= text.Length)
        {
            text = text[count..];
            return true;
        }
        return false;
    }

    public static ref text SkipWhile(this ref text text, CharPredicate charPredicate)
    {
        int i = 0;
        var capacity = text.Length;
        while (i < capacity && charPredicate(text[i]))
        {
            i++;
        }
        text = text[i..];
        return ref text;
    }
    
    public static ref text SkipUntil(this ref text text, CharPredicate charPredicate)
    {
        int i = 0;
        var capacity = text.Length;
        while (i < capacity && !charPredicate(text[i]))
        {
            i++;
        }
        text = text[i..];
        return ref text;
    }

    public static ref text SkipWhiteSpace(this ref text text)
    {
        int i = 0;
        var capacity = text.Length;
        while (i < capacity && char.IsWhiteSpace(text[i]))
        {
            i++;
        }
        text = text[i..];
        return ref text;
    }
    

    public static bool TryTake(this ref text text, out char ch)
    {
        if (text.Length > 0)
        {
            ch = text[0];
            text = text[1..];
            return true;
        }
        ch = default;
        return false;
    }
    
    public static bool TryTake(this ref text text, int count, out text taken)
    {
        if ((uint)count <= text.Length)
        {
            taken = text[..count];
            text = text[count..];
            return true;
        }
        taken = default;
        return false;
    }

    public static ref text TakeWhile(this ref text text, 
        CharPredicate charPredicate,
        out text taken)
    {
        int i = 0;
        var capacity = text.Length;
        while (i < capacity && charPredicate(text[i]))
        {
            i++;
        }
        taken = text[..i];
        text = text[i..];
        return ref text;
    }
    
    public static ref text TakeUntil(this ref text text, 
        CharPredicate charPredicate,
        out text taken)
    {
        int i = 0;
        var capacity = text.Length;
        while (i < capacity && !charPredicate(text[i]))
        {
            i++;
        }
        taken = text[..i];
        text = text[i..];
        return ref text;
    }

    public static ref text TakeParse<T>(this ref text text,
        out T? taken)
        where T : ISpanParsable<T>
    {
        int len = 1;
        var capacity = text.Length;
        int valueTaken = 0;
        taken = default;
        while (len < capacity)
        {
            if (!T.TryParse(text.Slice(0, len), null, out var parseValue))
            {
                break;
            }
            taken = parseValue;
            valueTaken = len;
            len++;
        }
        
        // cut out what we took
        text = text[valueTaken..];
        return ref text;
    }
    
    public static ref text TakeParse<T>(this ref text text,
        TryParseText<T> tryParse,
        out T? taken)
    {
        int len = 1;
        var capacity = text.Length;
        int valueTaken = 0;
        taken = default;
        while (len < capacity)
        {
            if (!tryParse(text.Slice(0, len), out var parseValue))
            {
                break;
            }
            taken = parseValue;
            valueTaken = len;
            len++;
        }
        
        // cut out what we took
        text = text[valueTaken..];
        return ref text;
    }
}