using System.Diagnostics;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace Jay.Collections.Pooling;

/// <summary>
/// A thread-safe pool of <typeparamref name="T" /> instances.
/// </summary>
/// <typeparam name="T">An instance class</typeparam>
public class ObjectPool<T> : IObjectPool<T>, IDisposable
    where T : class
{
    /// <summary>
    /// Optional instance clean action.
    /// </summary>
    private readonly PoolInstanceClean<T>? _cleanItem;

    /// <summary>
    /// Optional instance disposal action.
    /// </summary>
    private readonly PoolInstanceDispose<T>? _disposeItem;

    /// <summary>
    /// Instance creation function.
    /// </summary>
    private readonly PoolInstanceFactory<T> _itemFactory;

    /// <summary>
    /// Storage for the pool items.
    /// </summary>
    protected readonly Item[] _items;

    /// <summary>
    /// Whether or not this pool has been disposed.
    /// </summary>
    /// <remarks>
    /// We want disposal to be thread-safe (as the rest of the methods are), so we use <see cref="Interlocked" /> to manage it.
    /// Since `Interlocked.CompareExchange` does not work on <see cref="bool" /> values, we use an <see cref="int" /> where `> 0` means disposed.
    /// </remarks>
    protected int _disposed;

    /// <summary>
    /// The first item is stored in a dedicated field because we expect to be able to satisfy most requests from it.
    /// </summary>
    protected T? _firstItem;

    internal bool IsDisposed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interlocked.CompareExchange(ref _disposed, 0, 0) > 0;
    }

    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}" /> for classes.
    /// </summary>
    /// <typeparam name="T">An instance class</typeparam>
    /// <param name="factory">A function to create a new <typeparamref name="T" /> instance.</param>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T" /> when it is returned.</param>
    /// <param name="dispose">An optional action to perform on a <typeparamref name="T" /> if it is disposed.</param>
    public ObjectPool(
        PoolInstanceFactory<T> factory,
        PoolInstanceClean<T>? clean = null,
        PoolInstanceDispose<T>? dispose = null)
        : this(Pool.DefaultCapacity, factory, clean, dispose)
    {
    }


    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}" /> for classes with a specified <paramref name="capacity" />.
    /// </summary>
    /// <typeparam name="T">An instance class</typeparam>
    /// <param name="capacity">The specific number of items that will ever be retained in the pool.</param>
    /// <param name="factory">A function to create a new <typeparamref name="T" /> instance.</param>
    /// <param name="clean">An optional action to perform on a <typeparamref name="T" /> when it is returned.</param>
    /// <param name="dispose">An optional action to perform on a <typeparamref name="T" /> if it is disposed.</param>
    public ObjectPool(
        int capacity,
        PoolInstanceFactory<T> factory,
        PoolInstanceClean<T>? clean = null,
        PoolInstanceDispose<T>? dispose = null)
    {
        if (capacity < 1 || capacity > Pool.ArrayMaxCapacity)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                capacity,
                $"Pool Capacity must be between 1 and {Pool.ArrayMaxCapacity}");
        }

        _itemFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        _cleanItem = clean;
        _disposeItem = dispose;

        _firstItem = default;
        _items = new Item[capacity - 1];
    }

    /// <summary>
    /// Gets the maximum number of items retained by this pool.
    /// </summary>
    public int Capacity => _items.Length + 1;

    /// <inheritdoc />
    public int Count
    {
        get
        {
            var count = 0;
            if (_firstItem is not null) count++;
            for (var i = 0; i < _items.Length; i++)
            {
                if (_items[i].Value is not null) count++;
            }
            return count;
        }
    }

    /// <summary>
    /// Rent a <typeparamref name="T" /> instance that should be <see cref="Return" />ed.
    /// </summary>
    /// <remarks>
    /// Search strategy is a simple linear probing which is chosen for it cache-friendliness.
    /// Note that Rent will try to store recycled objects close to the start
    /// thus statistically reducing how far we will typically search.
    /// </remarks>
    public T Rent()
    {
        // Always check if we've been disposed
        CheckDisposed();

        /* PERF: Examine the first element.
         * If that fails, AllocateSlow will look at the remaining elements.
         * Note that the initial read is optimistically not synchronized. That is intentional. 
         * We will interlock only when we have a candidate.
         * In a worst case we may miss some recently returned objects. Not a big deal.
         */
        T? instance = _firstItem;
        if (instance is null || instance != Interlocked.CompareExchange(ref _firstItem, null, instance))
        {
            instance = RentSlow();
        }
        return instance;
    }

    /// <summary>
    /// Returns a <typeparamref name="T" /> instance to the pool to be cleaned and re-used.
    /// </summary>
    /// <remarks>
    /// Search strategy is a simple linear probing which is chosen for it cache-friendliness.
    /// Note that Free will try to store recycled objects close to the start thus statistically
    /// reducing how far we will typically search in Allocate.
    /// </remarks>
    public void Return(T? instance)
    {
        if (instance is null) return;

        // Always clean the item
        _cleanItem?.Invoke(instance);

        // If we're disposed, just dispose the instance and exit
        if (IsDisposed)
        {
            _disposeItem?.Invoke(instance);
            return;
        }

        // Examine first item, if that fails, use the pool.
        // Initial read is not synchronized; we only interlock on a candidate.
        if (_firstItem == null)
        {
            if (Interlocked.CompareExchange(ref _firstItem, instance, null) == null)
            {
                // We stored it
                return;
            }
        }

        // We have to try to return it to the pool (and this will also clean it up if it cannot be)
        ReturnSlow(instance);
    }

    /// <summary>
    /// Rents a <typeparamref name="T" /> <paramref name="instance" />
    /// that will be returned when the returned <see cref="IDisposable" /> is disposed.
    /// </summary>
    /// <param name="instance">
    /// A <typeparamref name="T" /> instance,
    /// it will be returned to its origin <see cref="ObjectPool{T}" /> when disposed.
    /// </param>
    /// <returns>An <see cref="IDisposable" /> that will return the <paramref name="instance" />.</returns>
    /// <remarks>
    /// <paramref name="instance" /> must not be used after this is disposed.
    /// </remarks>
    public IDisposable Borrow(out T instance)
    {
        instance = Rent();
        return new PoolInstance<T>(this, instance);
    }

    /// <inheritdoc />
    public IPoolInstance<T> Borrow()
    {
        T instance = Rent();
        return new PoolInstance<T>(this, instance);
    }

    /// <inheritdoc />
    public void Use(Action<T> instanceAction)
    {
        Validate.NotNull(instanceAction);
        T instance = Rent();
        instanceAction.Invoke(instance);
        Return(instance);
    }

    /// <inheritdoc />
    public TResult Use<TResult>(Func<T, TResult> instanceFunc)
    {
        Validate.NotNull(instanceFunc);
        T instance = Rent();
        TResult result = instanceFunc.Invoke(instance);
        Return(instance);
        return result;
    }

    /// <summary>
    /// Frees all stored <typeparamref name="T" /> instances.
    /// </summary>
    public void Dispose()
    {
        // Have I already been disposed?
        if (Interlocked.Increment(ref _disposed) > 1) return;
        // We only do anything if we have a disposer
        if (_disposeItem != null)
        {
            var disposeItem = _disposeItem!;

            T? item = Reference.Exchange<T?>(ref _firstItem, null);
            if (item != null)
            {
                disposeItem(item);
            }
            var items = _items;
            for (var i = 0; i < items.Length; i++)
            {
                item = Reference.Exchange<T?>(ref items[i].Value, null);
                if (item != null)
                {
                    disposeItem(item);
                }
            }
        }
        Debug.Assert(_items.All(item => item.Value == null));
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Check if this <see cref="ObjectPool{T}" /> has been disposed, and if it has, throw an <see cref="ObjectDisposedException" />.
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void CheckDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException("This ObjectPool has been disposed");
        }
    }

    /// <summary>
    /// When we cannot find an available item quickly with <see cref="Rent()" />, we take this slower path
    /// </summary>
    private T RentSlow()
    {
        var items = _items;
        T? instance;
        for (var i = 0; i < items.Length; i++)
        {
            /* Note that the initial read is optimistically not synchronized.
             * That is intentional. 
             * We will interlock only when we have a candidate.
             * In a worst case we may miss some recently returned objects. Not a big deal. */
            instance = items[i].Value;
            if (instance != null)
            {
                if (instance == Interlocked.CompareExchange(ref items[i].Value, null, instance))
                {
                    return instance;
                }
            }
        }

        //Just create a new value.
        return _itemFactory();
    }

    /// <summary>
    /// When we cannot return quickly with <see cref="Return" />, we take this slower path.
    /// </summary>
    /// <param name="instance"></param>
    private void ReturnSlow(T instance)
    {
        var items = _items;
        for (var i = 0; i < items.Length; i++)
        {
            // Like AllocateSlow, the initial read is not synchronized and will only interlock on a candidate.
            if (items[i].Value is null)
            {
                if (Interlocked.CompareExchange(ref items[i].Value, instance, null) is null)
                {
                    // We stored it
                    break;
                }
            }
        }

        // We couldn't store this value, dispose it and let it get collected
        _disposeItem?.Invoke(instance);
    }

    /// <summary>
    /// A <c>struct</c> holder for a <typeparamref name="T" /> value
    /// </summary>
    [DebuggerDisplay("{" + nameof(Value) + ",nq}")]
    protected struct Item
    {
        internal T? Value;
    }
}