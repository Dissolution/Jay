namespace Jay.Memory;

/// <summary>
/// Extensions on <see cref="SpanReader{T}"/>
/// </summary>
/// <remarks>
/// These methods are not on <see cref="SpanReader{T}"/> itself so that we can constrain
/// the generic type further
/// </remarks>
public static class SpanReaderExtensions
{
    public static void SkipWhile<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var span = spanReader.Span;
        var i = spanReader.Position;
        var capacity = spanReader.Length;
        while (i < capacity && span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.Position = i;
    }
    
    public static ReadOnlySpan<T> TakeWhile<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var span = spanReader.Span;
        var i = spanReader.Position;
        var start = i;
        var capacity = spanReader.Length;
        while (i < capacity && span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.Position = i;
        return span[start..i];
    }
    
    public static void SkipUntil<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var span = spanReader.Span;
        var i = spanReader.Position;
        var capacity = spanReader.Length;
        while (i < capacity && !span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.Position = i;
    }
    
    public static ReadOnlySpan<T> TakeUntil<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        var span = spanReader.Span;
        var i = spanReader.Position;
        var start = i;
        var capacity = spanReader.Length;
        while (i < capacity && !span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.Position = i;
        return span[start..i];
    }
}