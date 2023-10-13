using System.ComponentModel;
using Jay.Text;

namespace Jay.Memory;

/// <summary>
/// A <see cref="SpanReader{T}"/> wraps a <see cref="ReadOnlySpan{T}"/>
/// and provides methods to Peek, Skip, and Take <typeparamref name="T"/> value(s) from it
/// in forward-only reads that 'consume' the span
/// </summary>
/// <typeparam name="T">
/// <see cref="Type"/>s of values stored in the <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
public ref struct SpanReader<T>
{
    private readonly ReadOnlySpan<T> _span;
    private int _position;

    internal ReadOnlySpan<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span;
    }
    
    /// <summary>
    /// Gets the current reading position
    /// </summary>
    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _position = value;
    }

    /// <summary>
    /// Gets the total number of values that could be read
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }

    /// <summary>
    /// Gets the total number of values that can yet be read
    /// </summary>
    public int RemainingLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length - _position;
    }

    /// <summary>
    /// Gets the <see cref="ReadOnlySpan{T}"/> of items that have already been read
    /// </summary>
    public ReadOnlySpan<T> ReadItems
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[.._position];
    }
    
    /// <summary>
    /// Gets the <see cref="ReadOnlySpan{T}"/> of items that have not yet been read
    /// </summary>
    public ReadOnlySpan<T> UnreadItems
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[_position..];
    }

    /// <summary>
    /// Create a new <see cref="SpanReader{T}"/> that reads from the given <paramref name="span"/>
    /// </summary>
    /// <param name="span"></param>
    public SpanReader(ReadOnlySpan<T> span)
    {
        _span = span;
        _position = 0;
    }

#region Peek
    /// <summary>
    /// Tries to peek at the next <paramref name="item"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Result TryPeek([MaybeNullWhen(false)] out T item)
    {
        int pos = _position;
        var span = _span;
        if (pos < span.Length)
        {
            item = span[pos];
            return true;
        }
        item = default;
        return new InvalidOperationException("Cannot peek an item: Zero items remain");
    }

    /// <summary>
    /// Try to peek at the next <paramref name="count"/> <paramref name="items"/>
    /// </summary>
    /// <param name="count"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public Result TryPeek(int count, out ReadOnlySpan<T> items)
    {
        if (count < 0)
        {
            items = default;
            return new ArgumentOutOfRangeException(nameof(count), count, "Count must be zero or greater");
        }
        int pos = _position;
        var span = _span;
        if (pos + count <= span.Length)
        {
            items = span.Slice(pos, count);
            return true;
        }
        items = default;
        return new InvalidOperationException($"Cannot peek {count} items: Only {RemainingLength} items remain");
    }

    public T Peek()
    {
        TryPeek(out var value).ThrowIfError();
        return value;
    }

    public ReadOnlySpan<T> Peek(int count)
    {
        TryPeek(count, out var values).ThrowIfError();
        return values;
    }
#endregion

#region Skip
    public Result TrySkip()
    {
        int index = _position;
        if (index < Length)
        {
            _position = index + 1;
            return true;
        }
        return new InvalidOperationException("Cannot skip an item: No items remain");
    }

    public Result TrySkip(int count)
    {
        if (count <= 0)
            return true;

        int index = _position;
        int newIndex = index + count;
        if (newIndex <= Length)
        {
            _position = newIndex;
            return true;
        }
        return new InvalidOperationException($"Cannot skip {count} items: Only {RemainingLength} items remain");
    }

    public void Skip() => TrySkip().ThrowIfError();

    public void Skip(int count) => TrySkip(count).ThrowIfError();

    public void SkipWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = span.Length;
        while (index < len && itemPredicate(span[index]))
        {
            index += 1;
        }
        _position = index;
    }

    public void SkipWhile(T match) => SkipWhile(item => EqualityComparer<T>.Default.Equals(item, match));

    public void SkipUntil(Func<T, bool> itemPredicate) => SkipWhile(item => !itemPredicate(item));

    public void SkipUntil(T match) => SkipWhile(item => !EqualityComparer<T>.Default.Equals(item, match));

    public void SkipAny(params T[] matches) => SkipWhile(item => matches.Contains(item));

    public void SkipAny(IReadOnlyCollection<T> matches) => SkipWhile(item => matches.Contains(item));

    public void SkipAll() => SkipWhile(static _ => true);
