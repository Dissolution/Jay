using Jay.Memory;

namespace Jay.Text.Memory;

/// <summary>
/// Extensions on <see cref="SpanReader{T}">SpanReader&lt;char&gt;</see>
/// </summary>
/// <remarks>
/// Text has extra common ways to be consumed with char.IsXYZ() methods
/// </remarks>
public static class CharSpanReaderExtensions
{
    public static void SkipWhiteSpace(this ref SpanReader<char> textIterator)
        => textIterator.SkipWhile(static ch => char.IsWhiteSpace(ch));
    
    public static void SkipDigits(this ref SpanReader<char> textIterator)
        => textIterator.SkipWhile(static ch => char.IsDigit(ch));

    public static void SkipLetters(this ref SpanReader<char> textIterator)
        => textIterator.SkipWhile(static ch => char.IsLetter(ch));
    
    public static ReadOnlySpan<char> TakeWhiteSpace(
        this ref SpanReader<char> textIterator)
        => textIterator.TakeWhile(static ch => char.IsWhiteSpace(ch));
    
    public static ReadOnlySpan<char> TakeDigits(
        this ref SpanReader<char> textIterator)
        => textIterator.TakeWhile(static ch => char.IsDigit(ch));

    public static ReadOnlySpan<char> TakeLetters(
        this ref SpanReader<char> textIterator)
        => textIterator.TakeWhile(static ch => char.IsLetter(ch));

}

