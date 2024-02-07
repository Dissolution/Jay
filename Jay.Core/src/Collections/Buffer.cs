using System.Buffers;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Collections;

/// <summary>
/// A <see cref="Buffer{T}"/> is a temporary <see cref="IList{T}"/> that uses <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
/// to manage its underlying storage<br/>
/// When finished using a <see cref="Buffer{T}"/>, it should be <see cref="Dispose">disposed</see>
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items stored in the <see cref="Buffer{T}"/>
/// </typeparam>
public class Buffer<T> :
    IList<T>, IReadOnlyList<T>,
    ICollection<T>, IReadOnlyCollection<T>,
    IEnumerable<T>,
    IDisposable,
    IBuffer<T>,
    IUnsafeBuffer<T>
{
    /// <summary>
    /// The minimum capacity for any Buffer
    /// </summary>
    private static int MinCapacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => typeof(T).IsClass ? 64 : 1024;
    }
    /// <summary>
    /// The maximum capacity for any Buffer
    /// </summary>
    private const int MAX_CAPACITY = 0x3FFFFFDF;


    /// <summary>
    /// The rented array of items
    /// </summary>
    private T[] _array;

    /// <summary>
    /// The current number of items in the array that are filled
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Throw.Index(_count, index);
            return ref _array[index];
        }
    }

    /// <summary>
    /// Gets a reference to the item at <paramref name="index"/>
    /// </summary>
    /// <param name="index">
    /// The <see cref="Index"/> of the item to reference
    /// </param>
    public ref T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _array[Throw.Index(_count, index)];
    }

    /// <summary>
    /// Gets the <see cref="Span{T}"/> of items at <paramref name="range"/>
    /// </summary>
    /// <param name="range">
    /// The <see cref="Range"/> of items to reference
    /// </param>
    public Span<T> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            (int start, int length) = Throw.Range(_count, range);
            return _array.AsSpan(start, length);
        }
    }

    /// <summary>
    /// Gets the number of items in this <see cref="Buffer{T}"/>
    /// </summary>
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _count = value.Clamp(0, Capacity);
    }

    /// <summary>
    /// Gets the current capacity for this <see cref="Buffer{T}"/><br/>
    /// It will automatically be increased as needed
    /// </summary>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.Length;
        set
        {
            int newCapacity = value.Clamp(MinCapacity, MAX_CAPACITY);
            var newArray = ArrayPool<T>.Shared.Rent(newCapacity);
            AsSpan().CopyTo(newArray);
            var toReturn = _array;
            _array = newArray;
            if (toReturn.Length > 0)
            {
                ArrayPool<T>.Shared.Return(toReturn, true);
            }
        }
    }


    /// <summary>
    /// Construct a new <see cref="Buffer{T}"/>
    /// </summary>
    public Buffer()
    {
        _array = ArrayPool<T>.Shared.Rent(MinCapacity);
    }

    /// <summary>
    /// Construct a new <see cref="Buffer{T}"/> with a minimum starting capacity
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum starting capacity of the <see cref="Buffer{T}"/>
    /// </param>
    public Buffer(int minCapacity)
    {
        _array = ArrayPool<T>.Shared.Rent(minCapacity.Clamp(MinCapacity, MAX_CAPACITY));
    }
    

