namespace Jay.Collections;

/// <summary>
/// A <see cref="Dictionary{TKey,TValue}"/> of <see cref="Type"/> and <typeparamref name="TValue"/>
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class TypeDictionary<TValue> : Dictionary<Type, TValue>
{
    public TypeDictionary()
        : base() { }
    public TypeDictionary(int capacity)
        : base(capacity) { }

    public bool ContainsKey<T>() => base.ContainsKey(typeof(T));

    public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value) => base.TryGetValue(typeof(T), out value);

    public TValue GetOrAdd<T>(TValue addValue)
    {
        if (base.TryGetValue(typeof(T), out var value))
            return value;
        base.Add(typeof(T), addValue);
        return addValue;
    }
    public TValue GetOrAdd<T>(Func<Type, TValue> addValue)
    {
        if (base.TryGetValue(typeof(T), out var value))
            return value;
        value = addValue(typeof(T));
        base.Add(typeof(T), value);
        return value;
    }

    public TValue AddOrUpdate<T>(TValue addValue, Func<TValue, TValue> updateValue)
    {
        return this.AddOrUpdate<Type, TValue>(typeof(T), addValue, (_, existing) => updateValue(existing));
    }
    public TValue AddOrUpdate<T>(TValue addValue, Func<Type, TValue, TValue> updateValue)
    {
        return this.AddOrUpdate(typeof(T), addValue, updateValue);
    }
    public TValue AddOrUpdate<T>(Func<Type, TValue> addValue, Func<TValue, TValue> updateValue)
    {
        return this.AddOrUpdate(typeof(T), addValue, (_, existing) => updateValue(existing));
    }
    public TValue AddOrUpdate<T>(Func<Type, TValue> addValue, Func<Type, TValue, TValue> updateValue)
    {
        return this.AddOrUpdate(typeof(T), addValue, updateValue);
    }

    public TValue Set<T>(TValue value)
    {
        return this.AddOrUpdate(typeof(T), value, (_, _) => value);
    }
}