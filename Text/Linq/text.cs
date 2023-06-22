namespace Jay.Text.Linq;

#pragma warning disable CS8981

partial struct text
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator text(ReadOnlySpan<char> charSpan) => new text(charSpan);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator text(char[] chars) => new text(chars);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator text(string? str) => new text(str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<char>(text text) => text._charSpan;

}




public readonly ref partial struct text
{
    private readonly ReadOnlySpan<char> _charSpan;

    public ref readonly char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _charSpan[index];
    }

    public text this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new text(_charSpan[range]);
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charSpan.Length;
    }

    public text(ReadOnlySpan<char> charSpan)
    {
        _charSpan = charSpan;
    }
    public text(params char[]? chars)
    {
        _charSpan = chars.AsSpan();
    }
    public text(string? str)
    {
        _charSpan = str.AsSpan();
    }




    public text Slice(int start)
    {
        return new text(_charSpan.Slice(start));
    }
    public text Slice(int start, int length)
    {
        return new text(_charSpan.Slice(start, length));
    }
    public text Slice(Range range)
    {
        return new text(_charSpan[range]);
    }

    public text SafeSlice(Range range)
    {
        throw new NotImplementedException();
    }
    

}

partial struct text
{
    public TextEnumerator GetEnumerator()
    {
        return new TextEnumerator(this);
    }
}