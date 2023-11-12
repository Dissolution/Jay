namespace Jay.Collections;

/// <summary>
/// Enumerates the elements of a <see cref="Span{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public ref struct SpanEnumerator<T>
{
    private readonly Span<T> _span;
    private int _index;

    public int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _index;
    }
    
    public ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[_index];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanEnumerator(Span<T> span)
    {
        _span = span;
        _index = -1;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        _index = -1;
    }
}