namespace Jay.Collections;

/// <summary>
/// Enumerates the elements of a <see cref="ReadOnlySpan{T}"/>.
/// </summary>
public ref struct ReadOnlySpanEnumerator<T>
{
    /// <summary>
    /// The span being enumerated.
    /// </summary>
    private readonly ReadOnlySpan<T> _span;
    
    /// <summary>
    /// The next index to yield.
    /// </summary>
    private int _index;

    /// <summary>
    /// Gets the current index of enumeration
    /// </summary>
    public readonly int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _index;
    }

    /// <summary>
    /// Gets the element at the current position of the enumerator.
    /// </summary>
    public readonly ref readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[_index];
    }
    
    /// <summary>
    /// Initialize the enumerator.
    /// </summary>
    /// <param name="span">The span to enumerate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpanEnumerator(ReadOnlySpan<T> span)
    {
        _span = span;
        _index = -1;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out ReadOnlySpan<T> span, out int index)
    {
        span = _span;
        index = _index;
    }

    /// <summary>
    /// Advances the enumerator to the next element of the span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        int index = _index + 1;
        if (index < _span.Length)
        {
            _index = index;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Resets enumeration
    /// </summary>
    public void Reset()
    {
        _index = -1;
    }
}