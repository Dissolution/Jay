using System.Collections;

namespace Jay.Collections;

/// <summary>
/// An adapter for <see cref="IDictionary"/> to a typed <see cref="IDictionary{TKey,TValue}"/>
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class DictionaryAdapter<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
{
    [return: NotNullIfNotNull(nameof(objKey))]
    private static TKey ConvertObjKey(
        [AllowNull, NotNull] object? objKey,
        [CallerArgumentExpression(nameof(objKey))]
        string? keyName = null)
    {
        return objKey switch
        {
            null => throw new ArgumentNullException(keyName, "Key cannot be null"),
            TKey key => key,
            _ => throw new ArgumentException($"Key '{objKey}' is not a '{typeof(TKey).Name}'")
        };
    }
    [return: NotNullIfNotNull(nameof(objValue))]
    private static TValue ConvertObjValue(object? objValue)
    {
        if (objValue.CanBe<TValue>(out var value))
            return value!;
        throw new ArgumentException($"Value '{objValue}' is not a '{typeof(TValue).Name}'");
    }


    private readonly IDictionary _dictionary;

    public TValue this[TKey key]
    {
        get => ConvertObjValue(_dictionary[key]);
        set => _dictionary[key] = value;
    }


    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys.Cast<TKey>().ToHashSet();
#if NETSTANDARD2_0 || NETSTANDARD2_1
    public IReadOnlyCollection<TKey> Keys => _dictionary.Keys.Cast<TKey>().ToHashSet();
#else
    public IReadOnlySet<TKey> Keys => _dictionary.Keys.Cast<TKey>().ToHashSet();
#endif

    ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values.Cast<TValue>().ToList();

    public IReadOnlyCollection<TValue> Values => _dictionary.Values.Cast<TValue>().ToList();

    public int Count => _dictionary.Count;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _dictionary.IsReadOnly;

    public DictionaryAdapter(IDictionary dictionary)
    {
        _dictionary = dictionary;
    }

    public void Add(TKey key, TValue value) => _dictionary.Add((object)key, (object?)value);

    public void Add(KeyValuePair<TKey, TValue> pair) => _dictionary.Add((object)pair.Key, (object?)pair.Value);

    public bool ContainsKey(TKey key)
    {
        return _dictionary.Contains((object)key);
    }

    public bool Contains(TKey key, TValue value)
    {
        if (!_dictionary.Contains((object)key)) return false;
        var existingValue = ConvertObjValue(_dictionary[key]);
        return EqualityComparer<TValue>.Default.Equals(existingValue, value);
    }

    public bool Contains(KeyValuePair<TKey, TValue> pair)
    {
        if (!_dictionary.Contains((object)pair.Key)) return false;
        var existingValue = ConvertObjValue(_dictionary[pair.Key]);
        return EqualityComparer<TValue>.Default.Equals(existingValue, pair.Value);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_dictionary.Contains(key))
        {
            value = ConvertObjValue(_dictionary[key]);
            return true;
        }
        value = default;
        return false;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        Validate.ThrowIfNull(array);
        if ((uint)arrayIndex + Count > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        foreach (DictionaryEntry entry in _dictionary)
        {
            array[arrayIndex++] = new(ConvertObjKey(entry.Key), ConvertObjValue(entry.Value));
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
        .Select(entry => new KeyValuePair<TKey, TValue>(ConvertObjKey(entry.Key), ConvertObjValue(entry.Value)))
        .GetEnumerator();
}