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
    
    public Span<T> WrittenItems
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[.._position];
    }

    public Span<T> AvailableItems
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[_position..];
    }

    public SpanWriter(Span<T> span)
    {
        _span = span;
        _position = 0;
    }

    public Result TryWrite(T item)
    {
        int index = _position;
        var span = _span;
        if (index < span.Length)
        {
            span[index] = item;
            _position = index + 1;
            return true;
        }
        return new InvalidOperationException("Cannot add another item: No remaining capacity");
    }
    
    public Result TryWrite(params T[]? items)
    {
        if (items is null) return false;
        int itemsLen = items.Length;
        if (itemsLen == 0) return true;
        int index = _position;
        int newIndex = index + itemsLen;
        var span = _span;
        if (newIndex <= span.Length)
        {
            Easy.CopyTo(items, span[index..]);
            _position = newIndex;
            return true;
        }
        return new InvalidOperationException($"Cannot add {itemsLen} items: Only a capacity of {span.Length - index} remains");
    }
    
    public Result TryWrite(ReadOnlySpan<T> items)
    {
        int index = _position;
        int newIndex = index + items.Length;
        var span = _span;
        if (newIndex <= span.Length)
        {
            Easy.CopyTo(items, span[index..]);
            _position = newIndex;
            return true;
        }
        return new InvalidOperationException($"Cannot add {items.Length} items: Only a capacity of {span.Length - index} remains");
    }

    public Result TryWrite(IReadOnlyList<T> list)
    {
        int pos = _position;
        var remaining = _span[pos..];
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
    
    public Result TryWrite(IList<T> list)
    {
        int pos = _position;
        var remaining = _span[pos..];
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
    
    public Result TryWrite(IReadOnlyCollection<T> collection)
    {
        int pos = _position;
        var remaining = _span[pos..];
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
    
    public Result TryWrite(ICollection<T> collection)
    {
        int pos = _position;
        var remaining = _span[pos..];
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
    
    public void Write(T item) => TryWrite(item).ThrowIfError();

    public void Write(params T[]? items) => TryWrite(items).ThrowIfError();
    
    public void Write(ReadOnlySpan<T> items) => TryWrite(items).ThrowIfError();
    
    public Result TryAllocate(int count, out Span<T> allocated)
    {
        var remaining = this.AvailableItems;
        if ((uint)count <= remaining.Length)
        {
            allocated = remaining[..count];
            _position += count;
            return true;
        }
        allocated = default;
        return new InvalidOperationException($"Cannot allocate {count} items: Only a capacity of {remaining.Length} remains");
    }

    public override string ToString()
    {
        var written = this.WrittenItems;
        var writtenCount = written.Length;
        if (writtenCount == 0)
            return string.Empty;
        
        var text = StringBuilderPool.Rent();
        
        // We do not want to delimit Span<char>
        var delimiter = typeof(T) == typeof(char) ? "" : ",";

        // our length is > 0
        text.Append(written[0]);
        for (var i = 1; i < writtenCount; i++)
        {
            text.Append(delimiter)
                .Append(written[i]);
        }

        return text.ToStringAndReturn();
    }
}