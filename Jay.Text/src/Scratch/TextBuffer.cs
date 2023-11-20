using System.Diagnostics;
using Jay.Text.Building;

namespace Jay.Text.Scratch;

public abstract class TextBuffer : IBuildingText
{
    private char[] _chars;
    private int _length;

    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Throw.Index(_length, index);
            return ref _chars[index];
        }
    }

    public ref char this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            int offset = Throw.Index(_length, index);
            return ref _chars[offset];
        }
    }

    public Span<char> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Throw.Range(_length, range);
            return _chars.AsSpan(range);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            Throw.Range(_length, range);
            TextHelper.CopyTo(value, _chars.AsSpan(range));
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _length = value.Clamp(0, Capacity);
    }

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Length;
    }

    public Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.AsSpan(0, _length);
    }

    public Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.AsSpan(_length);
    }


    protected internal TextBuffer(int literalLength, int formattedCount)
    {
        _chars = TextPool.Rent(literalLength, formattedCount);
        _length = 0;
    }

    protected TextBuffer()
    {
        _chars = TextPool.Rent();
        _length = 0;
    }

    protected TextBuffer(int minCapacity)
    {
        _chars = TextPool.Rent(minCapacity);
        _length = 0;
    }


#region Grow
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int growCount)
    {
        Debug.Assert(growCount > 0);
        char[] newArray = TextPool.RentGrowBy(_chars.Length, growCount);
        TextHelper.CopyTo(_chars, newArray);

        char[] toReturn = _chars;
        _chars = newArray;
        TextPool.Return(toReturn);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(char ch)
    {
        int index = _length;
        GrowBy(1);
        TextHelper.Unsafe.CopyBlock(in ch, ref _chars[index], 1);
        _length = index + 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(scoped ReadOnlySpan<char> text)
    {
        int index = _length;
        int len = text.Length;
        GrowBy(len);
        TextHelper.Unsafe.CopyBlock(text, _chars.AsSpan(index), len);
        _length = index + len;
    }
#endregion Grow

#region Write
    protected internal void Write(char ch)
    {
        int pos = _length;
        Span<char> chars = _chars;
        if (pos < chars.Length)
        {
            chars[pos] = ch;
            _length = pos + 1;
        }
        else
        {
            GrowThenCopy(ch);
        }
    }

    protected internal void Write(scoped ReadOnlySpan<char> text)
    {
        int textLen = text.Length;
        if (textLen > 0)
        {
            int pos = _length;
            Span<char> chars = _chars;
            if (pos + textLen <= chars.Length)
            {
                TextHelper.Unsafe.CopyBlock(text, chars[pos..], textLen);
                _length = pos + textLen;
            }
            else
            {
                GrowThenCopy(text);
            }
        }
    }

    protected internal void Format<T>(T? value)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_chars.AsSpan(_length), out charsWritten, default, default))
                {
                    GrowBy(1);
                }
                _length += charsWritten;
                return;
            }
#endif

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }

        this.Write(str.AsSpan());
    }

    protected internal void Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_chars.AsSpan(_length), out charsWritten, format, provider))
                {
                    GrowBy(1);
                }
                _length += charsWritten;
                return;
            }
#endif

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        Write(str.AsSpan());
    }

    protected internal void Format<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_chars.AsSpan(_length), out charsWritten, format, provider))
                {
                    GrowBy(1);
                }
                _length += charsWritten;
                return;
            }
#endif

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        Write(str.AsSpan());
    }
#endregion Write

