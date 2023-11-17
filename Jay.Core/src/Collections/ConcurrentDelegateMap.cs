using System.Collections.Concurrent;
using Jay.Comparison;
using Jay.Reflection;
using Jay.Utilities;

namespace Jay.Collections;

public sealed class ConcurrentDelegateMap
{
    private static Type[] GetGenericTypes<TDelegate>()
        where TDelegate : Delegate
    {
        var typeArgs = typeof(TDelegate).GetGenericArguments();
        if (typeArgs.Length > 0)
            return typeArgs;
        throw new ArgumentException($"{typeof(TDelegate).NameOf()} does not contain at least one generic type", nameof(TDelegate));
    }

    private static bool TypesEqual(Type[] left, Type[] right)
    {
        if (ReferenceEquals(left, right)) return true;
        int len = left.Length;
        if (right.Length != len) return false;
        for (var i = 0; i < len; i++)
        {
            if (left[i] != right[i]) return false;
        }
        return true;
    }


    private readonly ConcurrentDictionary<Type[], Delegate> _delegates = new(
        comparer: EqualityComparerCache.CreateKeyComparer<Type[]>(
            static (left, right) => TypesEqual(left, right),
            static types => Hasher.Combine(types)));

    public int Count => _delegates.Count;

    public bool TryAdd<TDelegate>(TDelegate @delegate)
        where TDelegate : Delegate
    {
        return _delegates.TryAdd(GetGenericTypes<TDelegate>(), @delegate);
    }

    public void Set<TDelegate>(TDelegate @delegate)
        where TDelegate : Delegate
    {
        _delegates.AddOrUpdate(GetGenericTypes<TDelegate>(), @delegate);
    }

    public bool Contains<TDelegate>()
        where TDelegate : Delegate
    {
        return _delegates.ContainsKey(GetGenericTypes<TDelegate>());
    }

    public bool Contains(params Type[] delegateGenericTypes)
    {
        return _delegates.ContainsKey(delegateGenericTypes);
    }
    
    public bool TryGet<TDelegate>([NotNullWhen(true)] out TDelegate? @delegate)
        where TDelegate : Delegate
    {
        if (_delegates.TryGetValue(GetGenericTypes<TDelegate>(), out var del))
        {
            return del.Is(out @delegate);
        }
        @delegate = null;
        return false;
    }

    public bool TryRemove<TDelegate>()
        where TDelegate : Delegate
    {
        return _delegates.TryRemove(GetGenericTypes<TDelegate>(), out _);
    }
}