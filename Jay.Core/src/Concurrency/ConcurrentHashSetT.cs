using System.Collections;
using System.Collections.Concurrent;
using Jay.Utilities;

namespace Jay.Concurrency;

public class ConcurrentHashSet<T> :
    ConcurrentDictionary<T, Nothing>,
    //     ISet<T>,
    // #if NET6_0_OR_GREATER
    //     IReadOnlySet<T>,
    // #endif
    ICollection<T>, IReadOnlyCollection<T>,
    IEnumerable<T>
    where T : notnull
{
    bool ICollection<T>.IsReadOnly => false;

    public ConcurrentHashSet()
    {
    }

    public ConcurrentHashSet(IEqualityComparer<T>? comparer)
        : base(comparer ?? EqualityComparer<T>.Default)
    {
    }

    public bool TryAdd(T item) => base.TryAdd(item, default);

    void ICollection<T>.Add(T item)
    {
        if (base.TryAdd(item, default))
            throw new ArgumentException($"Cannot add {item}: Already exists", nameof(item));
    }

    public bool Contains(T item) => base.ContainsKey(item);

    public bool TryRemove(T item) => base.TryRemove(item, out _);

    bool ICollection<T>.Remove(T item) => base.TryRemove(item, out _);

    public void CopyTo(T[] array, int arrayIndex = default)
    {
        Validate.CanCopyTo(base.Count, array, arrayIndex);
        base.Keys.CopyTo(array, arrayIndex);
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetEnumerator();

    public new IEnumerator<T> GetEnumerator() => base.Keys.GetEnumerator();
}