#region Grow
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int adding)
    {
        int newCapacity = ((_array.Length + adding) * 2).Clamp(MinCapacity, MAX_CAPACITY);
        T[] newArray = ArrayPool<T>.Shared.Rent(newCapacity);
        _array.AsSpan(0, _count).CopyTo(newArray);

        T[] toReturn = _array;
        _array = newArray;

        if (toReturn.Length > 0)
        {
            ArrayPool<T>.Shared.Return(toReturn, true);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAdd(T item)
    {
        GrowBy(1);
        _array[_count] = item;
        _count += 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAdd(scoped ReadOnlySpan<T> items)
    {
        GrowBy(items.Length);
        items.CopyTo(_array.AsSpan(_count));
        _count += items.Length;
    }
#endregion

    public void IncreaseCapacity() => GrowBy(Capacity);

    /// <summary>
    /// Adds an <paramref name="item"/> to the end of this <see cref="Buffer{T}"/>
    /// </summary>
    /// <param name="item">
    /// The <typeparamref name="T"/> item to add
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        if (_count < _array.Length)
        {
            _array[_count] = item;
            _count++;
        }
        else
        {
            GrowAdd(item);
        }
    }

    public void AddMany(params T[]? items) => AddMany(items.AsSpan());

    public void AddMany(scoped ReadOnlySpan<T> items)
    {
        int count = _count;
        int newCount = count + items.Length;
        if (newCount <= _array.Length)
        {
            items.CopyTo(_array.AsSpan(count));
            _count = newCount;
        }
        else
        {
            GrowAdd(items);
        }
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
                GrowBy(itemCount);
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

    public void Insert(int index, T item)
    {
        int count = _count;
        Throw.Index(count, index, true);
        int newCount = count + 1;
        var array = _array;
        if (newCount > array.Length)
        {
            GrowBy(1);
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

#region Allocate
    public ref T Allocate()
    {
        int curLen = _count;
        int newLen = curLen + 1;
        // Check for growth
        if (newLen > _array.Length)
        {
            GrowBy(1);
        }

        // Add to our current position
        _count = newLen;
        // Return the allocated (at end of Written)
        return ref _array[curLen];
    }

    public Span<T> Allocate(int length)
    {
        if (length > 0)
        {
            int curLen = _count;
            int newLen = curLen + length;
            // Check for growth
            if (newLen > _array.Length)
            {
                GrowBy(length);
            }

            // Add to our current position
            _count = newLen;

            // Return the allocated (at end of Written)
            return _array.AsSpan(curLen, length);
        }

        // Asked for nothing
        return Span<T>.Empty;
    }

    public ref T AllocateAt(int index)
    {
        int curLen = _count;
        Throw.Index(curLen, index, true);
        int newLen = curLen + 1;

        // Check for growth
        if (newLen > _array.Length)
        {
            GrowBy(1);
        }

        // We're adding this much
        _count = newLen;

        // At end?
        if (index == curLen)
        {
            // The same as Allocate()
            return ref _array[curLen];
        }
        // Insert

        // Shift existing to the right
        var keep = _array.AsSpan(new Range(start: index, end: curLen));
        var keepLength = keep.Length;
        // We know we have enough space to grow to
        var rightBuffer = _array.AsSpan(index + 1, keepLength);
        keep.CopyTo(rightBuffer);
        // return where we allocated
        return ref _array[index];
    }

    public ref T AllocateAt(Index index)
    {
        int offset = Throw.Index(_count, index);
        return ref AllocateAt(offset);
    }
    
    public Span<T> AllocateRange(int index, int length)
    {
        int curLen = _count;
        Throw.Index(curLen, index, true);
        if (length > 0)
        {
            int newLen = curLen + length;

            // Check for growth
            if (newLen > _array.Length)
            {
                GrowBy(length);
            }

            // We're adding this much
            _count = newLen;

            // At end?
            if (index == curLen)
            {
                // The same as Allocate(length)
                return _array.AsSpan(curLen, length);
            }
            // Insert

            // Shift existing to the right
            var keep = _array.AsSpan(new Range(start: index, end: curLen));
            var keepLen = keep.Length;
            // We know we have enough space to grow to
            var destBuffer = _array.AsSpan(index + length, keepLen);
            keep.CopyTo(destBuffer);
            // return where we allocated
            return _array.AsSpan(index, length);
        }

        // Asked for nothing
        return Span<T>.Empty;
    }

    public Span<T> AllocateRange(Range range)
    {
        (int offset, int length) = Throw.Range(_count, range);
        return AllocateRange(offset, length);
    }
#endregion
    
    public bool TryAdd(SpanFunc<T, int> availableBufferUse)
    {
        var available = _array.AsSpan(_count);
        int used = availableBufferUse(available);
        if (used > 0)
        {
            _count += used;
            return true;
        }
        return false;
    }
    
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

    public bool Contains(scoped ReadOnlySpan<T> items, IEqualityComparer<T>? itemComparer = default)
    {
        int itemCount = items.Length;
        if (itemCount == 0) return true;
        if (itemCount == 1) return Contains(items[0], itemComparer);

        var array = _array;
        var arrayCount = _count;
        int exclusion = arrayCount - itemCount;
        var comparer = itemComparer ?? EqualityComparer<T>.Default;

        T firstItem = items[0];

        for (var arrayIndex = 0; arrayIndex < exclusion; arrayIndex++)
        {
            if (comparer.Equals(firstItem, array[arrayIndex]))
            {
                // Due to exclusion, we know that we can index safely
                if (array.AsSpan(arrayIndex + 1).SequenceEqual(items[1..], comparer))
                    return true;
            }
        }
        return false;
    }


    /// <inheritdoc cref="IList{T}"/>
    int IList<T>.IndexOf(T item) => FindIndex(item);

    private (int Start, int End, int Mod) Resolve(Index? scanIndex, bool fromEnd)
    {
        if (!scanIndex.TryGetValue(out var index))
        {
            if (!fromEnd)
            {
                return (0, _count - 1, +1);
            }
            else
            {
                return (_count - 1, 0, -1);
            }
        }
        else
        {
            int scan = index.GetOffset(_count);
            if (!fromEnd)
            {
                return (scan, _count - 1, +1);
            }
            else
            {
                return (scan, 0, -1);
            }
        }
    }
    
    public int FindIndex(T item, IEqualityComparer<T>? itemComparer = default, Index? startIndex = default, bool fromEnd = false)
    {
        var comparer = itemComparer ?? EqualityComparer<T>.Default;
        var array = _array;
        var (start, end, mod) = Resolve(startIndex, fromEnd);
        for (var i = start; i <= end; i += mod)
        {
            if (comparer.Equals(item, array[i]))
            {
                return i;
            }
        }
        return -1;
    }
    
    public int FindIndex(scoped ReadOnlySpan<T> items, IEqualityComparer<T>? itemComparer = default, Index? startIndex = default, bool fromEnd = false)
    {
        int itemCount = items.Length;
        if (itemCount == 0) return -1;
        if (itemCount == 1) return FindIndex(items[0], itemComparer, startIndex, fromEnd);


        var array = _array;
        var arrayCount = _count;
        int exclusion = arrayCount - itemCount;
        var comparer = itemComparer ?? EqualityComparer<T>.Default;

        T firstItem = items[0];

        for (var arrayIndex = 0; arrayIndex < exclusion; arrayIndex++)
        {
            if (comparer.Equals(firstItem, array[arrayIndex]))
            {
                // Due to exclusion, we know that we can index safely
                if (array.AsSpan(arrayIndex + 1).SequenceEqual(items[1..], comparer))
                    return true;
            }
        }
        return false;
    }

    /// <inheritdoc cref="IList{T}"/>
    void IList<T>.RemoveAt(int index)
    {
        Throw.Index(_count, index);
        TryRemoveAt(index);
    }

    public Result TryRemoveAt(int index)
    {
        int count = _count;
        if (index < 0 || index >= count)
            return new ArgumentOutOfRangeException(nameof(index), index, $"{nameof(index)} must be in [0..{count})");

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

    public Result TryRemoveAt(Index index) => TryRemoveAt(index.GetOffset(_count));

    public Result TryRemoveRange(int offset, int length)
    {
        var checkResult = Check.Range(_count, offset, length);
        if (!checkResult) return checkResult;

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

    public Result TryRemoveRange(Range range)
    {
        (int offset, int length) = range.GetOffsetAndLength(_count);
        return TryRemoveRange(offset, length);
    }

    /// <inheritdoc cref="ICollection{T}"/>
    bool ICollection<T>.Remove(T item) => TryRemoveFirst(item);

    public Result TryRemoveFirst(T item, IEqualityComparer<T>? itemComparer = default)
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

    public Result TryRemoveLast(T item, IEqualityComparer<T>? itemComparer = default)
    {
        var end = _count;
        var written = _array.AsSpan(0, end);
        var comparer = itemComparer ?? EqualityComparer<T>.Default;
        for (var i = end - 1; i >= 0; i++)
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
        Throw.CanCopyTo(_count, array, arrayIndex);
        this.CopyTo(array.AsSpan(arrayIndex));
    }

    public void CopyTo(Span<T> buffer) => this.AsSpan().CopyTo(buffer);

    public Result TryCopyTo(Span<T> buffer) => this.AsSpan().TryCopyTo(buffer);
    
    /// <summary>
    /// Removes all items from this <see cref="Buffer{T}"/>
    /// </summary>
    public void Clear()
    {
        _count = 0;
    }


    public IList<T> AsList() => this;

    public ICollection<T> AsCollection() => this;

    public List<T> ToList() => new(this);

    public T[] ToArray() => AsSpan().ToArray();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _array.AsSpan(0, _count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetUnwrittenSpan() => _array.AsSpan(_count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] GetArray() => _array;

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

#pragma warning disable CS0809
    [Obsolete($"Do not compare {nameof(Buffer<T>)} to anything. If you want to compare its contents, use AsSpan()", true)]
    public override bool Equals(object? obj) => false;

    [Obsolete($"Do not store {nameof(Buffer<T>)} in a set, it is intended to be Disposed", true)]
    public override int GetHashCode() => 0;
#pragma warning restore CS0809

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