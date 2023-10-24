using System.Buffers;
using System.Collections;

namespace Jay.Collections;

public sealed class PooledList<T> :
    IList<T>, IReadOnlyList<T>,
    ICollection<T>, IReadOnlyCollection<T>,
    IEnumerable<T>,
    IDisposable
{
    private T[]? _array;
    private int _count;

    bool ICollection<T>.IsReadOnly => false;
    
    public int Count => _count;
    public int Capacity => _array?.Length ?? 0;
    
    public T this[int index]
    {
        get
        {
            Validate.IsNotNull(_array);
            Validate.Index(_count, index);
            return _array[index];
        }
        set
        {
            Validate.IsNotNull(_array);
            Validate.Index(_count, index);
            _array[index] = value;
        }
    }

    public PooledList()
    {
        _array = null;
    }

    public PooledList(int minCapacity)
    {
        _array = ArrayPool<T>.Shared.Rent(minCapacity);
    }

    public void Add(T item)
    {
        int count = _count;
        int newCount = count + 1;
        ref T[]? array = ref _array;
        if (array is null)
        {
            array = ArrayPool<T>.Shared.Rent(newCount);
        }
        else if (newCount > array.Length)
        {
            var newArray = ArrayPool<T>.Shared.Rent(newCount);
            Easy.CopyTo(array, newArray);
            ArrayPool<T>.Shared.Return(array);
            array = newArray;
        }

        array[count] = item;
        _count = newCount;
    }

    public bool Contains(T item)
    {
        var array = _array;
        var end = _count;
        for (var i = 0; i < end; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array![i], item))
                return true;
        }
        return false;
    }

    public int IndexOf(T item)
    {
        var array = _array;
        var end = _count;
        for (var i = 0; i < end; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array![i], item))
                return i;
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        Validate.InsertIndex(_count, index);
        int newCount = _count + 1;
        var array = _array!; // Will be non-null because count == 0 above will have thrown
        if (newCount > array.Length)
        {
            var newArray = ArrayPool<T>.Shared.Rent(newCount);
            Easy.CopyTo(array, newArray);
            ArrayPool<T>.Shared.Return(array);
            array = newArray;
        }

        array.AsSpan(new Range(start: index, end: _count))
            .CopyTo(array.AsSpan(index + 1));
        array[index] = item;
        _count = newCount;
    }

    public void RemoveAt(int index)
    {
        Validate.Index(_count, index);
        Easy.CopyTo(
            _array.AsSpan(new Range(start: index + 1, end: _count)),
            _array.AsSpan(index));
        _count -= 1;
    }
    
    public bool Remove(T item)
    {
        var end = _count;
        var written = _array.AsSpan(0, end);
        for (var i = 0; i < end; i++)
        {
            if (EqualityComparer<T>.Default.Equals(written[i], item))
            {
                Easy.CopyTo(written.Slice(i + 1), written.Slice(i));
                _count = end - 1;
                return true;
            }
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex = 0)
    {
        if (_count > 0)
        {
            Validate.CanCopyTo(_count, array, arrayIndex);
            Easy.CopyTo(_array.AsSpan(0, _count), array.AsSpan(arrayIndex));
        }
    }

    public void Clear()
    {
        _count = 0;
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator()
    {
        var array = _array;
        var end = _count;
        for (var i = 0; i < end; i++)
        {
            yield return array![i]; // count will be zero if array is null
        }
    }

    public void Dispose()
    {
        T[]? toReturn = _array;
        _array = null;
        if (toReturn is not null)
        {
            ArrayPool<T>.Shared.Return(toReturn, true);
        }
    }
}