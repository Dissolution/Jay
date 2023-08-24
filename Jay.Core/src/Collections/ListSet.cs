namespace Jay.Collections;

/// <summary>
/// A combination of <see cref="List{T}"/> and <see cref="HashSet{T}"/>,
/// <see cref="ListSet{T}"/> is a list that contains only one copy of each item.
/// It may be faster than <see cref="HashSet{T}"/> for small item counts.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListSet<T> 
    // :
    // IList<T>, IReadOnlyList<T>,
    // ISet<T>, IReadOnlySet<T>,
    // ICollection<T>, IReadOnlyCollection<T>,
    // IEnumerable<T>, IEnumerable
{
    private readonly List<T> _items;

    
    //bool ICollection<T>.IsReadOnly => false;

    
    public IEqualityComparer<T> ItemComparer { get; }

    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }
    
    public ListSet(IEqualityComparer<T>? itemComparer = null)
    {
        _items = new();
        ItemComparer = itemComparer ?? EqualityComparer<T>.Default;
    }

    public bool TryAdd(T item)
    {
        if (!_items.Any(existingItem => ItemComparer.Equals(existingItem, item)))
        {
            _items.Add(item);
            return true;
        }
        return false;
    }

    public bool TryRemove(T item)
    {
        for (var i = 0; i < _items.Count; i++)
        {
            if (ItemComparer.Equals(item, _items[i]))
            {
                _items.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}