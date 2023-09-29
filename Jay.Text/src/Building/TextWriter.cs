using System.Diagnostics;
using Jay.Utilities;
// ReSharper disable MergeCastWithTypeCheck

namespace Jay.Text.Building;

public class TextWriter : IBuildingText
{
    private char[] _chars;
    private int _length;

    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Validate.RefIndex<char>(_chars, index);
    }

    public Span<char> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Validate.RetSlice<char>(_chars, range);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => TextHelper.CopyTo(value, Validate.RetSlice<char>(_chars, range));
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

    public TextWriter()
    {
        _chars = TextPool.Rent();
        _length = 0;
    }
    
    public TextWriter(int minCapacity)
    {
        _chars = TextPool.Rent(minCapacity);
        _length = 0;
    }

    public TextWriter(int literalLength, int formattedCount)
    {
        _chars = TextPool.Rent(literalLength, formattedCount);
        _length = 0;
    }

#region Grow
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int growCount)
    {
        Debug.Assert(growCount > 0);
        char[] newArray = TextPool.RentGrowBy(Capacity, growCount);
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
        TextHelper.Unsafe.CopyBlock(
            in ch,
            ref _chars[index],
            1);
        _length = index + 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(scoped ReadOnlySpan<char> text)
    {
        int index = _length;
        int len = text.Length;
        GrowBy(len);
        TextHelper.Unsafe.CopyBlock(text, _chars.AsSpan(index..), len);
        _length = index + len;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(string text)
    {
        int index = _length;
        int len = text.Length;
        GrowBy(len);
        TextHelper.Unsafe.CopyBlock(
            in text.GetPinnableReference(),
            ref _chars[index],
            len);
        _length = index + len;
    }
#endregion

#region Allocate
    /// <summary>
    /// Allocates a new <see cref="char"/> at the beginning of <see cref="Available"/>,
    /// increases <see cref="Length"/> by 1,
    /// and returns a <c>ref</c> to that <see cref="char"/>
    /// </summary>
    /// <returns></returns>
    public ref char Allocate()
    {
        int curLen = _length;
        int newLen = curLen + 1;
        // Check for growth
        if (newLen > Capacity)
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
    public Span<char> Allocate(int length)
    {
        if (length > 0)
        {
            int curLen = _length;
            int newLen = curLen + length;
            // Check for growth
            if (newLen > Capacity)
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
    public ref char AllocateAt(int index)
    {
        int curLen = _length;
        Validate.InsertIndex(curLen, index);
        int newLen = curLen + 1;

        // Check for growth
        if (newLen > Capacity)
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
    public Span<char> AllocateAt(int index, int length)
    {
        int curLen = _length;
        Validate.InsertIndex(curLen, index);
        if (length > 0)
        {
            int newLen = curLen + length;

            // Check for growth
            if (newLen > Capacity)
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
#endregion

#region Write
    public void Write(char ch)
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
    public virtual void Write(scoped ReadOnlySpan<char> text)
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

    public virtual void Write(params char[]? characters) => Write(characters.AsSpan());
    
    public virtual void Write(string? str)
    {
        if (str is not null)
        {
            int pos = _length;
            var chars = _chars;
            int textLen = str.Length;
            if (pos + textLen <= chars.Length)
            {
                TextHelper.Unsafe.CopyBlock(str, ref chars[pos], textLen);
                _length = pos + textLen;
            }
            else
            {
                GrowThenCopy(str);
            }
        }
    }
    public virtual void Write([InterpolatedStringHandlerArgument("")] ref InterpolatedTextWriter interpolatedText)
    {
        // written
    }
#endregion

#region Format
    public virtual void Format<T>(T? value) => Format<T>(value, default(ReadOnlySpan<char>));
    
    public virtual void Format<T>(T? value, string? format, IFormatProvider? provider = null)
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
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(TextPool.MINIMUM_CAPACITY);
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

        Write(str);
    }
    
    public virtual void Format<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null)
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
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(TextPool.MINIMUM_CAPACITY);
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

        Write(str);
    }
#endregion

#region Remove
    /// <summary>
    /// Removes the <see cref="char"/> at the given <paramref name="index"/>
    /// and shifts existing <see cref="Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the char to delete</param>
    public void Remove(int index)
    {
        int curLen = _length;
        Validate.Index(curLen, index);
        // Everything we're keeping after the cut
        var keep = _chars.AsSpan(new Range(start: index + 1, end: curLen));
        var keepLen = keep.Length;
        // The place to put it at the cut
        var destBuffer = _chars.AsSpan(index, keepLen);
        TextHelper.Unsafe.CopyBlock(keep, destBuffer, keepLen);
        // Length is shorter
        _length = curLen - 1;
    }

    /// <summary>
    /// Removes the <see cref="char"/>s from the given <paramref name="index"/> for the given <paramref name="length"/>
    /// and shifts existing <see cref="Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the first char to delete</param>
    /// <param name="length">The number of chars to delete</param>
    public void Remove(int index, int length)
    {
        int curLen = _length;
        Validate.Range(curLen, index, length);
        // Everything we're keeping after the cut
        var keep = _chars.AsSpan(new Range(start: index + length, end: curLen));
        var keepLen = keep.Length;
        // The place to put it at the cut
        var destBuffer = _chars.AsSpan(index, keepLen);
        TextHelper.Unsafe.CopyBlock(keep, destBuffer, keepLen);
        // Length is shorter
        _length = curLen - length;
    }

    internal void Remove(Range range)
    {
        (int offset, int length) = range.GetOffsetAndLength(_length);
        Remove(offset, length);
    }

    public void RemoveAll(scoped ReadOnlySpan<char> text, StringComparison comparison = StringComparison.Ordinal)
    {
        int index;
        while ((index = MemoryExtensions.IndexOf(Written, text, comparison)) != -1)
        {
            Remove(index, text.Length);
        }
    }

    public void RemoveFirst(int length)
    {
        if (length > 0)
        {
            if (length >= Length)
            {
                Length = 0;
                return;
            }

            int curLen = _length;
            // Everything we're keeping after the cut
            var keep = _chars.AsSpan(new Range(start: length, end: curLen));
            var keepLen = keep.Length;
            // The place we write the keep
            var destBuffer = _chars.AsSpan(0, keepLen);
            TextHelper.Unsafe.CopyBlock(keep, destBuffer, keepLen);

            // Length is shorter
            _length = curLen - length;
        }
    }

    public void RemoveLast(int length)
    {
        if (length > 0)
        {
            // Happy hack
            Length -= length;
        }
    }
#endregion

#nullable disable
    public void Dispose()
    {
        char[] toReturn = _chars;
        _chars = null!;
        TextPool.Return(toReturn);
    }
#nullable enable

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override string ToString() => Written.ToString();
    public override bool Equals(object? obj) => throw new NotSupportedException();
    public override int GetHashCode() => throw new NotSupportedException();
}