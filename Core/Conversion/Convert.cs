using System.Globalization;
using System.Reflection;
using Jay.Collections;
using Jay.Dumping;
using Jay.Reflection;
using Jay.Reflection.Building;

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
        Reflect.AllExportedTypes()
               .SelectMany(type => type.GetMethods(Reflect.StaticFlags)
                                       .Where(method => method.DeclaringType is not null)
                                       .Where(method => method.Name == "TryParse")
                                       .Where(method =>
                                       {
                                           var parameters = method.GetParameters();
                                           bool takesSpan = false;
                                           bool hasOut = false;
                                           foreach (var parameter in parameters)
                                           {
                                               if (parameter.ParameterType == typeof(ReadOnlySpan<char>))
                                               {
                                                   takesSpan = true;
                                               }
                                               else if (parameter.IsOut && parameter.ParameterType == type.MakeByRefType())
                                               {
                                                   hasOut = true;
                                               }
                                           }
                                           return takesSpan && hasOut;
                                       }))
               .Consume(method =>
               {
                   RuntimeBuilder.CreateDelegate(typeof(TryParseDel<>).MakeGenericType(method.DeclaringType!),
                                                 Dump.Text($"{method.DeclaringType}_TryParse"),
                                                 dm =>
                                                 {
                                                     var parameters = method.GetParameters();
                                                     var spanParam = parameters.First(p => p.ParameterType == typeof(ReadOnlySpan<char>));
                                                     var outParam = parameters.First(p => p.IsOut && p.ParameterType == method.DeclaringType.MakeByRefType())
                                                 }
               })


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