namespace Jay.Text.Scratch;

public class TextWriterTextWriter : ITextWriter
{
    private readonly TextWriter _textWriter;

    public TextWriterTextWriter(TextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    public void Write(char ch)
    {
        _textWriter.Write(ch);
    }

    public void Write(params char[]? chars)
    {
        _textWriter.Write(chars);
    }

    public void Write(scoped ReadOnlySpan<char> text)
    {
#if NET48 || NETSTANDARD2_0
        _textWriter.Write(text.ToArray());
#else
        _textWriter.Write(text);
#endif
    }

    public void Write(string? str)
    {
        _textWriter.Write(str);
    }

    public void Write(
        [InterpolatedStringHandlerArgument("")]
        ref InterpolatedTextWriter interpolatedText)
    {
        // already written
    }

    public void WriteLine()
    {
        _textWriter.WriteLine();
    }

    public void Format<T>(
        T? value,
        scoped ReadOnlySpan<char> format = default,
        IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            // we have to allocate a buffer
            Span<char> buffer = stackalloc char[BuilderHelper.MinimumCapacity];
            if (((ISpanFormattable)value).TryFormat(
                buffer,
                out int charsWritten,
                format,
                provider))
            {
                _textWriter.Write(buffer[..charsWritten]);
                return;
            }
        }
#endif
        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        _textWriter.Write(str);
    }

    public void Format<T>(
        T? value, string? format = null,
        IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            // we have to allocate a buffer
            Span<char> buffer = stackalloc char[BuilderHelper.MinimumCapacity];
            if (((ISpanFormattable)value).TryFormat(
                buffer,
                out int charsWritten,
                format,
                provider))
            {
                _textWriter.Write(buffer[..charsWritten]);
                return;
            }
        }
#endif
        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        _textWriter.Write(str);
    }

    public void Dispose()
    {
        _textWriter.Dispose();
    }
}