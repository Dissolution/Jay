using System.Collections.Concurrent;
using Jay.Utilities;

namespace Jay.Collections;

public sealed class ConcurrentHashSet<T> : IQuasiSet<T>
    where T : notnull
{
    private readonly ConcurrentDictionary<T, Nothing> _dictionary;
    private readonly IEqualityComparer<T> _valueComparer;
    
    public int Count => _dictionary.Count;

    public IEqualityComparer<T> EqualityComparer => _valueComparer;

    public ConcurrentHashSet(IEqualityComparer<T>? valueComparer = null)
    {
        _valueComparer = valueComparer ?? EqualityComparer<T>.Default;
        _dictionary = new(_valueComparer);
    }
    
    public bool TryAdd(T value)
    {
        return _dictionary.TryAdd(value, default);
    }

    public void AddOrUpdate(T value)
    {
        _dictionary.AddOrUpdate(value, default);
    }

    public bool Contains(T value)
    {
        return _dictionary.ContainsKey(value);
    }

    public bool TryRemove(T value)
    {
        return _dictionary.TryRemove(value, out _);
    }

    public void Clear()
    {
        _dictionary.Clear();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator() => _dictionary.Keys.GetEnumerator();
}