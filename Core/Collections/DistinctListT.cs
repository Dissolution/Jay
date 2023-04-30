namespace Jay.Collections;

public class DistinctList<T>
{
    private readonly List<T> _items;
    private readonly IEqualityComparer<T> _itemComparer;

    public IEqualityComparer<T> ItemComparer => _itemComparer;

    public DistinctList(IEqualityComparer<T>? itemComparer = null)
    {
        _items = new();
        this._itemComparer = itemComparer ?? EqualityComparer<T>.Default;
    }

    public bool TryAdd(T item)
    {
        if (!_items.Any(existingItem => _itemComparer.Equals(existingItem, item)))
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
            if (_itemComparer.Equals(item, _items[i]))
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