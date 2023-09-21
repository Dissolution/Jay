using Jay.Text;

namespace Jay.Memory;

public ref struct SpanWriter<T>
{
    public static implicit operator SpanWriter<T>(Span<T> span) => new(span);
    
    private readonly Span<T> _span;
    private int _position;

    public int Position
    {
        get => _position;
        set => _position = value;
    }
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }
    
    public Span<T> WrittenSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[.._position];
    }

    public Span<T> RemainingSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[_position..];
    }

    public SpanWriter(Span<T> span)
    {
        _span = span;
        _position = 0;
    }

    public bool TryWrite(T item)
    {
        int index = _position;
        if (index < Capacity)
        {
            _span[index] = item;
            _position = index + 1;
            return true;
        }
        return false;
    }
    
    public bool TryWrite(params T[]? items)
    {
        if (items is null)
            return true;
        return TryWrite((ReadOnlySpan<T>)items);
    }
    
    public bool TryWrite(ReadOnlySpan<T> items)
    {
        if (items.TryCopyTo(RemainingSpan))
        {
            _position += items.Length;
            return true;
        }
        return false;
    }

    public bool TryWrite(IEnumerable<T> items)
    {
        int pos = _position;
        var remaining = _span[pos..];
        if (items is IReadOnlyList<T> readOnlyList)
        {
            int itemsCount = readOnlyList.Count;
            int newPos = pos + itemsCount;
            if (newPos > remaining.Length)
                return false;
            for (var i = 0; i < itemsCount; i++)
            {
                remaining[i] = readOnlyList[i];
            }
            _position = newPos;
            return true;
        }
        else if (items is IList<T> list)
        {
            int itemsCount = list.Count;
            int newPos = pos + itemsCount;
            if (newPos > remaining.Length)
                return false;
            for (var i = 0; i < itemsCount; i++)
            {
                remaining[i] = list[i];
            }
            _position = newPos;
            return true;
        }
        else if (items is IReadOnlyCollection<T> readOnlyCollection)
        {
            int itemsCount = readOnlyCollection.Count;
            int newPos = pos + itemsCount;
            if (newPos > remaining.Length)
                return false;

            int r = 0;
            foreach (var item in readOnlyCollection)
            {
                remaining[r++] = item;
            }
            _position = newPos;
            return true;
        }
        else if (items is ICollection<T> collection)
        {
            int itemsCount = collection.Count;
            int newPos = pos + itemsCount;
            if (newPos > remaining.Length)
                return false;

            int r = 0;
            foreach (var item in collection)
            {
                remaining[r++] = item;
            }
            _position = newPos;
            return true;
        }
        else
        {
            // We cannot write something we don't know the length of
            return false;
        }
    }

    public void Write(T item)
    {
        if (!TryWrite(item))
            throw new InvalidOperationException($"Cannot write {item}: capacity is {RemainingSpan.Length}");
    }
    
    public void Write(params T[]? items)
    {
        if (!TryWrite(items))
            throw new InvalidOperationException($"Cannot write {items?.Length} items: capacity is {RemainingSpan.Length}");
    }
    
    public void Write(ReadOnlySpan<T> items)
    {
        if (!TryWrite(items))
            throw new InvalidOperationException($"Cannot write {items.Length} items: capacity is {RemainingSpan.Length}");
    }
    
    public void Write(IEnumerable<T> items)
    {
        if (!TryWrite(items))
            throw new InvalidOperationException($"Cannot write items: capacity is {RemainingSpan.Length}");
    }

    public bool TryAllocate(int count, out Span<T> allocated)
    {
        var remaining = this.RemainingSpan;
        if ((uint)count > remaining.Length)
        {
            allocated = default;
            return false;
        }
        allocated = remaining[..count];
        _position += count;
        return true;
    }

    public override string ToString()
    {
        var written = this.WrittenSpan;
        var writtenCount = written.Length;
        if (writtenCount == 0)
            return "";
        
        var text = StringBuilderPool.Shared.Rent();
        
        // We do not want to delimit Span<char>
        var delimiter = typeof(T) == typeof(char) ? "" : ",";

        // our length is > 0
        text.Append(written[0]);
        for (var i = 1; i < writtenCount; i++)
        {
            text.Append(delimiter);
            text.Append(written[i]);
        }

        return text.ToStringAndReturn();
    }
}