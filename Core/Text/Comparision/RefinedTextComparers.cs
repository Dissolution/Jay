namespace Jay.Text.Comparision;

/// <summary>
/// A utility for simplifying <see cref="string"/>s by stripping all non-ASCII characters, non-digits, non-letters, then uppercasing.
/// </summary>
public sealed class RefinedTextComparers : TextComparers
{
    public static RefinedTextComparers Instance { get; } = new RefinedTextComparers();

    private const int OFFSET = 'a' - 'A';

    public static string Refine(string? text) => Refine(text.AsSpan());

    public static string Refine(params char[]? chars) => Refine(chars.AsSpan());

    public static string Refine(ReadOnlySpan<char> text)
    {
        Span<char> buffer = stackalloc char[text.Length];
        int b = 0;
        char ch;
        for (var i = 0; i < text.Length; i++)
        {
            ch = text[i];
            if ((ch >= '0' && ch <= '9') || 
                (ch >= 'A' && ch <= 'Z'))
            {
                buffer[b++] = ch;
            }
            else if (ch >= 'a' && ch <= 'z')
            {
                buffer[b++] = (char)(ch - OFFSET);
            }
        }
        return new string(buffer.Slice(0, b));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryRefine(ref char ch)
    {
        switch (ch)
        {
            case >= '0' and <= '9':
            case >= 'A' and <= 'Z':
                return true;
            case >= 'a' and <= 'z':
                ch = (char)(ch - OFFSET);
                return true;
            default:
                return false;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryFindNextRefinedChar(ReadOnlySpan<char> text, ref int index, out char refinedChar)
    {
        while (index < text.Length)
        {
            refinedChar = text[index++];
            switch (refinedChar)
            {
                case >= '0' and <= '9':
                case >= 'A' and <= 'Z':
                    return true;
                case >= 'a' and <= 'z':
                    refinedChar = (char)(refinedChar - OFFSET);
                    return true;
            }
        }

        refinedChar = default;
        return false;
    }

    public override bool Equals(ReadOnlySpan<char> xText, ReadOnlySpan<char> yText)
    {
        int x = 0;
        int y = 0;
        while (TryFindNextRefinedChar(xText, ref x, out var xCh) &&
               TryFindNextRefinedChar(yText, ref y, out var yCh))
        {
            if (xCh != yCh) return false;
        }

        //Both ended at the same time, so they're equal
        return true;
    }

    public override int GetHashCode(ReadOnlySpan<char> text)
    {
        var hasher = new Hasher();
        for (var i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (TryRefine(ref c))
            {
                hasher.AddHash((int)c);
            }
        }
        return hasher.CreateHashCode();
    }

    public override int Compare(ReadOnlySpan<char> xText, ReadOnlySpan<char> yText)
    {
        int x = 0;
        int y = 0;
        while (TryFindNextRefinedChar(xText, ref x, out var xCh) &&
               TryFindNextRefinedChar(yText, ref y, out var yCh))
        {
            var c = xCh.CompareTo(yCh);
            if (c != 0)
                return c;
        }

        //Both ended at the same time, so they're equal
        return 0;
    }
}