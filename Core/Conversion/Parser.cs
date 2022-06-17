

namespace Jay.Conversion;

internal partial class Parser
{
    private static Result.Result TryConvert(ReadOnlySpan<char> text, out bool boolean, ParseOptions options = default)
    {
        if (text.Length == 0)
        {
            boolean = default;
            return ParseException.Create<bool>(text, options);
        }

        if (text.Length == 1 || options.UseFirstChar)
        {
            char c = text[0];
            if (c is '1' or 'T' or 't' or 'Y' or 'y')
            {
                boolean = true;
                return true;
            }
            if (c is '0' or 'F' or 'f' or 'N' or 'n')
            {
                boolean = false;
                return true;
            }

            boolean = default;
            return ParseException.Create<bool>(text, options);
        }

        if (text.Equals(bool.TrueString, options.Comparison))
        {
            boolean = true;
            return true;
        }

        if (text.Equals(bool.FalseString, options.Comparison))
        {
            boolean = false;
            return true;
        }

        boolean = default;
        return ParseException.Create<bool>(text, options);
    }

    private static Result.Result TryConvert(ReadOnlySpan<char> text, out char character, ParseOptions options = default)
    {
        if (text.Length == 0)
        {
            character = default;
            return ParseException.Create<char>(text, options);
        }

        if (text.Length == 1 || options.UseFirstChar)
        {
            character = text[0];
            return true;
        }

        character = default;
        return ParseException.Create<char>(text, options);
    }

    private static Result.Result TryConvert(ReadOnlySpan<char> text, out Guid guid, ParseOptions options = default)
    {
        if (options.HasExactFormat(out var exactFormat) && 
            Guid.TryParseExact(text, exactFormat, out guid))
        {
            return true;
        }

        if (Guid.TryParse(text, out guid))
        {
            return true;
        }

        guid = Guid.Empty;
        return ParseException.Create<char>(text, options);
    }

    private static Result.Result TryConvert(ReadOnlySpan<char> text, out TimeSpan timeSpan, ParseOptions options = default)
    {
        if (options.HasExactFormat(out var exactFormat) && 
            TimeSpan.TryParseExact(text, exactFormat, options.FormatProvider, options.TimeSpanStyles, out timeSpan))
        {
            return true;
        }

        if (options.ExactFormats != null && 
            TimeSpan.TryParseExact(text, options.ExactFormats, options.FormatProvider, options.TimeSpanStyles, out timeSpan))
        {
            return true;
        }

        if (TimeSpan.TryParse(text, options.FormatProvider, out timeSpan))
        {
            return true;
        }

        timeSpan = TimeSpan.Zero;
        return ParseException.Create<TimeSpan>(text, options);
    }

    private static Result.Result TryConvert(ReadOnlySpan<char> text, out DateTime dateTime, ParseOptions options = default)
    {
        if (options.HasExactFormat(out var exactFormat) && 
            DateTime.TryParseExact(text, exactFormat, options.FormatProvider, options.DateTimeStyles, out dateTime))
        {
            return true;
        }

        if (options.ExactFormats != null && 
            DateTime.TryParseExact(text, options.ExactFormats, options.FormatProvider, options.DateTimeStyles, out dateTime))
        {
            return true;
        }

        if (DateTime.TryParse(text, options.FormatProvider, options.DateTimeStyles, out dateTime))
        {
            return true;
        }

        dateTime = DateTime.Now;
        return ParseException.Create<DateTime>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out DateTimeOffset dateTimeOffset, ParseOptions options = default)
    {
        if (options.HasExactFormat(out var exactFormat) && 
            DateTimeOffset.TryParseExact(text, exactFormat, options.FormatProvider, options.DateTimeStyles, out dateTimeOffset))
        {
            return true;
        }

        if (options.ExactFormats != null && 
            DateTimeOffset.TryParseExact(text, options.ExactFormats, options.FormatProvider, options.DateTimeStyles, out dateTimeOffset))
        {
            return true;
        }

        if (DateTimeOffset.TryParse(text, options.FormatProvider, options.DateTimeStyles, out dateTimeOffset))
        {
            return true;
        }

        dateTimeOffset = DateTimeOffset.Now;
        return ParseException.Create<DateTimeOffset>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out byte value, ParseOptions options = default)
    {
        if (byte.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<byte>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out sbyte value, ParseOptions options = default)
    {
        if (sbyte.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<sbyte>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out short value, ParseOptions options = default)
    {
        if (short.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<short>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out ushort value, ParseOptions options = default)
    {
        if (ushort.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<ushort>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out int value, ParseOptions options = default)
    {
        if (int.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<int>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out uint value, ParseOptions options = default)
    {
        if (uint.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<uint>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out long value, ParseOptions options = default)
    {
        if (long.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<long>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out ulong value, ParseOptions options = default)
    {
        if (ulong.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<ulong>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out float value, ParseOptions options = default)
    {
        if (float.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<float>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out double value, ParseOptions options = default)
    {
        if (double.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<double>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out decimal value, ParseOptions options = default)
    {
        if (decimal.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
        {
            return true;
        }

        value = default;
        return ParseException.Create<decimal>(text, options);
    }

    public static Result.Result TryConvert(ReadOnlySpan<char> text, out string str, ParseOptions options = default)
    {
        str = new string(text);
        return true;
    }
}