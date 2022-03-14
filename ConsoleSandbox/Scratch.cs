using System.Diagnostics.CodeAnalysis;
using ConsoleSandbox.Conversion;
using Jay;

namespace ConsoleSandbox;



public interface IParser<TOut> : IParser
{
    Type IParser.OutputType => typeof(TOut);

    Result IParser.TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out object? obj, ParseOptions options)
    {
        Result result = TryParse(text, out TOut? value, options);
        if (!result)
        {
            obj = null;
            return result;
        }
        else
        {
            obj = value!;
            return true;
        }
    }

    Result TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out TOut? value, ParseOptions options = default);

    Result TryParse(string? text, [NotNullWhen(true)] out TOut? value, ParseOptions options = default)
        => TryParse(text.AsSpan(), out value, options);
}