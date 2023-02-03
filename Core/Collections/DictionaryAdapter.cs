using System.Collections;

namespace Jay.Collections; 

public sealed class DictionaryAdapter<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
{
    private static TKey GetKey([AllowNull, NotNull] object? objKey)
    {
        if (objKey is null)
            throw new ArgumentNullException(nameof(objKey), "Key cannot be null");
        if (objKey is TKey key)
            return key;
        throw new ArgumentException($"Key '{objKey}' is not a '{typeof(TKey).Name}'");
    }
    private static TValue GetValue(object? objValue)
    {
        if (objValue.CanBe<TValue>(out var value))
            return value!;
        throw new ArgumentException($"Value '{objValue}' is not a '{typeof(TValue).Name}'");
    }
    
    
    private readonly IDictionary _dictionary;

    public TValue this[TKey key]
    {
        get => GetValue(_dictionary[key]);
        set => _dictionary[key] = value;
    }



    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys.Cast<TKey>().ToHashSet();
    public IReadOnlySet<TKey> Keys => _dictionary.Keys.Cast<TKey>().ToHashSet();

    ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values.Cast<TValue>().ToList();
    public IReadOnlyCollection<TValue> Values => _dictionary.Values.Cast<TValue>().ToList();

    public int Count => _dictionary.Count;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _dictionary.IsReadOnly;

    public DictionaryAdapter(IDictionary dictionary)
    {
        _dictionary = dictionary;
    }

    public void Add(TKey key, TValue value) => _dictionary.Add(key, value);
    public void Add(KeyValuePair<TKey, TValue> pair) => _dictionary.Add(pair.Key, pair.Value);

    public bool ContainsKey(TKey key)
    {
        return _dictionary.Contains(key);
    }

    public bool Contains(TKey key, TValue value) =>
        _dictionary.Contains(key) &&
        EqualityComparer<TValue>.Default.Equals(GetValue(_dictionary[key]), value);
    public bool Contains(KeyValuePair<TKey, TValue> pair) =>
        _dictionary.Contains(pair.Key) &&
        EqualityComparer<TValue>.Default.Equals(GetValue(_dictionary[pair.Key]), pair.Value);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_dictionary.Contains(key))
        {
            value = GetValue(_dictionary[key]);
            return true;
        }
        value = default;
        return false;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        if ((uint)arrayIndex + Count > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        foreach (DictionaryEntry entry in _dictionary)
        {
            array[arrayIndex++] = new(GetKey(entry.Key), GetValue(entry.Value));
        }
    }

    public bool Remove(TKey key)
    {
        if (_dictionary.Contains(key))
        {
            _dictionary.Remove(key);
            return true;
        }
        return false;
    }

    public bool Remove(TKey key, TValue value)
    {
        if (Contains(key, value))
        {
            _dictionary.Remove(key);
            return true;
        }
        return false;
    }
    
    public bool Remove(KeyValuePair<TKey, TValue> pair)
    {
        if (Contains(pair))
        {
            _dictionary.Remove(pair.Key);
            return true;
        }
        return false;
    }

    public void Clear() => _dictionary.Clear();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary
            .Cast<DictionaryEntry>()
            .Select(entry => new KeyValuePair<TKey, TValue>(GetKey(entry.Key), GetValue(entry.Value)))
            .GetEnumerator();
}