#region Allocate
    /// <summary>
    /// Allocates a new <see cref="char"/> at the beginning of <see cref="Available"/>,
    /// increases <see cref="Length"/> by 1,
    /// and returns a <c>ref</c> to that <see cref="char"/>
    /// </summary>
    /// <returns></returns>
    protected ref char Allocate()
    {
        int curLen = _length;
        int newLen = curLen + 1;
        // Check for growth
        if (newLen > _chars.Length)
        {
            GrowBy(1);
        }

        // Add to our current position
        _length = newLen;
        // Return the allocated (at end of Written)
        return ref _chars[curLen];
    }

    /// <summary>
    /// Allocates a <c>Span&lt;char&gt;</c> at the beginning of <see cref="Available"/>,
    /// increases <see cref="Length"/> by <paramref name="length"/>
    /// and returns the allocated <c>Span&lt;char&gt;</c>
    /// </summary>
    /// <param name="length">The number of characters to allocate space for</param>
    protected Span<char> Allocate(int length)
    {
        if (length > 0)
        {
            int curLen = _length;
            int newLen = curLen + length;
            // Check for growth
            if (newLen > _chars.Length)
            {
                GrowBy(length);
            }

            // Add to our current position
            _length = newLen;

            // Return the allocated (at end of Written)
            return _chars.AsSpan(curLen, length);
        }

        // Asked for nothing
        return default;
    }

    /// <summary>
    /// Allocates a new <see cref="char"/> at <paramref name="index"/>,
    /// shifts existing chars to make an empty hole,
    /// increases <see cref="Length"/> by 1,
    /// and returns a <c>ref</c> to that <see cref="char"/>
    /// </summary>
    /// <param name="index">The index to allocate a character at</param>
    /// <returns></returns>
    protected ref char AllocateAt(int index)
    {
        int curLen = _length;
        Throw.Index(curLen, index, true);
        int newLen = curLen + 1;

        // Check for growth
        if (newLen > _chars.Length)
        {
            GrowBy(1);
        }

        // We're adding this much
        _length = newLen;

        // At end?
        if (index == curLen)
        {
            // The same as Allocate()
            return ref _chars[curLen];
        }
        // Insert

        // Shift existing to the right
        var keep = _chars.AsSpan(new Range(start: index, end: curLen));
        var keepLength = keep.Length;
        // We know we have enough space to grow to
        var rightBuffer = _chars.AsSpan(index + 1, keepLength);
        TextHelper.Unsafe.CopyBlock(
            source: keep,
            destination: rightBuffer,
            count: keepLength);
        // return where we allocated
        return ref _chars[index];
    }

    /// <summary>
    /// Allocates a <c>Span&lt;char&gt;</c> at <paramref name="index"/>,
    /// shifts existing chars to make an empty hole,
    /// increases <see cref="Length"/> by <paramref name="length"/>
    /// and returns the allocated <c>Span&lt;char&gt;</c>
    /// </summary>
    /// <param name="index">The index to allocate the span at</param>
    /// <param name="length">The number of characters to allocate space for</param>
    protected Span<char> AllocateRange(int index, int length)
    {
        int curLen = _length;
        Throw.Index(curLen, index, true);
        if (length > 0)
        {
            int newLen = curLen + length;

            // Check for growth
            if (newLen > _chars.Length)
            {
                GrowBy(length);
            }

            // We're adding this much
            _length = newLen;

            // At end?
            if (index == curLen)
            {
                // The same as Allocate(length)
                return _chars.AsSpan(curLen, length);
            }
            // Insert

            // Shift existing to the right
            var keep = _chars.AsSpan(new Range(start: index, end: curLen));
            var keepLen = keep.Length;
            // We know we have enough space to grow to
            var destBuffer = _chars.AsSpan(index + length, keepLen);
            TextHelper.Unsafe.CopyBlock(
                source: keep,
                destination: destBuffer,
                count: keepLen);
            // return where we allocated
            return _chars.AsSpan(index, length);
        }

        // Asked for nothing
        return Span<char>.Empty;
    }

    protected Span<char> AllocateRange(Range range)
    {
        (int offset, int length) = Throw.Range(_length, range);
        return AllocateRange(offset, length);
    }
#endregion

#region Remove
    /// <summary>
    /// Removes the <see cref="char"/> at the given <paramref name="index"/>
    /// and shifts existing <see cref="Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the char to delete</param>
    protected void RemoveAt(int index)
    {
        var written = this.Written;
        Throw.Index(written.Length, index);
        var leftSide = written.Slice(index);
        var rightSide = written.Slice(index + 1);
        rightSide.CopyTo(leftSide);
        _length -= 1;
    }

    /// <summary>
    /// Removes the <see cref="char"/>s from the given <paramref name="index"/> for the given <paramref name="length"/>
    /// and shifts existing <see cref="Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the first char to delete</param>
    /// <param name="length">The number of chars to delete</param>
    protected void RemoveRange(int index, int length)
    {
        var written = this.Written;
        Throw.Range(written.Length, index, length);
        var leftSide = written.Slice(index);
        var rightSide = written.Slice(index + length);
        rightSide.CopyTo(leftSide);
        _length -= length;
    }

    protected void RemoveRange(Range range)
    {
        (int offset, int length) = range.GetOffsetAndLength(_length);
        RemoveRange(offset, length);
    }
#endregion Remove

    protected void Clear()
    {
        _length = 0;
    }

    public virtual void Dispose()
    {
        char[] toReturn = _chars;
        _chars = null!;
        TextPool.Return(toReturn);
    }

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override string ToString() => new string(_chars, 0, _length);
    public sealed override bool Equals(object? obj) => throw new NotSupportedException();
    public sealed override int GetHashCode() => throw new NotSupportedException();
}