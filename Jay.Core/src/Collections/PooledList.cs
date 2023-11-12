using System.Buffers;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Collections;

public sealed class PooledList<T> :
    IList<T>, IReadOnlyList<T>,
    ICollection<T>, IReadOnlyCollection<T>,
    IEnumerable<T>,
    IDisposable
{
    /// <summary>
    /// The rented array of items
    /// </summary>
    private T[] _array;
    /// <summary>
    /// The current number of items in the array that are used
    /// </summary>
    private int _count;

    
    /// <inheritdoc cref="ICollection{T}"/>
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc cref="IReadOnlyList{T}"/>
    T IReadOnlyList<T>.this[int index] => this[index];

    /// <inheritdoc cref="IList{T}"/>
    T IList<T>.this[int index]
    {
        get => this[index];
        set => this[index] = value;
    }
    
    /// <summary>
    /// Gets a reference to the item at <paramref name="index"/>
    /// </summary>
    /// <param name="index">
    /// The index of the item to reference
    /// </param>
    public ref T this[int index]
    {
        get
        {
            Validate.Index(_count, index);
            return ref _array[index];
        }
    }
    
    /// <summary>
    /// Gets the number of items in this <see cref="PooledList{T}"/>
    /// </summary>
    public int Count => _count;
    
    
    /// <summary>
    /// Construct a new <see cref="PooledList{T}"/>
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum starting capacity of the <see cref="PooledList{T}"/>
    /// </param>
    public PooledList(int minCapacity = 64)
    {
        _array = ArrayPool<T>.Shared.Rent(minCapacity);
    }

    private void GrowTo(int minCapacity)
    {
        var newArray = ArrayPool<T>.Shared.Rent(minCapacity);
        if (_array.Length > 0)
        {
            this.AsSpan().CopyTo(newArray);
            ArrayPool<T>.Shared.Return(_array, true);
        }
        _array = newArray;
    }
    
    /// <summary>
    /// Adds a <typeparamref name="T"/> <paramref name="item"/> to this <see cref="PooledList{T}"/>
    /// </summary>
    /// <param name="item">
    /// The <typeparamref name="T"/> item to add
    /// </param>
    public void Add(T item)
    {
        int count = _count;
        int newCount = count + 1;
        if (newCount > _array.Length)
        {
            GrowTo(newCount);
        }

        _array[count] = item;
        _count = newCount;
    }

    public void AddMany(params T[] items) => AddMany(items.AsSpan());
    
    public void AddMany(ReadOnlySpan<T> items)
    {
        int count = _count;
        int newCount = count + items.Length;
        if (newCount > _array.Length)
        {
            GrowTo(newCount);
        }

        items.CopyTo(_array.AsSpan(count));
        _count = newCount;
    }

    // ReSharper disable PossibleMultipleEnumeration
    public void AddMany(IEnumerable<T> items)
    {
        if (items.TryGetNonEnumeratedCount(out int itemCount))
        {
            int count = _count;
            int newCount = count + itemCount;
            if (newCount > _array.Length)
            {
                GrowTo(newCount);
            }

            var dest = _array.AsSpan(count, itemCount);
            int d = 0;
            foreach (var item in items)
            {
                dest[d] = item;
                d++;
            }
            _count = newCount;
        }
        else
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
    // ReSharper restore PossibleMultipleEnumeration
    
    /// <inheritdoc cref="ICollection{T}"/>
    bool ICollection<T>.Contains(T item) => Contains(item, default);

    public bool Contains(T item, IEqualityComparer<T>? itemComparer = default)
    {
        var array = _array;
        var end = _count;
        var comparer = itemComparer ?? EqualityComparer<T>.Default;
        for (var i = 0; i < end; i++)
        {
            if (comparer.Equals(array[i], item))
                return true;
        }
        return false;
    }

    /// <inheritdoc cref="IList{T}"/>
    int IList<T>.IndexOf(T item) => FirstIndexOf(item);

    public int FirstIndexOf(T item, IEqualityComparer<T>? itemComparer = default)
    {
        var array = _array;
        var end = _count;
        var comparer = itemComparer ?? EqualityComparer<T>.Default;
        for (var i = 0; i < end; i++)
        {
            if (comparer.Equals(array[i], item))
                return i;
        }
        return -1;
    }
    
    public int LastIndexOf(T item, IEqualityComparer<T>? itemComparer = default)
    {
        var array = _array;
        var comparer = itemComparer ?? EqualityComparer<T>.Default;
        for (var i = _count - 1; i >= 0; i--)
        {
            if (comparer.Equals(array[i], item))
                return i;
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        int count = _count;
        Validate.InsertIndex(count, index);
        int newCount = count + 1;
        var array = _array;
        if (newCount > array.Length)
        {
            GrowTo(newCount);
        }

        // Take everything at index and to the right
        var source = array.AsSpan(index, count - index);
        // Copy it one to the right
        var dest = array.AsSpan(index + 1);
        source.CopyTo(dest);
        
        // Then put the new item in the new space
        array[index] = item;
        // one bigger
        _count = newCount;
    }

    /// <inheritdoc cref="IList{T}"/>
    void IList<T>.RemoveAt(int index)
    {
        Validate.Index(_count, index);
        TryRemoveAt(index);
    }

    public bool TryRemoveAt(int index)
    {
        int count = _count;
        if (index < 0 || index >= count)
            return false;
        
        // Take everything to the right of index
        int rightStart = index + 1;
        var source = _array.AsSpan(rightStart, count - rightStart);
        // Copy it at index (erasing it)
        var dest = _array.AsSpan(index);
        source.CopyTo(dest);

        // one smaller
        _count = count - 1;
        return true;
    }

    public bool TryRemoveAt(Index index) => TryRemoveAt(index.GetOffset(_count));

    public bool TryRemoveMany(int offset, int length)
    {
        if (!ValidateResult.Range(_count, offset, length))
            return false;
        
        // Take everything to the right of the range
        int rightStart = offset + length;
        var source = _array.AsSpan(rightStart, length - rightStart);
        // Copy it at index (erasing it)
        var dest = _array.AsSpan(offset);
        source.CopyTo(dest);

        // smaller
        _count -= length;
        return true;
    }
    
    public bool TryRemoveMany(Range range)
    {
        (int offset, int length) = range.GetOffsetAndLength(_count);
        return TryRemoveMany(offset, length);
    }

    /// <inheritdoc cref="ICollection{T}"/>
    bool ICollection<T>.Remove(T item) => TryRemoveFirst(item);
    
    public bool TryRemoveFirst(T item, IEqualityComparer<T>? itemComparer = default)
    {
        var end = _count;
        var written = _array.AsSpan(0, end);
        var comparer = itemComparer ?? EqualityComparer<T>.Default;
        for (var i = 0; i < end; i++)
        {
            if (comparer.Equals(written[i], item))
            {
                return TryRemoveAt(i);
            }
        }
        return false;
    }

    /// <inheritdoc cref="ICollection{T}"/>
    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        if (!TryCopyTo(array.AsSpan(arrayIndex)))
            throw new InvalidOperationException("Cannot copy to array");
    }

    public void CopyTo(Span<T> buffer) => this.AsSpan().CopyTo(buffer);
    
    public bool TryCopyTo(Span<T> buffer) => this.AsSpan().TryCopyTo(buffer);

    /// <inheritdoc cref="ICollection{T}"/>
    void ICollection<T>.Clear()
    {
        _count = 0;
    }

    /// <summary>
    /// Removes all items from this <see cref="PooledList{T}"/>
    /// </summary>
    /// <param name="derefItems"><i>Optional</i><br/>
    /// <c>false</c> <i>(default)</i><br/>
    /// Sets the <see cref="Count"/> to 0, but does not de-reference any items<br/>
    /// They will not be de-referenced until overwritten or <see cref="Dispose"/> is called<br/>
    /// <c>true</c><br/>
    /// Sets the <see cref="Count"/> to 0 and sets all items to <c>default(T)</c>
    /// </param>
    public void Clear(bool derefItems = false)
    {
        if (derefItems)
        {
            AsSpan().Clear();
        }
        _count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _array.AsSpan(0, _count);

    /// <inheritdoc cref="IEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public IEnumerator<T> GetEnumerator()
    {
        var array = _array;
        var end = _count;
        for (var i = 0; i < end; i++)
        {
            yield return array[i];
        }
    }

    public void Dispose()
    {
        var toReturn = _array;
        _array = Array.Empty<T>();
        if (toReturn.Length > 0)
        {
            ArrayPool<T>.Shared.Return(toReturn, true);
        }
    }
    
    [Obsolete("Do not compare PooledList<T> to anything. If you want to compare its contents, use AsSpan()", true)]
    public override bool Equals(object? obj) => false;
    
    [Obsolete("Do not store PooledList<T> in a collection, it is intended to be Disposed", true)]
    public override int GetHashCode() => 0;

    public override string ToString()
    {
        return StringBuilderPool.Rent()
            .Append("PooledList<")
            .Append(typeof(T).NameOf())
            .Append(">[")
            .AppendDelimit<T>(", ", AsSpan())
            .Append(']')
            .ToStringAndReturn();
    }
}