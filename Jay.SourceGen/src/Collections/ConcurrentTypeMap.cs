using System.Collections.Concurrent;

namespace Jay.CodeGen.Collections;

/// <summary>
/// A <see cref="ConcurrentDictionary{TKey,TValue}"/> of <see cref="Type"/> and <typeparamref name="TValue"/>
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class ConcurrentTypeMap<TValue> : ConcurrentDictionary<Type, TValue>
{
    public ConcurrentTypeMap()
        : base() { }
    
    public ConcurrentTypeMap(int capacity)
        : base(Environment.ProcessorCount, capacity) { }
    
    public bool ContainsKey<T>() => base.ContainsKey(typeof(T));

    public bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value) => base.TryGetValue(typeof(T), out value);

    public void Add((Type key, TValue value) tuple)
    {
        base.TryAdd(tuple.key, tuple.value);
    }
    
    public TValue GetOrAdd<T>(TValue addValue)
    {
        return base.GetOrAdd(typeof(T), addValue);
    }
    
    public TValue GetOrAdd<T>(Func<Type, TValue> addValue)
    {
        return base.GetOrAdd(typeof(T), addValue);
    }

    public TValue AddOrUpdate<T>(TValue addValue, Func<TValue, TValue> updateValue)
    {
        return base.AddOrUpdate(typeof(T), _ => addValue, (_, existingValue) => updateValue(existingValue));
    }
    
    public TValue AddOrUpdate<T>(TValue addValue, Func<Type, TValue, TValue> updateValue)
    {
        return base.AddOrUpdate(typeof(T), _ => addValue, updateValue);
    }

    public TValue AddOrUpdate<T>(Func<TValue> createValue, Func<TValue, TValue> updateValue)
    {
        return base.AddOrUpdate(typeof(T), _ => createValue(), (_, existingValue) => updateValue(existingValue));
    }
    
    public TValue AddOrUpdate<T>(Func<TValue> createValue, Func<Type, TValue, TValue> updateValue)
    {
        return base.AddOrUpdate(typeof(T), _ => createValue(), updateValue);
    }
    
    public TValue AddOrUpdate<T>(Func<Type, TValue> createValue, Func<TValue, TValue> updateValue)
    {
        return base.AddOrUpdate(typeof(T), createValue, (_,existingValue) => updateValue(existingValue));
    }
    
    public TValue AddOrUpdate<T>(Func<Type, TValue> createValue, Func<Type, TValue, TValue> updateValue)
    {
        return base.AddOrUpdate(typeof(T), createValue, updateValue);
    }
    
    public TValue Set<T>(TValue value)
    {
        return base.AddOrUpdate(typeof(T), value, (_, _) => value);
    }

    public TValue Set(Type type, TValue value)
    {
        return base.AddOrUpdate(type, value, (_, _) => value);
    }
}