using System.Diagnostics;
using Jay.Reflection;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace Jay.Collections;

/// <summary>
/// A pool of <typeparamref name="T"/> instances
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <c>class</c> instances stored in this <see cref="ObjectPool{T}"/>
/// </typeparam>
/// <remarks>
/// - The main purpose of an <see cref="ObjectPool{T}"/> is to help re-use a limited number of
/// <typeparamref name="T"/> instances rather than continuously <c>new</c>-ing them up.<br/>
/// - It is not the goal to keep all returned instances.<br/>
///   - The pool is not meant for storage (short nor long).<br/>
///   - If there is no space in the pool, extra returned instances will be disposed.<br/>
/// - It is implied that if an instance is obtained from a pool, the caller will return it back in a relatively short time.<br/>
///   - Keeping checked out instances for long durations is _ok_, but it reduces the usefulness of pooling.<br/>
///   - Not returning instances to the pool in not detrimental to its work, but is a bad practice.<br/>
///   - If there is no intent to return or re-use the instance, do not use a pool.<br/>
/// - When this pool is Disposed, all instances will also be disposed.<br/>
///   - Any further returned instances will be cleaned, disposed, and discarded.<br/>
/// </remarks>
public sealed class ObjectPool<T> : IDisposable
    where T : class
{
    /// <summary>
    /// The maximum capacity for any pool
    /// </summary>
    private const int MAX_CAPACITY = 0X7FFFFFC7; // == Array.MaxLength
    
    /// <summary>
    /// Instance creation function<br/>
    /// Whenever an instance is requested but unavailable, this function will create a new instance
    /// </summary>
    private readonly Func<T> _instanceFactory;

    /// <summary>
    /// Optional instance cleaning action<br/>
    /// Performed on each instance returned to the pool to get them ready to be re-used
    /// </summary>
    private readonly Action<T>? _cleanInstance;

    /// <summary>
    /// Optional instance disposal action<br/>
    /// Performed on any instance dropped by this pool
    /// </summary>
    private readonly Action<T>? _disposeInstance;


    /// <summary>
    /// The first instance is stored in a dedicated field
    /// as we expect to be able to satisfy most requests from it
    /// </summary>
    private T? _firstInstance;

    /// <summary>
    /// Storage for the extra pool instances
    /// </summary>
    private RefInstance[]? _instances;

    [DebuggerDisplay("{" + nameof(Instance) + ",nq}")]
    private struct RefInstance
    {
        public T? Instance;
    }

    /// <summary>
    /// Gets the maximum number of instances this <see cref="ObjectPool{T}"/> can store
    /// </summary>
    public int MaxCapacity
    {
        get
        {
            if (_instances is null) return 0; // We've been disposed

            return _instances.Length + 1;
        }
    }

    /// <summary>
    /// Gets the current number of instances in this <see cref="ObjectPool{T}"/>
    /// </summary>
    /// <remarks>
    /// During active <see cref="Rent"/> and <see cref="Return"/> operations, this number may not be accurate
    /// </remarks>
    public int Count
    {
        get
        {
            var count = 0;
            if (_firstInstance is not null) count++;
            if (_instances is null) return count;

            for (var i = 0; i < _instances.Length; i++)
            {
                if (_instances[i].Instance is not null) count++;
            }
            return count;
        }
    }

    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of <c>class</c> instances stored
    /// </typeparam>
    /// <param name="factory">
    /// A <see cref="Func{T}"/> used to create needed instances
    /// </param>
    /// <param name="clean">
    /// An optional <see cref="Action{T}"/> to perform on each returned instance
    /// </param>
    /// <param name="dispose">
    /// An optional <see cref="Action{T}"/> to perform on a disposed instance
    /// </param>
    public ObjectPool(
        Func<T> factory,
        Action<T>? clean = null,
        Action<T>? dispose = null)
        : this(1 + (2 * Environment.ProcessorCount), factory, clean, dispose)
    {
    }


    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/> with the given <paramref name="totalCapacity"/>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of <c>class</c> instances stored
    /// </typeparam>
    /// <param name="totalCapacity">
    /// The total number of instances that can ever be stored
    /// </param>
    /// <param name="factory">
    /// A <see cref="Func{T}"/> used to create needed instances
    /// </param>
    /// <param name="clean">
    /// An optional <see cref="Action{T}"/> to perform on each returned instance
    /// </param>
    /// <param name="dispose">
    /// An optional <see cref="Action{T}"/> to perform on a disposed instance
    /// </param>
    public ObjectPool(
        int totalCapacity,
        Func<T> factory,
        Action<T>? clean = null,
        Action<T>? dispose = null)
    {
        if (totalCapacity is < 1 or > MAX_CAPACITY)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalCapacity),
                totalCapacity,
                $"Pool Capacity must be between 1 and {MAX_CAPACITY}");
        }

        _instanceFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        _cleanInstance = clean;
        _disposeInstance = dispose;

        _firstInstance = default;
        _instances = new RefInstance[totalCapacity - 1];
    }


    /// <summary>
    /// Rent a <typeparamref name="T"/> instance from this <see cref="ObjectPool{T}"/>
    /// </summary>
    /// <remarks>
    /// Per <see cref="ObjectPool{T}"/>, please <see cref="Return"/> the instance when you are done with it!
    /// </remarks>
    public T Rent()
    {
        // Always check if we've been disposed
        if (_instances is null)
            throw new ObjectDisposedException(GetType().NameOf());

        // Check if we can satisfy with the first instance
        T? instance = _firstInstance;
        // If we could, Interlocked check to be sure that we are the only thread that can take it
        if (instance is null || instance != Interlocked.CompareExchange(ref _firstInstance, null, instance))
        {
            // There was no first item OR we could not get the first item,
            // we have to check the other instances
            instance = RentSlow();
        }
        return instance;
    }

    /// <summary>
    /// Returns a <typeparamref name="T"/> instance to this <see cref="ObjectPool{T}"/>
    /// </summary>
    public void Return(T? instance)
    {
        // skip null instances
        if (instance is null) return;

        // Always clean the instance
        _cleanInstance?.Invoke(instance);

        // If we're disposed, dispose the instance and exit
        if (_instances is null)
        {
            _disposeInstance?.Invoke(instance);
            return;
        }

        // Check if we can store in first instance
        if (_firstInstance == null)
        {
            // If we could, Interlocked check to be sure that we are the only thread that can store it
            if (Interlocked.CompareExchange(ref _firstInstance, instance, null) == null)
            {
                // We stored it
                return;
            }
        }

        // We have to try to return it to the instances
        ReturnSlow(instance);
    }

    /// <summary>
    /// <see cref="Rent">Rents</see> a <typeparamref name="T"/> <paramref name="instance"/> from this <see cref="ObjectPool{T}"/>
    /// and get an <see cref="IDisposable"/> that will return that <paramref name="instance"/> when it is disposed
    /// </summary>
    /// <param name="instance">
    /// A <typeparamref name="T"/> instance <see cref="Rent">Rented</see> from this <see cref="ObjectPool{T}"/>
    /// </param>
    /// <returns>
    /// An <see cref="IDisposable"/> that will return that <paramref name="instance"/> when it is disposed
    /// </returns>
    public IDisposable GetInstance(out T instance)
    {
        instance = Rent();
        return new PoolInstance(this, instance);
    }

    private sealed class PoolInstance : IDisposable
    {
        private readonly ObjectPool<T> _pool;
        private T? _instance;

        public PoolInstance(ObjectPool<T> pool, T? instance)
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

    /// <summary>
    /// <see cref="Rent">Rents</see> a <typeparamref name="T"/> instance,
    /// performs an <paramref name="instanceAction"/> on it,
    /// and then <see cref="Return">Returns</see> it to this <see cref="ObjectPool{T}"/>
    /// </summary>
    /// <param name="instanceAction">
    /// An <see cref="Action{T}"/> to perform on the rented instance
    /// </param>
    public void Borrow(Action<T> instanceAction)
    {
        Validate.IsNotNull(instanceAction);
        T instance = Rent();
        instanceAction.Invoke(instance);
        Return(instance);
    }

    /// <summary>
    /// <see cref="Rent">Rents</see> a <typeparamref name="T"/> instance,
    /// performs an <paramref name="instanceFunc"/> on it,
    /// <see cref="Return">Returns</see> it to this <see cref="ObjectPool{T}"/>,
    /// and then returns the result of the <paramref name="instanceFunc"/>
    /// </summary>
    /// <param name="instanceFunc">
    /// An <see cref="Func{T, TResult}"/> to perform on the rented instance and return the result of
    /// </param>
    public TResult Borrow<TResult>(Func<T, TResult> instanceFunc)
    {
        Validate.IsNotNull(instanceFunc);
        T instance = Rent();
        TResult result = instanceFunc.Invoke(instance);
        Return(instance);
        return result;
    }

    /// <summary>
    /// Dispose all stored <typeparamref name="T"/> instances and start automatic disposal of any further <see cref="Return">Returned</see> instances
    /// </summary>
    public void Dispose()
    {
        T? instance;
        // we use _items == null to determine disposal in Return()
        var instances = Interlocked.Exchange<RefInstance[]?>(ref _instances, null);
        if (instances is not null)
        {
            for (var i = 0; i < instances.Length; i++)
            {
                instance = Interlocked.Exchange<T?>(ref instances[i].Instance, null);
                if (instance != null)
                {
                    _disposeInstance?.Invoke(instance);
                }
            }

            // continue to dispose the first instance
            instance = Interlocked.Exchange<T?>(ref _firstInstance, null);
            if (instance is not null)
            {
                _disposeInstance?.Invoke(instance);
            }
        }
    }

    private T RentSlow()
    {
        RefInstance[] instances = _instances!;
        T? instance;
        for (var i = 0; i < instances.Length; i++)
        {
            instance = instances[i].Instance;
            if (instance != null)
            {
                if (instance == Interlocked.CompareExchange(ref instances[i].Instance, null, instance))
                {
                    // found one
                    return instance;
                }
            }
        }

        // we have to create a new instance
        return _instanceFactory();
    }

    private void ReturnSlow(T instance)
    {
        RefInstance[] instances = _instances!;
        for (var i = 0; i < instances.Length; i++)
        {
            if (instances[i].Instance is null)
            {
                if (Interlocked.CompareExchange(ref instances[i].Instance, instance, null) is null)
                {
                    // We stored it
                    break;
                }
            }
        }

        // we could not store this instance, dispose it
        _disposeInstance?.Invoke(instance);
    }
}