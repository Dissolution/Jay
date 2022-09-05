using System.Buffers;
using System.Diagnostics;
using Jay.Validation;

namespace Jay.Collections;

public ref struct StackList<T>
{
    public static implicit operator StackList<T>(Span<T> buffer) => new StackList<T>(buffer);

    private Span<T> _span;
    private T[]? _arrayFromPool;
    private int _length;

        
    public ref T this[int index]
    {
        get
        {
            Validate.Index(index, _length);
            return ref _span[index];
        }
    }
        
    public int Length
    {
        get => _length;
        set
        {
            if (value < 0 || value > _span.Length)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Length may not be changed to below 0 or larger than current Capacity");
            _length = value;
        }
    }

    public int Capacity => _span.Length;

    public StackList(Span<T> initialSpan)
    {
        _span = initialSpan;
        _arrayFromPool = null;
        _length = 0;
    }

        
    private void Grow()
    {
        const int ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength
            
        // Double the size of the span.  If it's currently empty, default to size 4,
        // although it'll be increased in Rent to the pool's minimum bucket size.
        int nextCapacity = _span.Length != 0 ? _span.Length * 2 : 4;

        // If the computed doubled capacity exceeds the possible length of an array, then we
        // want to downgrade to either the maximum array length if that's large enough to hold
        // an additional item, or the current length + 1 if it's larger than the max length, in
        // which case it'll result in an OOM when calling Rent below.  In the exceedingly rare
        // case where _span.Length is already int.MaxValue (in which case it couldn't be a managed
        // array), just use that same value again and let it OOM in Rent as well.
        if ((uint)nextCapacity > ArrayMaxLength)
        {
            nextCapacity = Math.Max(Math.Max(_span.Length + 1, ArrayMaxLength), _span.Length);
        }

        T[] array = ArrayPool<T>.Shared.Rent(nextCapacity);
        _span[0.._length].CopyTo(array);

        T[]? toReturn = _arrayFromPool;
        _span = _arrayFromPool = array;
        if (toReturn != null)
        {
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }
        
    // Hide uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        Debug.Assert(_length == _span.Length);
        int pos = _length;
        Grow();
        _span[pos] = item;
        _length = pos + 1;
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        int pos = _length;

        // Workaround for https://github.com/dotnet/runtime/issues/72004
        Span<T> span = _span;
        if ((uint)pos < (uint)span.Length)
        {
            span[pos] = item;
            _length = pos + 1;
        }
        else
        {
            AddWithResize(item);
        }
    }

    public Span<T> AsSpan() => _span.Slice(0, _length);
        
    public ReadOnlySpan<T> AsReadOnlySpan() => _span.Slice(0, _length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _arrayFromPool;
        if (toReturn != null)
        {
            _arrayFromPool = null;
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }
}