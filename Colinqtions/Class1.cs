using System.Collections;

namespace Colinqtions;

public interface IIterator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    object? IEnumerator.Current => (object?)this.Current;

    void IEnumerator.Reset()
    {
        if (!TryReset())
        {
            throw new InvalidOperationException("Cannot reset this Iterator");
        }
    }
    
    void IDisposable.Dispose() { }
    
    int Position { get; }
    
    bool TryMoveNext(out T? current);

    bool TryReset();
}

public interface IIterable<T> : IEnumerable<T>, IEnumerable
{
    IEnumerator IEnumerable.GetEnumerator() => this.GetIterator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetIterator();

    IIterator<T> GetIterator();
}

public interface IRoBunch<T> : IIterable<T>//, IReadOnlyCollection<T>
{
    int Count { get; }

    bool Contains(T item, IEqualityComparer<T>? itemComparer = null);
}

public interface IBunch<T> : IRoBunch<T>//, ICollection<T>
{
    int Capacity { get; }
    
    bool TryAdd(T item);
    bool TryRemove(T item);  
    
    void Clear();
}

public interface IExBunch<T> : IBunch<T> //, ICollection<T>
{
    int IBunch<T>.Capacity => int.MaxValue;
    
    void Add(T item);

    void AddAll(IIterable<T> items);
}

public interface IRoOrderedBunch<T> : IRoBunch<T>//, IReadOnlyList<T>
{
    T this[int position] { get; }

    bool TryGet(int position, out T item);

    int FirstPosOf(T item, IEqualityComparer<T>? itemComparer = null) => this.NextPosOf(0, item, itemComparer);
    int NextPosOf(int posOffset, T item, IEqualityComparer<T>? itemComparer = null);
    int PrevPosOf(int posOffset, T item, IEqualityComparer<T>? itemComparer = null);
    int LastPosOf(T item, IEqualityComparer<T>? itemComparer = null) => this.PrevPosOf(this.Count, item, itemComparer);
}

public interface IOrderedBunch<T> : IRoOrderedBunch<T>, IBunch<T>//, IList<T>
{
    T IRoOrderedBunch<T>.this[int position] => this[position];
    new T this[int position] { get; set; }

    bool TrySet(int position, T item);
}

public interface IExOrderedBunch<T> : IOrderedBunch<T>, IExBunch<T>//, IList<T>
{
    void Insert(int position, T item);
}