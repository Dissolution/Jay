using System.Buffers;
using System.Runtime.CompilerServices;
using Jay.Exceptions;
using Jay.Text;

namespace Jay.BenchTests.Text;

public static class TextBuilderExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TextBuilder Clear(this ref TextBuilder textBuilder)
    {
        textBuilder.Length = 0;
        return ref textBuilder;
    }

}

public ref partial struct TextBuilder
{
    internal const int MinCapacity = 1024;
    internal const int MaxCapacity = 0x7FFFFFC7 / sizeof(char); // Array.MaxLength

    public static implicit operator TextBuilder(Span<char> buffer) => new TextBuilder(buffer);
    
    static TextBuilder()
    {
        //DefaultInterpolatedStringHandler
    }
}

public ref partial struct TextBuilder
{
    private char[]? _charArray;
    private Span<char> _charSpan;
    private int _length;

    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)index < (uint)Capacity)
            {
                return ref _charSpan[index];
            }
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and {Capacity - 1}");
        }
    }

    internal int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charSpan.Length;
    }
    
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _length = value;
    }

    public Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charSpan[.._length];
    }

    internal Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charSpan[_length..];
    }

    public TextBuilder()
    {
        _charSpan = _charArray = ArrayPool<char>.Shared.Rent(MinCapacity);
        _length = 0;
    }

    public TextBuilder(int minCapacity)
    {
        _charSpan = _charArray = ArrayPool<char>.Shared.Rent(Math.Clamp(minCapacity, MinCapacity, MaxCapacity));
        _length = 0;
    }

    public TextBuilder(Span<char> buffer)
    {
        _charArray = null;
        _charSpan = buffer;
        _length = 0;
    }
   
    #region Benchmark Methods

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowBy(int amount)
    {
        var newCapacity = Math.Clamp((Capacity + amount) * 2, MinCapacity, MaxCapacity);
        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        TextHelper.CopyTo(Written, newArray);
        char[]? toReturn = _charArray;
        _charSpan = _charArray = newArray;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowCopy(char ch)
    {
        GrowBy(1);
        _charSpan[0] = ch;
        _length++;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharA(char ch)
    {
        if (Available.Length == 0)
        {
            GrowBy(1);
        }
        Available[0] = ch;
        _length++;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharB(char ch)
    {
        if (Available.Length == 0)
        {
            GrowBy(1);
        }
        _charSpan[0] = ch;
        _length++;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharC(char ch)
    {
        if (_charSpan.Length - _length <= 0)
        {
            GrowBy(1);
        }
        _charSpan[0] = ch;
        _length++;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharD(char ch)
    {
        Span<char> chars = _charSpan;
        int pos = _length;
        if (pos < chars.Length)
        {
            chars[pos] = ch;
            _length = pos + 1;
        }
        else
        {
            GrowCopy(ch);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCharE(char ch)
    {
        int pos = _length;
        if (pos >= _charSpan.Length)
        {
            GrowBy(1);
        }
        _charSpan[pos] = ch;
        _length = pos + 1;
    }
    
    #endregion
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        char[]? toReturn = _charArray;
        this = default; // defensive clear
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(string? text) => TextHelper.Equals(Written, text);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(string? text, StringComparison comparison) => TextHelper.Equals(Written, text, comparison);

    public override bool Equals(object? obj)
    {
        if (obj is string str)
            return TextHelper.Equals(Written, str);
        if (obj is char[] chars)
            return TextHelper.Equals(Written, chars);
        return false;
    }

    public override int GetHashCode() => UnsuitableException.ThrowGetHashCode(typeof(TextBuilder));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return new string(_charArray, 0, _length);
    }
}