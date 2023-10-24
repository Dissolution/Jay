namespace Jay.Collections;

/// <summary>
/// A <see cref="Dictionary{TKey,TValue}">Dictionary&lt;Type,TValue&gt;</see>
/// </summary>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of value associated with a <see cref="Type"/> key
/// </typeparam>
public class TypeMap<TValue> : Dictionary<Type, TValue>
{
    public TypeMap()
        : base()
    {
    }

    public TypeMap(int capacity)
        : base(capacity)
    {
    }

    public bool ContainsKey<T>()
        => base.ContainsKey(typeof(T));

    public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value)
        => base.TryGetValue(typeof(T), out value);

    public TValue GetOrAdd<T>(TValue addValue)
        => this.GetOrAdd(typeof(T), addValue);

    public TValue GetOrAdd<T>(Func<Type, TValue> addValue)
        => this.GetOrAdd(typeof(T), addValue);

    public TValue AddOrUpdate<T>(TValue value)
        => this.AddOrUpdate(typeof(T), value);

    public TValue AddOrUpdate<T>(TValue addValue, Func<TValue, TValue> updateValue)
        => this.AddOrUpdate(typeof(T), _ => addValue, (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<T>(TValue addValue, Func<Type, TValue, TValue> updateValue)
        => this.AddOrUpdate(typeof(T), _ => addValue, updateValue);

    public TValue AddOrUpdate<T>(Func<TValue> createValue, Func<TValue, TValue> updateValue)
        => this.AddOrUpdate(typeof(T), _ => createValue(), (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<T>(Func<TValue> createValue, Func<Type, TValue, TValue> updateValue)
        => this.AddOrUpdate(typeof(T), _ => createValue(), updateValue);

    public TValue AddOrUpdate<T>(Func<Type, TValue> createValue, Func<TValue, TValue> updateValue)
        => this.AddOrUpdate(typeof(T), createValue, (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<T>(Func<Type, TValue> createValue, Func<Type, TValue, TValue> updateValue)
        => this.AddOrUpdate(typeof(T), createValue, updateValue);
}