#endregion

#region Take
    public Result TryTake([MaybeNullWhen(false)] out T taken)
    {
        int index = _position;
        if (index < Length)
        {
            _position = index + 1;
            taken = _span[index];
            return true;
        }

        taken = default;
        return new InvalidOperationException("Cannot take an item: No items remain");
    }

    public Result TryTake(int count, out ReadOnlySpan<T> taken)
    {
        if (count <= 0)
        {
            taken = default;
            return true;
        }

        int index = _position;
        int newIndex = index + count;
        if (newIndex <= Length)
        {
            _position = newIndex;
            taken = _span.Slice(index, count);
            return true;
        }

        taken = default;
        return new InvalidOperationException($"Cannot take {count} items: Only {RemainingLength} items remain");
    }

    public T Take()
    {
        var result = TryTake(out var value);
        result.ThrowIfError();
        return value!;
    }

    public ReadOnlySpan<T> Take(int count)
    {
        var result = TryPeek(count, out var values);
        result.ThrowIfError();
        return values;
    }

    public ReadOnlySpan<T> TakeWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = Length;
        while (index < len && itemPredicate(span[index]))
        {
            index += 1;
        }
        _position = index;
        return span.Slice(start, index - start);
    }

    public ReadOnlySpan<T> TakeWhile(T match) => TakeWhile(item => EqualityComparer<T>.Default.Equals(item, match));

    public ReadOnlySpan<T> TakeUntil(Func<T, bool> itemPredicate) => TakeWhile(item => !itemPredicate(item));

    public ReadOnlySpan<T> TakeUntil(T match) => TakeWhile(item => !EqualityComparer<T>.Default.Equals(item, match));

    public ReadOnlySpan<T> TakeAny(T[] matches) => TakeWhile(item => matches.Contains(item));

    public ReadOnlySpan<T> TakeAny(IReadOnlyCollection<T> matches) => TakeWhile(item => matches.Contains(item));

    public ReadOnlySpan<T> TakeAll() => TakeWhile(static _ => true);
#endregion

#region IEnumerator Support
    [EditorBrowsable(EditorBrowsableState.Never)]
    public T Current => _span[_position];

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool MoveNext() => TrySkip();
#endregion

    public override string ToString()
    {
        /* We want to show our position in the source span like this:
         * ...,a,b,c ⌖ d,e,f,...
         * For ReadOnlySpan<char>, we have special handling to treat it like text and capture more characters
         */

        string delimiter;
        int capture;
        if (typeof(T) == typeof(char))
        {
            delimiter = string.Empty;
            capture = 16;
        }
        else
        {
            delimiter = ",";
            capture = 4;
        }

        var text = StringBuilderPool.Rent();

        int index = _position;
        var span = _span;

        // Previously read items
        int prevIndex = index - capture;
        // If we have more before this, indicate with ellipsis
        if (prevIndex > 0)
        {
            text.Append('…').Append(delimiter);
        }
        // Otherwise, cap at a min zero
        else
        {
            prevIndex = 0;
        }

        for (var i = prevIndex; i < index; i++)
        {
            if (i > prevIndex)
            {
                text.Append(delimiter);
            }
            text.Append<T>(span[i]);
        }

        // position indicator
        text.Append(" ⌖ ");

        // items yet to be read
        int nextIndex = index + capture;

        // if we have more after, we're going to end with an ellipsis
        bool postpendEllipsis;
        // but we also need to cap at capacity
        if (nextIndex < span.Length)
        {
            postpendEllipsis = true;
        }
        else
        {
            postpendEllipsis = false;
            nextIndex = span.Length;
        }

        for (var i = index; i < nextIndex; i++)
        {
            if (i > index)
            {
                text.Append(delimiter);
            }
            text.Append<T>(span[i]);
        }

        if (postpendEllipsis)
        {
            text.Append(delimiter)
                .Append('…');
        }

        return text.ToStringAndReturn();
    }
}