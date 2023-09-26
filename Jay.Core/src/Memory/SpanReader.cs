using System.ComponentModel;
using Jay.Text;

namespace Jay.Memory;

/// <summary>
/// A <see cref="SpanReader{T}"/> wraps a <see cref="ReadOnlySpan{T}"/>
/// and provides methods to Peek, Skip, and Take <typeparamref name="T"/> value(s) from it
/// in forward-only reads
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

    internal int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }

    public int RemainingLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length - _position;
    }

    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _position = value;
    }

    public ReadOnlySpan<T> Remaining
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[_position..];
    }

    public SpanReader(ReadOnlySpan<T> span)
    {
        _span = span;
        _position = 0;
    }

#region Peek
    public bool TryPeek([MaybeNullWhen(false)] out T item)
    {
        if (_position < Length)
        {
            item = _span[_position];
            return true;
        }
        item = default;
        return false;
    }

    public bool TryPeek(int count, out ReadOnlySpan<T> items)
    {
        if ((_position + (uint)count) <= Length)
        {
            items = _span.Slice(_position, count);
            return true;
        }
        items = default;
        return false;
    }

    public T Peek() => TryPeek(out var item) ? item : throw new InvalidOperationException("No more items");

    public ReadOnlySpan<T> Peek(int count) => TryPeek(count, out var items) ? items : throw new InvalidOperationException("No more items");
#endregion

#region Skip
    public bool TrySkip()
    {
        int index = _position;
        if (index < Length)
        {
            _position = index + 1;
            return true;
        }
        return false;
    }

    public bool TrySkip(int count)
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
        return false;
    }

    public void Skip()
    {
        if (!TrySkip())
            throw new InvalidOperationException("No more items");
    }

    public void Skip(int count)
    {
        if (!TrySkip(count))
            throw new InvalidOperationException("No more items");
    }

    public void SkipWhile(Func<T, bool> itemPredicate)
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
    }

    public void SkipWhile(T match) => SkipWhile(item => EqualityComparer<T>.Default.Equals(item, match));

    public void SkipUntil(Func<T, bool> itemPredicate) => SkipWhile(item => !itemPredicate(item));

    public void SkipUntil(T match) => SkipWhile(item => !EqualityComparer<T>.Default.Equals(item, match));

    public void SkipAny(params T[] matches) => SkipWhile(item => matches.Contains(item));

    public void SkipAny(IReadOnlyCollection<T> matches) => SkipWhile(item => matches.Contains(item));

    public void SkipAll() => SkipWhile(static _ => true);
#endregion

#region Take
    public bool TryTake([MaybeNullWhen(false)] out T taken)
    {
        int index = _position;
        if (index < Length)
        {
            _position = index + 1;
            taken = _span[index];
            return true;
        }

        taken = default;
        return false;
    }

    public bool TryTake(int count, out ReadOnlySpan<T> taken)
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
        return false;
    }

    public T Take() => TryTake(out var taken) ? taken : throw new InvalidOperationException("No more items");

    public ReadOnlySpan<T> Take(int count) => TryTake(count, out var taken) ? taken : throw new InvalidOperationException("No more items");

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
         * ...abcdef|ghijkl...
         */

        var builder = StringBuilderPool.Shared.Rent();
        int index = _position;
        var span = _span;

        // We do not want to delimit Span<char>
        var delimiter = typeof(T) == typeof(char) ? "" : ",";

        // previous characters
        int prev = index - 16;
        if (prev > 0)
        {
            builder.Append('…');
        }
        else
        {
            prev = 0;
        }
        for (var i = prev; i < index; i++)
        {
            if (i > prev)
            {
                builder.Append(delimiter);
            }
            builder.Append<T>(span[i]);
        }

        // position indicator
        builder.Append('|');

        // next characters
        var end = span.Length;
        int next = index + 16;
        bool postpend;
        if (next < end)
        {
            postpend = true;
        }
        else
        {
            postpend = false;
            next = end;
        }

        for (var i = index; i < next; i++)
        {
            if (i > index)
            {
                builder.Append(delimiter);
            }
            builder.Append<T>(span[i]);
        }
        if (postpend)
        {
            builder.Append('…');
        }

        return builder.ToStringAndReturn();
    }
}