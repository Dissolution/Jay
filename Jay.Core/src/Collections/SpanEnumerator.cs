namespace Jay.Collections;

/// <summary>
/// Enumerates the elements of a <see cref="Span{T}" />.
/// </summary>
public ref struct SpanEnumerator<T>
{
    /// <summary>
    /// The span being enumerated.
    /// </summary>
    private readonly Span<T> _span;

    /// <summary>
    /// The next index to yield.
    /// </summary>
    public int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    }

    /// <summary>Gets the element at the current position of the enumerator.</summary>
    public ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[Index];
    }

    /// <summary>
    /// Initialize the enumerator.
    /// </summary>
    /// <param name="span">The span to enumerate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanEnumerator(Span<T> span)
    {
        _span = span;
        Index = -1;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out Span<T> span, out int index)
    {
        span = _span;
        index = Index;
    }

    /// <summary>
    /// Advances the enumerator to the next element of the span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        int index = Index + 1;
        if (index < _span.Length)
        {
            Index = index;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Resets enumeration
    /// </summary>
    public void Reset()
    {
        Index = -1;
    }
}