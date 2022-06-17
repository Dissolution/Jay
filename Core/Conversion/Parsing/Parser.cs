using System.Globalization;
using Jay.Collections;

namespace Jay.Conversion.Parsing;

public delegate bool TryParse<T>(ReadOnlySpan<char> text, [NotNullWhen(true)] out T? value, ParseOptions options = default);


public static class Parser
{
    private static readonly ConcurrentTypeDictionary<Delegate?> _cache;

    static Parser()
    {
        _cache = new ConcurrentTypeDictionary<Delegate?>
        {
            [typeof(bool)] = (TryParse<bool>)TryParseBool
        };
    }

    private static bool TryParseBool(ReadOnlySpan<char> text, out bool boolean, ParseOptions options)
    {
        if (bool.TryParse(text, out boolean))
            return true;
        if (text.Equals("yes", StringComparison.OrdinalIgnoreCase))
        {
            boolean = true;
            return true;
        }

        if (text.Equals("no", StringComparison.OrdinalIgnoreCase))
        {
            boolean = false;
            return true;
        }
        
        if (text.Length == 1)
        {
            char ch = text[0];
            if (ch is 'Y' or 'y' or 'T' or 't' or '1')
            {
                boolean = true;
                return true;
            }

            if (ch is 'N' or 'n' or 'F' or 'f' or '0')
            {
                boolean = false;
                return true;
            }

            boolean = default;
            return false;
        }

        boolean = default;
        return false;
    }

    public static bool TryParse<T>(ReadOnlySpan<char> text, [NotNullWhen(true)] out T value, ParseOptions options = default)
    {
        throw new NotImplementedException();
    }
}

public readonly struct ParseOptions
{
    public readonly NumberStyles NumberStyle = NumberStyles.Any;
    public readonly DateTimeStyles DateTimeStyle = DateTimeStyles.None;
    public readonly IFormatProvider? FormatProvider = null;

    public ParseOptions()
    {
        
    }
}