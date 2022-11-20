using Jay.Extensions;

namespace Jay.Text;

public ref struct CharSpanReader
{
    private readonly ReadOnlySpan<char> _text;
    private int _index;

    internal int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _text.Length;
    }

    public int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _index;
#if NETSTANDARD2_0_OR_GREATER
        set
        {
            if (value < 0) _index = 0;
            else if (value > Capacity) _index = Capacity;
            else _index = value;
        }
#else
        set => _index = Math.Clamp(value, 0, _text.Length);
#endif
    }

    public char Current
    {
        get
        {
            CheckIsCurrent();
            return _text[Index];
        }
    }

    public CharSpanReader(Span<char> text)
    {
        _text = text;
        _index = 0;
    }

    private void CheckIsCurrent()
    {
        if ((uint)_index < Capacity) return;
        if (_index < Capacity)
            throw new InvalidOperationException($"Enumeration has not yet started in this {nameof(CharSpanReader)}");
        throw new InvalidOperationException($"There is no current character for this {nameof(CharSpanReader)}");
    }

    public void SkipWhiteSpace()
    {
        var text = _text;
        var i = _index;
        var capacity = Capacity;
        while (i < capacity && char.IsWhiteSpace(text[i]))
        {
            i++;
        }

        _index = i;
    }

    public ReadOnlySpan<char> TakeWhiteSpace()
    {
        var text = _text;
        var i = _index;
        var start = i;
        var capacity = Capacity;
        while (i < capacity && char.IsWhiteSpace(text[i]))
        {
            i++;
        }

        _index = i;
        return _text.Slice(start, i - start);
    }

    public void SkipDigits()
    {
        var text = _text;
        var i = _index;
        var capacity = Capacity;
        while (i < capacity && char.IsDigit(text[i]))
        {
            i++;
        }

        _index = i;
    }

    public ReadOnlySpan<char> TakeDigits()
    {
        var text = _text;
        var i = _index;
        var start = i;
        var capacity = Capacity;
        while (i < capacity && char.IsDigit(text[i]))
        {
            i++;
        }

        _index = i;
        return _text.Slice(start, i - start);
    }

    public void SkipWhile(Func<char, bool> predicate)
    {
        var text = _text;
        var i = _index;
        var capacity = Capacity;
        while (i < capacity && predicate(text[i]))
        {
            i++;
        }

        _index = i;
    }

    public ReadOnlySpan<char> TakeWhile(Func<char, bool> predicate)
    {
        var text = _text;
        var i = _index;
        var start = i;
        var capacity = Capacity;
        while (i < capacity && predicate(text[i]))
        {
            i++;
        }

        _index = i;
        return _text.Slice(start, i - start);
    }

    public void SkipUntil(char matchChar)
    {
        var text = _text;
        var i = _index;
        var capacity = Capacity;
        while (i < capacity && text[i] != matchChar)
        {
            i++;
        }

        _index = i;
    }

    public ReadOnlySpan<char> TakeUntil(char matchChar)
    {
        var text = _text;
        var i = _index;
        var start = i;
        var capacity = Capacity;
        while (i < capacity && text[i] != matchChar)
        {
            i++;
        }

        _index = i;
        return _text.Slice(start, i - start);
    }

    public void SkipUntil(Func<char, bool> predicate)
    {
        var text = _text;
        var i = _index;
        var capacity = Capacity;
        while (i < capacity && !predicate(text[i]))
        {
            i++;
        }

        _index = i;
    }

    public ReadOnlySpan<char> TakeUntil(Func<char, bool> predicate)
    {
        var text = _text;
        var i = _index;
        var start = i;
        var capacity = Capacity;
        while (i < capacity && !predicate(text[i]))
        {
            i++;
        }

        _index = i;
        return _text.Slice(start, i - start);
    }

    public void SkipAny(params char[] chars)
    {
        var text = _text;
        var i = _index;
        var capacity = Capacity;
        while (i < capacity && chars.Contains(text[i]))
        {
            i++;
        }

        _index = i;
    }

    public ReadOnlySpan<char> TakeAny(params char[] chars)
    {
        var text = _text;
        var i = _index;
        var start = i;
        var capacity = Capacity;
        while (i < capacity && chars.Contains(text[i]))
        {
            i++;
        }

        _index = i;
        return _text.Slice(start, i - start);
    }

    public void Reset()
    {
        _index = -1;
    }
}