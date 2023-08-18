namespace Jay.Collections;

public class DistinctList<T>
{
    private readonly List<T> _items;

    public IEqualityComparer<T> ItemComparer { get; }

    public DistinctList(IEqualityComparer<T>? itemComparer = null)
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