using System.Diagnostics.CodeAnalysis;
using Jay;

namespace ConsoleSandbox.Conversion;

public interface IParser
{
    Type OutputType { get; }

    Result TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out object? obj, ParseOptions options = default);

    Result TryParse(string? text, [NotNullWhen(true)] out object? obj, ParseOptions options = default)
        => TryParse(text.AsSpan(), out obj, options);
}