using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Collections
{
    public interface ITypeDictionary<TValue> : IReadOnlyTypeDictionary<TValue>
    {
        bool TryAdd<T>(TValue value);
        bool TryRemove<T>();
        bool TryRemove(Type type);
        bool TryRemove<T>([MaybeNullWhen(false)] out TValue removedValue);
        
        void Set<T>(TValue value);
        
        TValue GetOrAdd<T>(TValue value);
        TValue GetOrAdd<T>(Func<TValue> valueFactory);
        TValue GetOrAdd(Type type, Func<Type, TValue> valueFactory);

        TValue AddOrUpdate<T>(TValue newValue, Func<TValue, TValue> updateValue);
        TValue AddOrUpdate<T>(Func<TValue> newValueFactory, Func<TValue, TValue> updateValue);
        TValue AddOrUpdate(Type type, Func<Type, TValue> newValueFactory, Func<Type, TValue, TValue> updateValue);
        
        IReadOnlyTypeDictionary<TValue> AsReadOnly();
    }
}