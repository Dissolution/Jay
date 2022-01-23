using System.Collections.Concurrent;

namespace Jay.Collections;

public class ConcurrentTypeDictionary<TValue> : ConcurrentDictionary<Type, TValue>
{
    public bool ContainsKey<T>() => base.ContainsKey(typeof(T));

    public bool TryGetValue<T>(out TValue? value) => base.TryGetValue(typeof(T), out value);

    public TValue GetOrAdd<T>(TValue addValue)
    {
        return base.GetOrAdd(typeof(T), addValue);
    }
    public TValue GetOrAdd<T>(Func<Type, TValue> addValue)
    {
        return base.GetOrAdd(typeof(T), addValue);
    }

    public TValue AddOrUpdate<T>(TValue addValue, Func<Type, TValue, TValue> updateValue)
    {
        return base.AddOrUpdate(typeof(T), addValue, updateValue);
    }
    public TValue AddOrUpdate<T>(Func<Type, TValue> addValue, Func<Type, TValue, TValue> updateValue)
    {
        return base.AddOrUpdate(typeof(T), addValue, updateValue);
    }
}