namespace Jay.Reflection.Conversion;

public interface IFormatter
{
    bool CanFormat(Type inputType);

    bool CanFormat<T>() => CanFormat(typeof(T));

    string Format(object? input, FormatOptions options = default);

    Result TryFormat(object? input, Span<char> destination, out int charsWritten, FormatOptions options = default);
}

public interface IFormatter<T> : IFormatter
{
    bool IFormatter.CanFormat(Type inputType) => inputType.Implements<T>();

    bool IFormatter.CanFormat<T0>() => typeof(T0).Implements<T>();

    string IFormatter.Format(object? input, FormatOptions options)
    {
        if (input is null) return "";
        if (input is T) return Format((T)input, options);
        throw new ArgumentException(FormatterCache.AssemblyCache
                                                  .Format($"'{input}' is not a {typeof(T)} value"),
                                    nameof(input));
    }

    Result IFormatter.TryFormat(object? input, Span<char> destination, out int charsWritten, FormatOptions options)
    {
        if (input is null)
        {
            charsWritten = 0;
            return true;
        }

        if (input is T)
        {
            return TryFormat((T)input, destination, out charsWritten, options);
        }

        charsWritten = 0;
        return new ArgumentException(FormatterCache.AssemblyCache
                                                   .Format($"'{input}' is not a {typeof(T)} value"),
                                     nameof(input));
    }

    string Format(T? value, FormatOptions options = default);

    Result TryFormat(T? value, Span<char> destination, out int charsWritten, FormatOptions options = default);
}