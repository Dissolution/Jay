using System.Collections.Concurrent;

namespace Jay.Collections;

/// <summary>
/// A <see cref="ConcurrentDictionary{TKey,TValue}"/> where <c>TKey</c> is <see cref="Type"/>
/// </summary>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of values associated with the <see cref="Type"/> keys
/// </typeparam>
public class ConcurrentTypeMap<TValue> : ConcurrentDictionary<Type, TValue>
{
    public ConcurrentTypeMap()
        : base()
    {
    }

    public ConcurrentTypeMap(int capacity)
        : base(Environment.ProcessorCount, capacity)
    {
    }

    public bool ContainsKey<T>()
        => base.ContainsKey(typeof(T));

    public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value)
        => base.TryGetValue(typeof(T), out value);

    public TValue GetOrAdd<T>(TValue addValue)
        => base.GetOrAdd(typeof(T), addValue);

    public TValue GetOrAdd<T>(Func<Type, TValue> addValue)
        => base.GetOrAdd(typeof(T), addValue);

    public TValue AddOrUpdate(Type type, TValue value)
        => base.AddOrUpdate(type, value, (_, _) => value);

    public TValue AddOrUpdate<T>(TValue value)
        => base.AddOrUpdate(typeof(T), value, (_, _) => value);

    public TValue AddOrUpdate<T>(TValue addValue, Func<TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(T), _ => addValue, (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<T>(TValue addValue, Func<Type, TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(T), _ => addValue, updateValue);

    public TValue AddOrUpdate<T>(Func<TValue> createValue, Func<TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(T), _ => createValue(), (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<T>(Func<TValue> createValue, Func<Type, TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(T), _ => createValue(), updateValue);

    public TValue AddOrUpdate<T>(Func<Type, TValue> createValue, Func<TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(T), createValue, (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<T>(Func<Type, TValue> createValue, Func<Type, TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(T), createValue, updateValue);
}