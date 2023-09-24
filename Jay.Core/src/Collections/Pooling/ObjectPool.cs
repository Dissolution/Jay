using System.Diagnostics;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace Jay.Collections.Pooling;

/// <inheritdoc cref="IObjectPool{T}"/>
public class ObjectPool<T> : IObjectPool<T>, IDisposable
    where T : class
{
    /// <summary>
    /// Optional instance cleaning action
    /// Performed on each instance returned to the pool to get them ready to be re-used
    /// </summary>
    private readonly PoolInstanceClean<T>? _cleanItem;

    /// <summary>
    /// Optional instance disposal action
    /// Performed on any instance dropped by this pool (due to <see cref="MaxCapacity"/>
    /// and when this pool is disposed
    /// </summary>
    private readonly PoolInstanceDispose<T>? _disposeItem;

    /// <summary>
    /// Instance creation function.
    /// Whenever an instance is requested but unavailable, this function will create a new instance
    /// </summary>
    private readonly PoolInstanceFactory<T> _itemFactory;

    
    /// <summary>
    /// The first instance is stored in a dedicated field because we expect to be able to
    /// satisfy most requests from it
    /// </summary>
    private T? _firstItem;
    
    /// <summary>
    /// Storage for the extra pool instances
    /// </summary>
    private Item[]? _items;
    
    public int MaxCapacity
    {
        get
        {
            if (_items is null) return 0; // We've been disposed
            return _items.Length + 1;
        }
    }

    public int Count
    {
        get
        {
            var count = 0;
            if (_firstItem is not null) count++;
            if (_items is null) return count;
            for (var i = 0; i < _items.Length; i++)
            {
                if (_items[i].Value is not null) count++;
            }
            return count;
        }
    }
    
    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}" /> for classes
    /// </summary>
    /// <typeparam name="T">An instance class</typeparam>
    /// <param name="factory">
    /// A function to create a new <typeparamref name="T" /> instance
    /// </param>
    /// <param name="clean">
    /// An optional action to perform on a <typeparamref name="T" /> when it is returned
    /// </param>
    /// <param name="dispose">
    /// An optional action to perform on a <typeparamref name="T" /> if it is disposed
    /// </param>
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
        if (capacity is < 1 or > Pool.MAX_CAPACITY)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                capacity,
                $"Pool Capacity must be between 1 and {Pool.MAX_CAPACITY}");
        }

        _itemFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        _cleanItem = clean;
        _disposeItem = dispose;

        _firstItem = default;
        _items = new Item[capacity - 1];
    }
    
   

    /// <summary>
    /// Rent a <typeparamref name="T" /> instance that should be <see cref="Return" />ed.
    /// </summary>
    public T Rent()
    {
        // Always check if we've been disposed
        if (_items is null)
            throw new ObjectDisposedException("This Object Pool has been disposed");

        /* Check the first element, if we can use it we can return right away,
         * otherwise we have have to take the slow path.
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
    public void Return(T? instance)
    {
        if (instance is null) return;

        // Always clean the item
        _cleanItem?.Invoke(instance);

        // If we're disposed, just dispose the instance and exit
        if (_items is null)
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
    public IDisposable GetInstance(out T instance)
    {
        instance = Rent();
        return new PoolInstance<T>(this, instance);
    }

    public IPoolInstance<T> GetInstance()
    {
        T instance = Rent();
        return new PoolInstance<T>(this, instance);
    }

    public void Borrow(Action<T> instanceAction)
    {
        Validate.IsNotNull(instanceAction);
        T instance = Rent();
        instanceAction.Invoke(instance);
        Return(instance);
    }

    public TResult Borrow<TResult>(Func<T, TResult> instanceFunc)
    {
        Validate.IsNotNull(instanceFunc);
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
        T? item = Interlocked.Exchange<T?>(ref _firstItem, null);
        if (item is not null)
        {
            _disposeItem?.Invoke(item);
        }
        var items = Interlocked.Exchange<Item[]?>(ref _items, null);
        if (items is not null)
        {
            for (var i = 0; i < items.Length; i++)
            {
                item = Interlocked.Exchange<T?>(ref items[i].Value, null);
                if (item != null)
                {
                    _disposeItem?.Invoke(item);
                }
            }
        }
    }

    /// <summary>
    /// When we cannot find an available item quickly with <see cref="Rent()" />, we take this slower path
    /// </summary>
    private T RentSlow()
    {
        var items = _items;
        T? instance;
        for (var i = 0; i < items!.Length; i++)
        {
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
        for (var i = 0; i < items!.Length; i++)
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
    /// A <c>struct</c> holder for a <c>T</c> value
    /// </summary>
    [DebuggerDisplay("{" + nameof(Value) + ",nq}")]
    private struct Item
    {
        public T? Value;
    }
}