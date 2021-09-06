/*using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

public class TempList : IList, ICollection, IEnumerable
{
    protected static readonly ArrayPool<object> _pool = ArrayPool<object>.Create();
}

public class TempList<T> : TempList, 
                           IList<T>, IReadOnlyList<T>,
                           ICollection<T>, IReadOnlyCollection<T>,
                           IEnumerable<T>,
                           IDisposable
{
    private object[] _array;
    private int _length;

    public T this[int index]
    {
        get => (T) _array[index];
        set => _array[index] = (object) value;
    }

    public int Count => _length;

    internal TempList()
    {
        _array = _pool.Rent(1024);
        _length = 0;
    }

    public void Add(T item)
    {
        if (_length == _array.Length)
        {
            var temp = _pool.Rent(_length * 2);
            Array.Copy(_array, 0, temp, 0, _length);
            _pool.Return(_array, true);
            _array = temp;
        }
        _array[_length++] = item;
    }

    public void Clear()
    {
        _length = 0;
        _pool.Return(_array, true);
        _array = Array.Empty<object>();
    }
    
    public bool Contains(T item)
    {
        for (var i = 0; i < _length; i++)
        {
            if (_array[i] is T value && EqualityComparer<T>.Default.Equals(value, item))
                return true;
        }
        return false;
    }
    public bool Contains(T item, IEqualityComparer<T> itemComparer)
    {
        for (var i = 0; i < _length; i++)
        {
            if (_array[i] is T value && itemComparer.Equals(value, item))
                return true;
        }
        return false;
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex = 0)
    {
        if (array is null)
            throw new ArgumentNullException(nameof(array));
        if ((uint) arrayIndex > (uint) array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (_length + arrayIndex > array.Length)
            throw new ArgumentException("", nameof(array));
        for (var i = 0; i < _length; i++)
        {
            array[arrayIndex + i] = (T) _array[i];
        }
    }
}*/