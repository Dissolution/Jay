using System.Buffers;
using System.Diagnostics;
using Jay.Collections.Pooling;
using Jay.Utilities;

namespace Jay.Collections.Stacked;

public ref struct SpanList<T>
    /* roughly:
     * IList<T>, IReadOnlyList<T>,
     * ICollection<T>, IReadOnlyCollection<T>,
     * IEnumerable<T>,
     * IDisposable
     */
{
    private T[]? _arrayFromPool;
    private Span<T> _buffer;
    private int _count;


    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Validate.Index(_count, index);
            return ref _buffer[index];
        }
    }

    public Span<T> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            (int offset, int length) = Validate.RangeResolve(_count, range);
            return _buffer.Slice(offset, length);
        }
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            int oldCount = _count;
            Validate.Between(value, 0, oldCount);
            if (value == oldCount) return;
            if (TypeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _buffer[new Range(start: value, end: oldCount)].Clear();
            }
            _count = value;
        }
    }

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _buffer.Length;
    }

    public SpanList()
    {
        _buffer = _arrayFromPool = ArrayPool<T>.Shared.Rent(256);
        _count = 0;
    }
    public SpanList(int minCapacity)
    {
        _buffer = _arrayFromPool = ArrayPool<T>.Shared.Rent(Math.Max(minCapacity, 256));
        _count = 0;
    }
    public SpanList(Span<T> emptyBuffer)
    {
        _arrayFromPool = null;
        _buffer = emptyBuffer;
        _count = 0;
    }
    public SpanList(Span<T> buffer, int count)
    {
        Validate.Between(count, 0, buffer.Length);
        _arrayFromPool = null;
        _buffer = buffer;
        _count = count;
    }


    private void Grow()
    {
        const int ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Double the size of the span
        int nextCapacity = Capacity * 2;

        if ((uint)nextCapacity > ArrayMaxLength)
        {
            throw new InvalidOperationException();
        }

        T[] array = ArrayPool<T>.Shared.Rent(nextCapacity);
        TryCopyTo(array);

        T[]? toReturn = _arrayFromPool;
        _buffer = _arrayFromPool = array;
        if (toReturn != null)
        {
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    // Hide uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        Debug.Assert(_count == _buffer.Length);
        int pos = _count;
        Grow();
        _buffer[pos] = item;
        _count = pos + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        int index = _count;
        if (index < Capacity)
        {
            _buffer[index] = item;
            _count = index + 1;
        }
        else
        {
            AddWithResize(item);
        }
    }

    public void AddAll(params T[] items) => AddAll(items.AsSpan());
    public void AddAll(ReadOnlySpan<T> items)
    {
        int itemsCount = items.Length;
        int startIndex = _count;
        int endIndex = startIndex + itemsCount;
        if (endIndex > Capacity)
            throw new InvalidOperationException($"Adding {itemsCount} new items would exceed Capacity -- [{startIndex}+{itemsCount}>{Capacity}]");
        items.CopyTo(_buffer.Slice(startIndex));
        _count = endIndex;
    }
    public void AddAll(IEnumerable<T> items)
    {
        int index = _count;
        var buffer = _buffer;
        var capacity = buffer.Length;

        if (items is IList<T> list)
        {
            if (index + list.Count > capacity)
                throw new InvalidOperationException($"Adding {list.Count} new items would exceed Capacity -- [{index}+{list.Count}>{capacity}]");
            for (var i = 0; i < list.Count; i++, index++)
            {
                buffer[index] = list[i];
            }
        }
#if NET6_0_OR_GREATER
        else if (items.TryGetNonEnumeratedCount(out int itemsCount))
        {
            if (index + itemsCount > capacity)
                throw new InvalidOperationException($"Adding {itemsCount} new items would exceed Capacity -- [{index}+{itemsCount}>{capacity}]");
            foreach (var item in items)
            {
                buffer[index] = item;
                index += 1;
            }
        }
#else
        else if (items is ICollection<T> collection)
        {
            if (index + collection.Count > capacity)
                throw new InvalidOperationException(
                    $"Adding {collection.Count} new items would exceed Capacity -- [{index}+{collection.Count}>{capacity}]");
            foreach (var item in items)
            {
                buffer[index] = item;
                index += 1;
            }
        }
#endif
        else
        {
            foreach (var item in items)
            {
                if (index >= capacity)
                    throw new InvalidOperationException($"Adding a new item would exceed Capacity -- [{index}+1>{capacity}]");
                buffer[index] = item;
                index += 1;
            }
        }
        _count = index;
    }

    public void Insert(int index, T item)
    {
        Validate.InsertIndex(_count, index);
        int endIndex = _count;
        if (endIndex >= Capacity)
            throw new InvalidOperationException("Cannot insert a new Item: At Capacity");
        Span<T> buffer = _buffer;
        if (index != endIndex)
        {
            // Shift existing to the right to make a hole
            var span = buffer.Slice(index, endIndex - index);
            var dest = buffer.Slice(index + 1, span.Length);
            span.CopyTo(dest);
        }
        buffer[index] = item;
        _count = endIndex + 1;
    }

    public void Insert(int index, ReadOnlySpan<T> items)
    {
        int itemsCount = items.Length;
        Validate.InsertIndex(_count, index);
        int count = _count;
        if (count + itemsCount > Capacity)
            throw new InvalidOperationException($"Adding {itemsCount} new items would exceed Capacity -- [{_count}+{itemsCount}>{Capacity}]");
        Span<T> buffer = _buffer;
        if (index != count)
        {
            // Shift existing to the right to make a hole
            var span = buffer.Slice(index, count - index);
            var dest = buffer.Slice(index + itemsCount, span.Length);
            span.CopyTo(dest);
        }

        items.CopyTo(buffer.Slice(index, itemsCount));
        _count = count + itemsCount;
    }

    public Span<T> Allocate(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));
        int startIndex = _count;
        int endIndex = startIndex + count;
        if (endIndex > Capacity)
            throw new InvalidOperationException($"Allocating {count} new items would exceed Capacity -- [{startIndex}+{count}>{Capacity}]");
        _count = endIndex;
        return _buffer.Slice(startIndex, count);
    }

    public Span<T> AllocateAt(int index, int count)
    {
        Validate.StartLength(_count, index, count);
        int currentCount = _count;
        int newCount = currentCount + count;
        if (newCount > Capacity)
            throw new InvalidOperationException($"Allocating {count} new items would exceed Capacity -- [{_count}+{count}>{Capacity}]");

        Span<T> buffer = _buffer;
        if (index != currentCount)
        {
            // Shift existing to the right to make a hole
            var span = buffer.Slice(index, currentCount - index);
            var dest = buffer.Slice(index + count, span.Length);
            span.CopyTo(dest);
        }
        _count = newCount;
        return _buffer.Slice(index, count);
    }

    public bool Contains(T item, IEqualityComparer<T>? itemComparer = default)
    {
        int count = _count;
        var buffer = _buffer;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < count; i++)
        {
            if (itemComparer.Equals(buffer[i], item)) return true;
        }
        return false;
    }

    public int FirstIndexOf(T item, IEqualityComparer<T>? itemComparer = default)
    {
        int count = _count;
        var buffer = _buffer;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = 0; i < count; i++)
        {
            if (itemComparer.Equals(buffer[i], item))
                return i;
        }
        return -1;
    }

    public int LastIndexOf(T item, IEqualityComparer<T>? itemComparer = default)
    {
        int count = _count;
        var buffer = _buffer;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = count - 1; i >= 0; i--)
        {
            if (itemComparer.Equals(buffer[i], item))
                return i;
        }
        return -1;
    }

    public int NextIndexOf(int startIndex, T item, IEqualityComparer<T>? itemComparer = default)
    {
        int count = _count;
        if ((uint)startIndex > (uint)count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        var buffer = _buffer;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = startIndex; i < count; i++)
        {
            if (itemComparer.Equals(buffer[i], item))
                return i;
        }
        return -1;
    }

    public int PrevIndexOf(int startIndex, T item, IEqualityComparer<T>? itemComparer = default)
    {
        int count = _count;
        if ((uint)startIndex >= (uint)count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        var buffer = _buffer;
        itemComparer ??= EqualityComparer<T>.Default;
        for (var i = startIndex; i >= 0; i--)
        {
            if (itemComparer.Equals(buffer[i], item))
                return i;
        }
        return -1;
    }

    public bool TryRemoveAt(int index)
    {
        int count = _count;
        var buffer = _buffer;
        if ((uint)index < (uint)count)
        {
            if (index == count - 1)
            {
                buffer[index] = default!;
            }
            else
            {
                // Shift existing to the left to cover the hole
                var span = buffer[new Range(index + 1, count)];
                var dest = buffer.Slice(index, span.Length);
                span.CopyTo(dest);
            }
            _count = index;
            return true;
        }
        return false;
    }

    public void Clear()
    {
        _buffer[.._count].Clear();
        _count = 0;
    }

    public SpanEnumerator<T> GetEnumerator()
    {
        return new SpanEnumerator<T>(AsSpan());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _buffer[.._count];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(int start)
    {
        int length = Validate.IndexGetLength(_count, start);
        return _buffer.Slice(start, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(int start, int length)
    {
        Validate.StartLength(_count, start, length);
        return _buffer.Slice(start, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(Range range)
    {
        (int offset, int length) = Validate.RangeResolve(_count, range);
        return _buffer.Slice(offset, length);
    }

    public bool TryCopyTo(Span<T> destination)
    {
        return AsSpan().TryCopyTo(destination);
    }

    public T[] ToArray()
    {
        T[] newArray = new T[_count];
        AsSpan().CopyTo(newArray);
        return newArray;
    }

    public SpanList<T> Clone()
    {
        var buffer = AsSpan();
        var clone = new SpanList<T>();
        buffer.CopyTo(clone._buffer);
        clone._count = _count;
        return clone;
    }

    public SpanList<T> Clone(Span<T> cloneBuffer)
    {
        var buffer = AsSpan();
        var clone = new SpanList<T>(cloneBuffer);
        buffer.CopyTo(clone._buffer);
        clone._count = _count;
        return clone;
    }

    public bool SequenceEqual(SpanList<T> spanList, IEqualityComparer<T>? itemComparer = default)
    {
        return AsSpan().SequenceEqual(spanList.AsSpan(), itemComparer);
    }

    public bool SequenceEqual(ReadOnlySpan<T> span, IEqualityComparer<T>? itemComparer = default)
    {
        return AsSpan().SequenceEqual(span, itemComparer);
    }

    public bool SequenceEqual(IEnumerable<T> items, IEqualityComparer<T>? itemComparer = default)
    {
        itemComparer ??= EqualityComparer<T>.Default;
        int count = _count;
        var buffer = _buffer.Slice(0, count);
        if (items is IList<T> list)
        {
            if (list.Count != count) return false;
            for (var i = 0; i < count; i++)
            {
                if (!itemComparer.Equals(buffer[i], list[i])) return false;
            }
        }
#if NET6_0_OR_GREATER
        else if (items.TryGetNonEnumeratedCount(out int itemsCount))
        {
            if (itemsCount != count) return false;
            using var e = items.GetEnumerator();
            for (var i = 0; i < count; i++)
            {
                e.MoveNext(); // Known to be good because of count verify
                if (!itemComparer.Equals(buffer[i], e.Current)) return false;
            }
        }
#else
        else if (items is ICollection<T> collection)
        {
            if (collection.Count != count) return false;
            using var e = items.GetEnumerator();
            for (var i = 0; i < count; i++)
            {
                e.MoveNext(); // Known to be good because of count verify
                if (!itemComparer.Equals(buffer[i], e.Current)) return false;
            }
        }
#endif
        else
        {
            using var e = items.GetEnumerator();
            for (var i = 0; i < count; i++)
            {
                if (!e.MoveNext()) return false; // too few items
                if (!itemComparer.Equals(buffer[i], e.Current)) return false;
            }
            if (e.MoveNext()) return false; // too many items
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _arrayFromPool;
        if (toReturn != null)
        {
            _arrayFromPool = null;
            ArrayPool<T>.Shared.Return(toReturn, true);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is T[] array) return SequenceEqual(array.AsSpan());
        if (obj is IEnumerable<T> items) return SequenceEqual(items);
        return false;
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException("SpanList is mutable and has no hash code");
    }

    public override string ToString()
    {
#if NET6_0_OR_GREATER
        var text = new DefaultInterpolatedStringHandler();
        text.AppendFormatted('[');
        int count = _count;
        if (count > 0)
        {
            var buffer = _buffer[..count];
            text.AppendFormatted(buffer[0]);
            for (var i = 1; i < count; i++)
            {
                text.AppendFormatted(',');
                text.AppendFormatted(buffer[i]);
            }
        }
        text.AppendFormatted(']');
        return text.ToStringAndClear();
#else
        var text = new StringBuilder();
        text.Append('[');
        int count = _count;
        if (count > 0)
        {
            var buffer = _buffer[..count];
            text.Append(buffer[0]);
            for (var i = 1; i < count; i++)
            {
                text.Append(',');
                text.Append(buffer[i]);
            }
        }
        text.Append(']');
        return text.ToString();
#endif


    }
}