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

internal abstract class Iterator<T> : IIterator<T>
{
    protected readonly IEnumerator<T> _enumerator;

    protected int _index;
    private T? _current;

    
    public int Index => _index;

    public abstract int? TotalCount { get; }

    public bool IsFirst => _index == 0;

    public abstract bool? IsLast { get; }
    
    object? IEnumerator.Current => _current;
    public T Current => _current!;
    
    protected Iterator(IEnumerator<T> enumerator)
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

internal class ListIterator<T> : IIterator<T>
{
    protected readonly IList<T> _list;

    private int _index;

    public int Index => _index;

    public int? TotalCount => _list.Count;

    public bool IsFirst => _index == 0;

    public bool? IsLast => _index == _list.Count - 1;

    object? IEnumerator.Current => this.Current;
    public T Current => _list[_index];
    
    public ListIterator(IList<T> list)
    {
        _list = list;
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
    
    public bool TryMoveNext([MaybeNullWhen(false)] out T current)
    {
        int index = _index + 1;
        if (index < _list.Count)
        {
            _index = index;
            current = _list[index];
            return true;
        }
        current = default;
        return false;
    }
    
    bool IEnumerator.MoveNext()
    {
        int index = _index + 1;
        if (index < _list.Count)
        {
            _index = index;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        _index = -1;
    }
}