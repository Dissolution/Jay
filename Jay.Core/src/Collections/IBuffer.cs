namespace Jay.Collections;


public interface IBuffer<T> : 
    IReadOnlyCollection<T>, IEnumerable<T>, 
    IDisposable,
    IFormattable
{
    /// <summary>
    /// Gets a reference to the item at <paramref name="index"/>
    /// </summary>
    /// <param name="index">
    /// The index of the item to reference
    /// </param>
    ref T this[int index] { get; }

    /// <summary>
    /// Gets a reference to the item at <paramref name="index"/>
    /// </summary>
    /// <param name="index">
    /// The <see cref="Index"/> of the item to reference
    /// </param>
    ref T this[Index index] { get; }

    /// <summary>
    /// Gets the <see cref="Span{T}"/> of items at <paramref name="range"/>
    /// </summary>
    /// <param name="range">
    /// The <see cref="Range"/> of items to reference
    /// </param>
    Span<T> this[Range range] { get; }
    
    /// <summary>
    /// Gets the current capacity for this buffer - which increases as needed<br/>
    /// Sets the minimum capacity for this buffer - but actual may be higher 
    /// </summary>
    int Capacity { get; set; }
    
    /// <summary>
    /// Adds a new <paramref name="item"/> to the end of this buffer
    /// </summary>
    /// <param name="item">The item to append</param>
    void Add(T item);
    
    /// <summary>
    /// Adds several <paramref name="items"/> to the end of this buffer
    /// </summary>
    /// <param name="items">The items to append</param>
    void AddMany(params T[] items);
    
    /// <summary>
    /// Adds several <paramref name="items"/> to the end of this buffer
    /// </summary>
    /// <param name="items">The items to append</param>
    void AddMany(scoped ReadOnlySpan<T> items);
    
    /// <summary>
    /// Adds several <paramref name="items"/> to the end of this buffer
    /// </summary>
    /// <param name="items">The items to append</param>
    void AddMany(IEnumerable<T> items);
    
    /// <summary>
    /// Tries to add item(s) to this buffer with a function that
    /// operates on the current unwritten <see cref="Span{T}"/>
    /// and returns the amount of that span that was filled
    /// </summary>
    /// <param name="availableBufferUse"></param>
    /// <returns>
    /// <c>true</c> if items were added<br/>
    /// <c>false</c> if no items were added
    /// </returns>
    bool TryAdd(SpanFunc<T, int> availableBufferUse);
    
    /// <summary>
    /// Inserts an <paramref name="item"/> at the given <paramref name="index"/>
    /// </summary>
    /// <param name="index">The index in this buffer to insert the <paramref name="item"/></param>
    /// <param name="item">The item to insert</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is invalid
    /// </exception>
    void Insert(int index, T item);
    
    /// <summary>
    /// Inserts several <paramref name="items"/> at the given <paramref name="index"/>
    /// </summary>
    /// <param name="index">The index in this buffer to insert the <paramref name="items"/></param>
    /// <param name="items">The items to insert</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is invalid
    /// </exception>
    void InsertMany(int index, scoped ReadOnlySpan<T> items);
    
    /// <summary>
    /// Does this buffer contain an instance of the given <paramref name="item"/>?
    /// </summary>
    /// <param name="item">
    /// The item instance to check for
    /// </param>
    /// <param name="itemComparer">
    /// An optional <see cref="IEqualityComparer{T}"/> for items
    /// </param>
    /// <returns></returns>
    bool Contains(T item, IEqualityComparer<T>? itemComparer = default);
    
    /// <summary>
    /// Does this buffer contain a sequence of the given <paramref name="items"/>?
    /// </summary>
    /// <param name="items">
    /// The items to check for
    /// </param>
    /// <param name="itemComparer">
    /// An optional <see cref="IEqualityComparer{T}"/> for items
    /// </param>
    /// <returns></returns>
    bool Contains(scoped ReadOnlySpan<T> items, IEqualityComparer<T>? itemComparer = default);

    int FindIndex(T item, IEqualityComparer<T>? itemComparer = default, Index? startIndex = default, bool fromEnd = false);
    int FindIndex(scoped ReadOnlySpan<T> items, IEqualityComparer<T>? itemComparer = default, Index? startIndex = default, bool fromEnd = false);

    
    
    Result TryRemoveAt(int index);
    Result TryRemoveAt(Index index);
    Result TryRemoveRange(int start, int length);
    Result TryRemoveRange(Range range);
    Result TryRemoveFirst(T item, IEqualityComparer<T>? itemComparer = default);
    Result TryRemoveLast(T item, IEqualityComparer<T>? itemComparer = default);
    
    
    /// <summary>
    /// Removes all items from this buffer
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Copy the items in this buffer into the given <paramref name="span"/>
    /// </summary>
    /// <param name="span">
    /// The <see cref="Span{T}"/> to copy this buffers items into
    /// </param>
    void CopyTo(Span<T> span);
    
    /// <summary>
    /// Try to copy the items in this buffer into the given <paramref name="span"/>
    /// </summary>
    /// <param name="span">
    /// The <see cref="Span{T}"/> to copy this buffers items into
    /// </param>
    /// <returns>
    /// <c>true</c> if all items were copied<br/>
    /// <c>false</c> if the items could not be copied
    /// </returns>
    Result TryCopyTo(Span<T> span);
    


    /// <summary>
    /// Gets an <see cref="IList{T}"/> representation of this buffer
    /// </summary>
    IList<T> AsList();
    /// <summary>
    /// Gets an <see cref="ICollection{T}"/> representation of this buffer
    /// </summary>
    ICollection<T> AsCollection();
    /// <summary>
    /// Gets an <see cref="Span{T}"/> representation of this buffer
    /// </summary>
    Span<T> AsSpan();
    
    /// <summary>
    /// Convert this buffer to a <see cref="List{T}"/>
    /// </summary>
    /// <returns>
    /// A new <see cref="List{T}"/> containing the items in this buffer in the same order
    /// </returns>
    List<T> ToList();
    
    /// <summary>
    /// Convert this buffer to a <see cref="Array">T[]</see>
    /// </summary>
    /// <returns>
    /// A new <c>T[]</c> containing the items in this buffer in the same order
    /// </returns>
    T[] ToArray();
}

public interface IUnsafeBuffer<T> : IBuffer<T>
{
    new int Count { get; set; }
    
    ref T Allocate();
    Span<T> AllocateMany(int length);
    ref T AllocateAt(int index);
    ref T AllocateAt(Index index);
    Span<T> AllocateRange(int index, int length);
    Span<T> AllocateRange(Range range);

    Span<T> GetUnwrittenSpan();
    T[] GetArray();
}