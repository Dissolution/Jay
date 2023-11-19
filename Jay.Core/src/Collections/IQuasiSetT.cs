namespace Jay.Collections;

public interface IQuasiSet<T> : IReadOnlyCollection<T>, IEnumerable<T>
    where T : notnull
{
    IEqualityComparer<T> EqualityComparer { get; }

    bool TryAdd(T value);
    void AddOrUpdate(T value);
    bool Contains(T value);
    bool TryRemove(T value);
    void Clear();
}