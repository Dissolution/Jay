using System.Collections;

namespace Jay.Collections.Iteration;

public interface IIterable<T> : IEnumerable<T>, IEnumerable
{
#if !NETSTANDARD2_0
    IEnumerator IEnumerable.GetEnumerator() => this.GetIterator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetIterator();
#endif

    IIterator<T> GetIterator();
}

public interface IIterator<T> : IEnumerator<T>, IDisposable, IEnumerator
{
    int Index { get; }

    int? TotalCount { get; }

    bool IsFirst { get; }

    bool? IsLast { get; }

    bool TryMoveNext([MaybeNullWhen(false)] out T current);

    void Deconstruct(out T current);
    void Deconstruct(out int index, out T current);

#if !NETSTANDARD2_0
    void IDisposable.Dispose()
    {
    }
#endif
}

public static class IteratorExtensions
{
    public static IIterator<T> GetIterator<T>(this IEnumerable<T> enumerable)
    {
#if !NETSTANDARD2_0
        if (enumerable.TryGetNonEnumeratedCount(out int count))
        {
            return new Iterator<T>(enumerable.GetEnumerator())
            {
                TotalCount = count,
            };
        }
#else
        if (enumerable is ICollection<T> collection)
        {
            return new Iterator<T>(enumerable.GetEnumerator())
            {
                TotalCount = collection.Count,
            };
        }
#endif
        else
        {
            return new Iterator<T>(enumerable.GetEnumerator());
        }
    }
}

internal class Iterator<T> : IIterator<T>
{
    protected readonly IEnumerator<T> _enumerator;

    protected int _index;
    private T? _current;


    public int Index => _index;

    public int? TotalCount { get; init; } = null;

    public bool IsFirst => _index == 0;

    public bool? IsLast
    {
        get
        {
            if (TotalCount.TryGetValue(out var count))
                return _index == count - 1;
            return null;
        }
    }

    object? IEnumerator.Current => _current;

    public T Current => _current!;

    public Iterator(IEnumerator<T> enumerator)
    {
        Validate.NotNull(enumerator);
        _enumerator = enumerator;
        _index = -1;
    }

    public void Deconstruct(out T current)
    {
        current = this.Current;
    }
    public void Deconstruct(out int index, out T current)
    {
        index = _index;
        current = Current;
    }

    public virtual bool TryMoveNext([MaybeNullWhen(false)] out T current)
    {
        _index++;
        if (_enumerator.MoveNext())
        {
            current = _current = _enumerator.Current;
            return true;
        }
        current = _current = default;
        return false;
    }

    bool IEnumerator.MoveNext() => TryMoveNext(out _);

    public virtual void Reset()
    {
        _enumerator.Reset();
        _index = -1;
    }

    public virtual void Dispose()
    {
        _enumerator.Dispose();
    }
}