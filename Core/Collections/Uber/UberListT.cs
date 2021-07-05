/*using System;
using System.Buffers;

public static partial class UberList
{
    public static T[] RentForArray<T>(Action<UberList<T>> tempList)
    {
        throw new NotImplementedException();
    }
}

public partial class UberList<T> : IDisposable
{
    private static readonly ArrayPool<T> _arrayPool = ArrayPool<T>.Shared;

    private T[] _items;
    private int _count;
    private bool _disposed;

    private Span<T> Written => new Span<T>(_items, 0, _count);
    private Span<T> Available => new Span<T>(_items, _count, _items.Length - _count);
    
    public UberList(int capacity = 64)
    {
        if (capacity <= 0)
        {
            _items = _arrayPool.Rent(64);
        }
        else
        {
            _items = _arrayPool.Rent(capacity);
        }
        _count = 0;
    }

    private int Adding(int count)
    {
        var index = _count;
        var newLen = index + count;
        if (newLen > _items.Length)
        {
            var array = _arrayPool.Rent(newLen * 2);
            Written.CopyTo(array);
            _items = array;
        }
        _count = newLen;
        return index;
    }
    
    public UberList<T> Add(T item)
    {
        _items[Adding(1)] = item;
        return this;
    }
    
    public UberList<TSelect> Select<TSelect>(Func<T, TSelect> transform)
    {
        
    }
    
    public UberList<T> Where(Func<T, bool> predicate)
    {
        if (_count > 0)
        {
            int empty = 0;
            int index = 0;
            var items = _items;
            T item;
            while (index < _count)
            {
                item = items[index];
                if (predicate(item))
                {
                    if (index != empty)
                    {
                        items[empty] = item;
                    }
                    empty++;
                }
                index++;
            }
            _count = empty;
        }
        return this;
    }

    public T[] ToArray()
    {
        var array = new T[_count];
        Written.CopyTo(array);
        return array;
    }
}*/