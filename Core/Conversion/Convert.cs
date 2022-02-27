using System.Globalization;
using Jay.Collections;

namespace Jay.Conversion;

public readonly struct ParseOptions
{
    public readonly NumberStyles NumberStyle = NumberStyles.Any;
    public readonly IFormatProvider? FormatProvider = null;
}


public class ConvertException : Exception
{
    public ConvertException(string? message = null, 
                            Exception? innerException = null)
        : base(message, innerException)
    {
        
    }
}

public class Converter
{
    private delegate Result TryParseDel<T>(ReadOnlySpan<char> text,
                                        [NotNullWhen(true)] out T value,
                                        ParseOptions options = default);

    private static readonly ConcurrentTypeDictionary<Delegate?> _cache;

    static Converter()
    {
        _cache = new();
        _cache[typeof(object)] = TryParseObject;
        _cache[typeof(int)] = TryParseInt;
    }

    private static Result TryParseObject(ReadOnlySpan<char> text, out object? obj, ParseOptions options)
    {
        throw new NotImplementedException();
    }

    private static Result TryParseInt(ReadOnlySpan<char> text, out int i, ParseOptions options)
    {
        if (int.TryParse(text, options.NumberStyle, options.FormatProvider, out i))
            return true;
        return new ConvertException();
    }
    
    public static Result TryParse<T>(ReadOnlySpan<char> text, 
                                     [NotNullWhen(true)] out T value, 
                                     ParseOptions options = default)
    {
            
    }
}