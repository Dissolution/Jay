namespace Jay.Conversion;

public interface IParser
{
    Type OutputType { get; }

    Result TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out object? obj, ParseOptions options = default);

    Result TryParse(string? text, [NotNullWhen(true)] out object? obj, ParseOptions options = default)
        => TryParse(text.AsSpan(), out obj, options);
}

public interface IParser<T> : IParser
{
    Type IParser.OutputType => typeof(T);

    Result IParser.TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out object? obj, ParseOptions options)
    {
        throw new NotImplementedException();
    }
    
    Result TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out T? value, ParseOptions options = default);

    Result TryParse(string? text, [NotNullWhen(true)] out T? value, ParseOptions options = default)
        => TryParse(text.AsSpan(), out value, options);
}