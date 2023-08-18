namespace Jay.Utilities;

public ref struct SpanReader<T>
{
    private readonly ReadOnlySpan<T> _span;

    public int Index { get; private set; }

    public T? Current { get; private set; }

    public ReadOnlySpan<T> PastSpan
    {
        get
        {
            if (Index <= 0) return ReadOnlySpan<T>.Empty;
            if (Index >= _span.Length) return _span;
            return _span[..Index];
        }
    }

    public ReadOnlySpan<T> FutureSpan
    {
        get
        {
            if (Index <= 0) return _span;
            if (Index >= _span.Length) return ReadOnlySpan<T>.Empty;
            return _span[(Index + 1)..];
        }
    }

    public SpanReader(ReadOnlySpan<T> roSpan)
    {
        _span = roSpan;
        Index = -1;
        Current = default;
    }

    public bool MoveNext()
    {
        int i = Index + 1;
        if (i < _span.Length)
        {
            Index = i;
            Current = _span[i];
            return true;
        }
        Current = default;
        return false;
    }

    public void SkipUntil(Func<T, bool> predicate)
    {
        int i = Index;
        var span = _span;
        if ((uint)i >= span.Length) return;
        while (i < span.Length)
        {
            if (predicate(span[i]))
            {
                Index = i;
                return;
            }
            i++;
        }
        Index = span.Length;
    }
}