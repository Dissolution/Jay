using Jay;

namespace ConsoleSandbox.Conversion;

public interface IFormatter
{
    bool CanFormatFrom(Type inputType);

    Result TryFormat(object? input, out string text, FormatOptions options = default);

    Result TryFormat(object? input, Span<char> destination, out int charsWritten, FormatOptions options = default);
   
}