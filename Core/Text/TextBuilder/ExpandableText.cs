using System.Buffers;
using System.Runtime.CompilerServices;

namespace Jay.Text;

public ref struct ExpandableText
{
    private char[] _charArray;
    private int _position;

    public ref char this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_position)
                throw new IndexOutOfRangeException();
            return ref _charArray[index];
        }
    }
    public int Length => _position;

    public Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.AsSpan(0, _position);
    }

    internal Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.AsSpan(_position);
    }

    public ExpandableText()
    {
        _charArray = ArrayPool<char>.Shared.Rent(1024);
        _position = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void Grow()
    {
        var newArray = ArrayPool<char>.Shared.Rent(_charArray.Length * 2);
        TextHelper.CopyTo(Written, newArray);
        ArrayPool<char>.Shared.Return(_charArray);
        _charArray = newArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(char ch)
    {
        if (Available.Length == 0)
            Grow();
        _charArray[_position++] = ch;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<char> text)
    {
        while (text.Length > Available.Length)
        {
            Grow();
        }
        TextHelper.CopyTo(text, Available);
        _position += text.Length;
    }

    public void Dispose()
    {
        ArrayPool<char>.Shared.Return(_charArray);
    }
}