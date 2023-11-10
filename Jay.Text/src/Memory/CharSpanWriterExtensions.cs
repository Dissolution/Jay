using Jay.Memory;

namespace Jay.Text.Memory;

public static class CharSpanWriterExtensions
{
    public static Result TryWrite(
        this SpanWriter<char> textWriter,
        [AllowNull] string text)
    {
        if (text is null) return true;

        int index = textWriter.Position;
        int newIndex = index + text.Length;
        var span = textWriter.AvailableItems;
        if (newIndex <= span.Length)
        {
            text.AsSpan().CopyTo(span);
            textWriter.Position = newIndex;
            return true;
        }
        return new InvalidOperationException($"Cannot add '{text}': Only a capacity of {span.Length - index} characters remains");
    }

#if NET6_0_OR_GREATER
    public static Result TryWrite<T>(
        this SpanWriter<char> textWriter,
        T? value,
        scoped ReadOnlySpan<char> format,
        IFormatProvider? provider = null)
        where T : ISpanFormattable
    {
        if (value is null) return true;
        if (value.TryFormat(textWriter.AvailableItems, out int charsWritten, format, provider))
        {
            textWriter.Position += charsWritten;
            return true;
        }
        return new InvalidOperationException(
            $"Cannot write '{value}': Only a capacity of {textWriter.AvailableItems.Length} remains");
    }
#endif


    // public static Result TryWrite(
    //     this SpanWriter<char> textWriter,
    //     [InterpolatedStringHandlerArgument(nameof(textWriter))]
    //     ref InterpolatedSpanWriter interpolatedText)
    // {
    //     return true;
    // }
}

[InterpolatedStringHandler]
public ref struct InterpolatedSpanWriter
{
    private SpanWriter<char> _textWriter;

    public InterpolatedSpanWriter(SpanWriter<char> textWriter)
    {
        _textWriter = textWriter;
    }
}