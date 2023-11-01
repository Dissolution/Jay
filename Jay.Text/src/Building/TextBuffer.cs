using System.Diagnostics;
using Jay.Text.Splitting;

namespace Jay.Text.Building;

public class TextBuffer : TextWriter, ITextBuffer, ITextWriter, IBuildingText
{
    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Validate.Index(_length, index);
            return ref _chars[index];
        }
    }

    public Span<char> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Validate.Range(_length, range);
            return _chars.AsSpan(range);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            Validate.Range(_length, range);
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

    public TextBuffer()
    {
    }

    public TextBuffer(int minCapacity) : base(minCapacity)
    {
    }

    public TextBuffer(int literalLength, int formattedCount) : base(literalLength, formattedCount)
    {
    }

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
    public Span<char> Allocate(int length)
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
    public ref char AllocateAt(int index)
    {
        int curLen = _length;
        Validate.InsertIndex(curLen, index);
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
    public Span<char> AllocateRange(int index, int length)
    {
        int curLen = _length;
        Validate.InsertIndex(curLen, index);
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

    public Span<char> AllocateRange(Range range)
    {
        (int offset, int length) = Validate.RangeResolveOffsetLength(_length, range);
        return AllocateRange(offset, length);
    }
#endregion

    /// <summary>
    /// Removes the <see cref="char"/> at the given <paramref name="index"/>
    /// and shifts existing <see cref="Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the char to delete</param>
    public void RemoveAt(int index)
    {
        Written.RemoveAt(index);
        _length -= 1;
    }

    /// <summary>
    /// Removes the <see cref="char"/>s from the given <paramref name="index"/> for the given <paramref name="length"/>
    /// and shifts existing <see cref="Written"/> to cover the hole
    /// </summary>
    /// <param name="index">The index of the first char to delete</param>
    /// <param name="length">The number of chars to delete</param>
    public void RemoveRange(int index, int length)
    {
        Written.RemoveRange(index, length);
        _length -= length;
    }

    public void RemoveRange(Range range)
    {
        (int offset, int length) = range.GetOffsetAndLength(_length);
        RemoveRange(offset, length);
    }

    public void TrimStart()
    {
        int len = _length;
        int i = 0;
        var written = _chars.AsSpan(0, len);
        while (i < len && char.IsWhiteSpace(written[i]))
        {
            i++;
        }
        written.RemoveRange(0, i);
        _length = i;
    }

    public void TrimEnd()
    {
        int len = _length;
        int i = len - 1;
        var written = _chars.AsSpan(0, len);
        while (i >= 0 && char.IsWhiteSpace(written[i]))
        {
            i--;
        }
        _length = i + 1;
    }

#region Replace
    public void Replace(char oldChar, char newChar)
    {
        var written = Written;
        for (var i = written.Length - 1; i >= 0; i--)
        {
            if (written[i] == oldChar)
                written[i] = newChar;
        }
    }

    public void Replace(ReadOnlySpan<char> oldText, ReadOnlySpan<char> newText, StringComparison comparison = StringComparison.Ordinal)
    {
        int oldTextLen = oldText.Length;
        if (oldTextLen == 0)
            throw new ArgumentException("Cannot replace null or empty text", nameof(oldText));

        int newTextLen = newText.Length;
        // Length zero is okay


        // Three possible modes:
        var written = Written;

        // Swap
        if (oldTextLen == newTextLen)
        {
            int index = 0;
            while ((index = written.NextIndexOf(oldText, index, comparison)) != -1)
            {
                TextHelper.Unsafe.CopyBlock(
                    in newText.GetPinnableReference(),
                    ref written[index],
                    newTextLen);
                // Increase index to not continue swapping the same thing forever
                index++;
            }
            return;
        }

        // Shrink
        if (newTextLen < oldTextLen)
        {
            // Start writing at the front
            int writePos = 0;
            var splitRangeList = written
                .TextSplit(oldText, stringComparison: comparison)
                .RangesToList();
            for (var i = 0; i < splitRangeList.Count; i++)
            {
                // Write the range
                Range range = splitRangeList[i];
                (int offset, int length) = range.GetOffsetAndLength(Length);
                TextHelper.Unsafe.CopyBlock(
                    written.Slice(offset, length),
                    written.Slice(writePos), 
                    length);
                writePos += length;

                // If we're at end, we are done
                if (i == (splitRangeList.Count - 1))
                {
                    Debug.Assert(offset + length == Length);
                    Length = writePos;
                    return;
                }

                // Write our new text
                TextHelper.Unsafe.CopyBlock(newText, written.Slice(writePos), newTextLen);
                writePos += newTextLen;
            }

            // Done
            Length = writePos;
            return;
        }

        // Expand
        Debug.Assert(newTextLen > oldTextLen);
        {
            // Move current to a buffer
            Span<char> buffer = stackalloc char[Length];
            TextHelper.Unsafe.CopyBlock(written, buffer, Length);
            // Set us to zero
            Length = 0;

            int writePos = 0;
            var splitRangeList = buffer
                .TextSplit(oldText, stringComparison: comparison)
                .RangesToList();
            for (var i = 0; i < splitRangeList.Count; i++)
            {
                // Write the range
                Range range = splitRangeList[i];
                (int offset, int length) = range.GetOffsetAndLength(buffer.Length);
                this.Write(buffer.Slice(offset, length));
                writePos += length;

                // If we're at end, we are done
                if (i == (splitRangeList.Count - 1))
                {
                    Debug.Assert(offset + length == buffer.Length);
                    //Length = writePos;
                    Debug.Assert(Length == writePos);
                    return;
                }

                // Write our new text
                this.Write(newText);
                writePos += newTextLen;
                Debug.Assert(Length == writePos);
            }

            // Done
            //Length = writePos;
            Debug.Assert(Length == writePos);
        }
    }

    public void Replace(string oldText, string? newText, StringComparison comparison = StringComparison.Ordinal) 
        => Replace(oldText.AsSpan(), newText.AsSpan(), comparison);
#endregion

    public void Clear()
    {
        _length = 0;
    }
}