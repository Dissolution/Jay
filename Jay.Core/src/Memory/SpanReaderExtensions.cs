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
        int readCount = spanReader.ReadCount;
        ReadOnlySpan<T> span = spanReader.Span[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && span[i..].StartsWith(match))
        {
            i += match.Length;
        }
        spanReader.ReadCount = readCount + i;
    }
    
    public static ReadOnlySpan<T> TakeWhile<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = spanReader.ReadCount;
        ReadOnlySpan<T> span = spanReader.Span[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.ReadCount = readCount + i;
        return span[..i];
    }
    
    public static void SkipUntil<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = spanReader.ReadCount;
        ReadOnlySpan<T> span = spanReader.Span[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && !span[i..].StartsWith(match))
        {
            i += match.Length;
        }
        spanReader.ReadCount = readCount + i;
    }
    
    public static ReadOnlySpan<T> TakeUntil<T>(
        this ref SpanReader<T> spanReader,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = spanReader.ReadCount;
        ReadOnlySpan<T> span = spanReader.Span[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && !span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanReader.ReadCount = readCount + i;
        return span[..i];
    }
}