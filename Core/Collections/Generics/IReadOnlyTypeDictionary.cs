using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Collections
{
    public interface IReadOnlyTypeDictionary<TValue> : IReadOnlyCollection<KeyValuePair<Type, TValue>>
    {
        IReadOnlyCollection<Type> Types { get; }
        IReadOnlyCollection<TValue> Values { get; }
        
        bool Contains<T>();
        bool Contains(Type type);
        bool TryGetValue<T>([MaybeNullWhen(false)] out TValue value);
    }
}