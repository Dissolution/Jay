using System.Buffers;
using System.Collections;

namespace Jay.Collections;

/// <summary>
/// A <see cref="ListSet{T}"/> is a combination of an <see cref="IList{T}"/> and an <see cref="ISet{T}"/>
/// that may be faster than <see cref="ISet{T}"/> for small collection counts.<br/>
/// Borrowed from a shared <see cref="ArrayPool{T}"/>, <see cref="ListSet{T}"/> cannot grow beyond its starting
/// <see cref="Capacity"/>
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> if items stored
/// </typeparam>
public class ListSet<T> : 
    //IList<T>, IReadOnlyList<T>,
    //ISet<T>, IReadOnlySet<T>,
    ICollection<T>, IReadOnlyCollection<T>,
    IEnumerable<T>,
    IDisposable
{
    private T[] _items;
    private int _count;
    
    private readonly IEqualityComparer<T> _itemEqualityComparer;
    
    bool ICollection<T>.IsReadOnly => Count == Capacity;

    public IEqualityComparer<T> ItemEqualityComparer => _itemEqualityComparer;
    public int Count => _count;
    public int Capacity => _items.Length;
    
    public ListSet() : this(64, EqualityComparer<T>.Default) { }
    public ListSet(int minCapacity) : this(minCapacity, EqualityComparer<T>.Default) { }
    public ListSet(IEqualityComparer<T> itemEqualityComparer) : this(64, itemEqualityComparer) { }
    public ListSet(int minCapacity, IEqualityComparer<T> itemEqualityComparer)
    {
        _items = ArrayPool<T>.Shared.Rent(minCapacity);
        _count = 0;
        _itemEqualityComparer = itemEqualityComparer;
    }

    void ICollection<T>.Add(T item) => TryAdd(item);
    public bool TryAdd(T item)
    {
        var items = _items;
        var count = _count;
        if (count >= items.Length) return false;

        var comparer = _itemEqualityComparer;
        for (var i = 0; i < count; i++)
        {
            if (comparer.Equals(items[i], item))
                return false;
        }
        items[count] = item;
        _count = count + 1;
        return true;
    }

    bool ICollection<T>.Remove(T item) => TryRemove(item);

    public bool TryRemove(T item)
    {
        var count = _count;
        var items = _items.AsSpan(0, count);
        var comparer = _itemEqualityComparer;
        for (var i = 0; i < count; i++)
        {
            if (comparer.Equals(items[i], item))
            {
                items.RemoveAt(i);
                _count = count - 1;
                return true;
            }
        }
        return false;
    }
    

    public bool Contains(T item)
    {
        var items = _items;
        var count = _count;
        var comparer = _itemEqualityComparer;
        for (var i = 0; i < count; i++)
        {
            if (comparer.Equals(items[i], item))
            {
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        // We do not clear the array as that is done when we are disposed
        _count = 0;
    }

    public void CopyTo(T[] array, int arrayIndex = 0)
    {
        Validate.CanCopyTo(_count, array, arrayIndex);
        Easy.CopyTo(_items.AsSpan(0, _count), array.AsSpan(arrayIndex));
    }

    public bool TryCopyTo(Span<T> span)
    {
        if (ValidateResult.CanCopyTo(_count, span))
        {
            Easy.CopyTo(_items.AsSpan(0, _count), span);
            return true;
        }
        return false;
    }
   
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        int count = _count;
        var items = _items;
        for (var i = 0; i < count; i++)
        {
            yield return items[i];
        }
    }
    
    public void Dispose()
    {
        T[]? arrayToReturn = _items;
        _items = null!;
        if (arrayToReturn is not null)
        {
            ArrayPool<T>.Shared.Return(arrayToReturn);
        }
    }
}