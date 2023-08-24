using Jay.Collections.Iteration;

namespace Jay.Text.Utilities;

/// <summary>
/// Extensions on <see cref="SpanIterator{T}"/> where <typeparamref name="T"/> is <c>char</c>
/// </summary>
/// <remarks>
/// Text has a lot of special ways to be handled with char.IsXYZ() methods
/// </remarks>
public static class CharSpanIteratorExtensions
{
    public static ref SpanIterator<char> SkipWhiteSpace(this ref SpanIterator<char> textIterator)
        => ref textIterator.SkipWhile(static ch => char.IsWhiteSpace(ch));
    
    public static ref SpanIterator<char> SkipDigits(this ref SpanIterator<char> textIterator)
        => ref textIterator.SkipWhile(static ch => char.IsDigit(ch));

    public static ref SpanIterator<char> SkipLetters(this ref SpanIterator<char> textIterator)
        => ref textIterator.SkipWhile(static ch => char.IsLetter(ch));
    
    public static ref SpanIterator<char> TakeWhiteSpace(
        this ref SpanIterator<char> textIterator,
        out ReadOnlySpan<char> taken)
        => ref textIterator.TakeWhile(static ch => char.IsWhiteSpace(ch), out taken);
    
    public static ref SpanIterator<char> TakeDigits(
        this ref SpanIterator<char> textIterator,
        out ReadOnlySpan<char> taken)
        => ref textIterator.TakeWhile(static ch => char.IsDigit(ch), out taken);

    public static ref SpanIterator<char> TakeLetters(
        this ref SpanIterator<char> textIterator,
        out ReadOnlySpan<char> taken)
        => ref textIterator.TakeWhile(static ch => char.IsLetter(ch), out taken);

}

