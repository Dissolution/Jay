namespace Jay.Collections.Iteration;

public static class SpanIteratorExtensions
{
    public static ref SpanIterator<T> SkipWhile<T>(
        this ref SpanIterator<T> spanIterator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var span = spanIterator.Span;
        var i = spanIterator.Position;
        var capacity = spanIterator.Length;
        while (i < capacity && span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanIterator.Position = i;
        return ref spanIterator;
    }
    
    public static ref SpanIterator<T> TakeWhile<T>(
        this ref SpanIterator<T> spanIterator,
        ReadOnlySpan<T> match,
        out ReadOnlySpan<T> taken)
        where T : IEquatable<T>
    {
        var span = spanIterator.Span;
        var i = spanIterator.Position;
        var start = i;
        var capacity = spanIterator.Length;
        while (i < capacity && span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanIterator.Position = i;
        taken = span[start..i];
        return ref spanIterator;
    }
    
    public static ref SpanIterator<T> SkipUntil<T>(
        this ref SpanIterator<T> spanIterator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var span = spanIterator.Span;
        var i = spanIterator.Position;
        var capacity = spanIterator.Length;
        while (i < capacity && !span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanIterator.Position = i;
        return ref spanIterator;
    }
    
    public static ref SpanIterator<T> TakeUntil<T>(
        this ref SpanIterator<T> spanIterator,
        ReadOnlySpan<T> match,
        out ReadOnlySpan<T> taken)
        where T : IEquatable<T>
    {
        var span = spanIterator.Span;
        var i = spanIterator.Position;
        var start = i;
        var capacity = spanIterator.Length;
        while (i < capacity && !span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanIterator.Position = i;
        taken = span[start..i];
        return ref spanIterator;
    }
}