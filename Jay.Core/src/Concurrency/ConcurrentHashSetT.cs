using System.Collections.Concurrent;
using Jay.Utilities;

namespace Jay.Concurrency;

public class ConcurrentHashSet<T> :
    ConcurrentDictionary<T, Nothing>
    where T : notnull
{
    public ConcurrentHashSet()
    {
    }

    public ConcurrentHashSet(IEqualityComparer<T>? comparer)
        : base(comparer ?? EqualityComparer<T>.Default)
    {
    }
    
    public bool TryAdd(T item) => base.TryAdd(item, default);

    public bool Contains(T item) => base.ContainsKey(item);
    
    public bool TryRemove(T item) => base.TryRemove(item, out _);

    public new IEnumerator<T> GetEnumerator() => base.Keys.GetEnumerator();
}