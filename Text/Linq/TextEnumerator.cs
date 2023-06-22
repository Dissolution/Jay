namespace Jay.Text.Linq;

/// <summary>
/// Enumerates the characters in <see cref="text"/>
/// </summary>
public ref struct TextEnumerator // : IEnumerator<char>, IEnumerator
{
    /// <summary>
    /// The text being enumerated
    /// </summary>
    private readonly text _text;
    
    /// <summary>
    /// The last yielded index
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
    /// Gets the <see cref="char"/> at the current position of the enumerator
    /// </summary>
    public readonly ref readonly char Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _text[_index];
    }
    
    /// <summary>
    /// Initialize the enumerator.
    /// </summary>
    /// <param name="span">The span to enumerate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextEnumerator(text text)
    {
        _text = text;
        _index = -1;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out text text, out int index)
    {
        text = _text;
        index = _index;
    }

    /// <summary>
    /// Advances the enumerator to the next element of the span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        int index = _index + 1;
        if (index < _text.Length)
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