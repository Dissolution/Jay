using System.Diagnostics;
using System.Runtime.CompilerServices;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace Jay.Collections.Pools;

/// <remarks>
///     This is a generic implementation of the object pooling pattern.
///     The main purpose is to re-use a limited number of objects rather than continuously `new()`ing them up. 
/// 
///     - It is not the goal to keep all returned objects.
///       - Pool is not meant for storage.
///       - If there is no space in the pool, extra returned objects will be dropped.
///     - It is implied that if object was obtained from a pool, the caller will return it back in a relatively short time.
///       - Keeping checked out objects for long durations is ok, but reduces usefulness of pooling.
///     - Not returning objects to the pool in not detrimental to the pool's work, but is a bad practice. 
///       - If there is no intent for reusing the object, do not use pool
/// </remarks>
public sealed class ObjectPool<T> : IDisposable
    where T : class
{
    /// <summary>
    ///     An <see cref="IDisposable"/> wrapper around returning an instance to an object pool
    /// </summary>
    private sealed class PoolReturner : IDisposable
    {
        private readonly ObjectPool<T> _pool;
        private T? _instance;

        public PoolReturner(ObjectPool<T> pool, T instance)
        {
            _pool = pool;
            _instance = instance;
        }

        public void Dispose()
        {
            T? instance = Interlocked.Exchange(ref _instance, null);
            _pool.Return(instance);
        }
    }
        
    [DebuggerDisplay("{" + nameof(Value) + ",nq}")]
    private struct Item
    {
        internal T? Value;
    }

    /// <summary>
    /// Whether or not this pool has been disposed.
    /// </summary>
    /// <remarks>
    /// We want disposal to be thread-safe (as the rest of the methods are), so we use <see cref="Interlocked"/> to manage it.
    /// Since `Interlocked.CompareExchange` does not work on <see cref="bool"/> values, we use an <see cref="int"/> where `> 0` means disposed.
    /// </remarks>
    private int _disposed;

    /// <summary>
    /// The first item is stored in a dedicated field because we expect to be able to satisfy most requests from it.
    /// </summary>
    private T? _firstItem;

    /// <summary>
    /// Storage for the pool items.
    /// </summary>
    private readonly Item[] _items;

    /// <summary>
    /// Value creation factory.
    /// </summary>
    private readonly Func<T> _factory;

    private readonly Action<T>? _clean;

    private readonly Action<T>? _dispose;

    public ObjectPool(Func<T> factory,
                      Action<T>? clean = null,
                      Action<T>? dispose = null)
        : this(Pool.DefaultCapacity, factory, clean, dispose)
    {
    }

    public ObjectPool(int capacity,
                      Func<T> factory,
                      Action<T>? clean = null,
                      Action<T>? dispose = null)
    {
        if (capacity < 1 || capacity > Pool.MaxCapacity)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity,
                $"Pool Capacity must be 1 <= {capacity} <= {Pool.MaxCapacity}");
        }

        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _clean = clean;
        _dispose = dispose;
        // We have a _firstItem
        _items = new Item[capacity - 1];
    }
    ~ObjectPool()
    {
        this.Dispose();
    }

    internal bool IsDisposed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Interlocked.CompareExchange(ref _disposed, 0, 0) > 0;
    }

    /// <summary>
    /// When we cannot find an available item quickly with Rent()
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
             * In a worst case we may miss some recently returned objects. Not a big deal.
             */
            instance = items[i].Value;
            if (instance != null)
            {
                if (instance == Interlocked.CompareExchange<T?>(ref items[i].Value, null, instance))
                {
                    return instance;
                }
            }
        }

        //Just create a new value.
        return _factory();
    }

    private void ReturnSlow(T instance)
    {
        var items = _items;
        for (var i = 0; i < items.Length; i++)
        {
            // Like AllocateSlow, the initial read is not synchronized and will only interlock on a candidate.
            if (items[i].Value is null)
            {
                if (Interlocked.CompareExchange<T?>(ref items[i].Value, instance, null) is null)
                {
                    // We stored it
                    break;
                }
            }
        }

        // We couldn't store this value
        _dispose?.Invoke(instance);
    }

    /// <summary>
    /// Rents a <typeparamref name="T"/> instance that should be <see cref="Return"/>ed once it is done being used.
    /// </summary>
    /// <remarks>
    /// Search strategy is a simple linear probing which is chosen for it cache-friendliness.
    /// Note that Free will try to store recycled objects close to the start thus statistically reducing how far we will typically search.
    /// </remarks>
    public T Rent()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(this.GetType().Name);

        /* PERF: Examine the first element.
         * If that fails, AllocateSlow will look at the remaining elements.
         * Note that the initial read is optimistically not synchronized. That is intentional. 
         * We will interlock only when we have a candidate.
         * In a worst case we may miss some recently returned objects. Not a big deal.
         */
        T? instance = _firstItem;
        if (instance is null || instance != Interlocked.CompareExchange<T?>(ref _firstItem, null, instance))
        {
            instance = RentSlow();
        }
        return instance;
    }

    /// <summary>
    /// Returns a <typeparamref name="T"/> instance to the pool to be cleaned and re-used.
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
        _clean?.Invoke(instance);
            
        // Disposed check
        if (IsDisposed)
            return;

        // Examine first item, if that fails use the pool.
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
    /// Rents a <typeparamref name="T"/> instance that will be returned the result of this operation is disposed.
    /// </summary>
    /// <param name="instance">A fresh instance to be used, it will automatically be returned upon disposal.</param>
    /// <returns>An <see cref="IDisposable"/> that will return the <paramref name="instance"/>. </returns>
    /// <remarks><paramref name="instance"/> must not be used after this is disposed.</remarks>
    public IDisposable Borrow(out T instance)
    {
        instance = Rent();
        return new PoolReturner(this, instance);
    }
        
    /// <summary>
    /// Frees all stored <typeparamref name="T"/> instances.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Increment(ref _disposed) > 1) return;
        if (_dispose != null)
        {
            var dispose = _dispose!;
            T? item = Reference.Exchange(ref _firstItem, null);
            if (item != null)
                dispose(item);
            var items = _items;
            for (var i = 0; i < items.Length; i++)
            {
                item = Reference.Exchange(ref items[i].Value, null);
                if (item != null)
                    dispose(item);
            }
        }
        Debug.Assert(_items.All(item => item.Value is null));
        GC.SuppressFinalize(this);
    }
}