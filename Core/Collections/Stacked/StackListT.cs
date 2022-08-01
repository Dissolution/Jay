using System.Buffers;
using Jay.Comparision;
using Jay.Dumping;
using Jay.Text;

namespace Jay.Collections.Stacked;

public ref struct StackList<T>
    where T : IEquatable<T>
{
    public static implicit operator StackList<T>(Span<T> buffer) => new StackList<T>(buffer);
    
    internal const int DefaultCapacity = 1024;
    
    private T[]? _array;
    private Span<T> _span;
    private int _length;

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)index >= (uint)_length)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Index '{index}' must be between 0 and {_length - 1}");
            return ref _span[index];
        }
    }

    public int Capacity => _span.Length;
    
    public int Length => _length;

    public StackList(int capacity)
    {
        capacity = Math.Clamp(capacity, DefaultCapacity, Array.MaxLength);
        _span = _array = ArrayPool<T>.Shared.Rent(capacity);
        _length = 0;
    }

    public StackList(Span<T> buffer)
    {
        _span = buffer;
        _array = null;
        _length = 0;
    }

    public void Dispose()
    {
        var array = _array;
        if (array is not null)
        {
            ArrayPool<T>.Shared.Return(array);
        }
        this = default;
    }

    public bool SequenceEquals(StackList<T> stackList)
    {
        return _span.SequenceEqual<T>(stackList._span);
    }

    public bool SequenceEquals(ReadOnlySpan<T> span)
    {
        return _span.SequenceEqual<T>(span);
    }
    
    public bool SequenceEquals(T[] array)
    {
        return _span.SequenceEqual<T>(array);
    }
    
    public bool SequenceEquals(IEnumerable<T> items)
    {
        return EnumerableEqualityComparer<T>.Default.Equals(_span, items);
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is T[] array) return SequenceEquals(array);
        if (obj is IEnumerable<T> enumerable) return SequenceEquals(enumerable);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create<T>(_span);
    }

    public override string ToString()
    {
        using var text = TextBuilder.Borrow();
        text.Append('[')
            .AppendDelimit<T>(",", _span, (tb, item) => tb.AppendDump(item))
            .Write(']');
        return text.ToString();
    }
}