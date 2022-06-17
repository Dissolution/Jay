using System.Diagnostics;
using Jay.Dumping;
using Jay.Exceptions;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace Jay.Collections.Pools;

/// <summary>
/// A thread-safe async-pool of <typeparamref name="T"/> instances.
/// </summary>
/// <typeparam name="T">An instance class</typeparam>
public class AsyncObjectPool<T> : ObjectPool<T>, IAsyncObjectPool<T>,
                                         IAsyncDisposable, IDisposable
    where T : class
{
    private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
    
    private static T CreateInstanceSync(AsyncFunc<T> asyncFactory)
    {
        try
        {
            return Task.Run(async () =>
            {
                using var cts = new CancellationTokenSource();
                var task = asyncFactory(cts.Token);
                var timeout = Task.Delay(_timeout, cts.Token);
                var completed = await Task.WhenAny(task, timeout);
                cts.Cancel();
                if (completed == task)
                    return await task.ConfigureAwait(false);
                throw new TimeoutException(Dump.Text($"Could not create a {typeof(T)} instance in {_timeout}"));
            }).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            var dex = Dump.Value(ex);
            Debugger.Break();
            throw;
        }
    }
    
    /// <summary>
    /// Value creation factory.
    /// </summary>
    private readonly AsyncFunc<T> _asyncFactory;

    /// <summary>
    /// Optional instance clean action.
    /// </summary>
    private readonly AsyncAction<T>? _asyncClean;

    /// <summary>
    /// Optional instance disposal action.
    /// </summary>
    private readonly AsyncAction<T>? _asyncDispose;

    public AsyncObjectPool(AsyncFunc<T> factory,
                           AsyncAction<T>? clean = null,
                           AsyncAction<T>? dispose = null)
        : this(Pool.DefaultCapacity, factory, clean, dispose)
    {
    }

    public AsyncObjectPool(int capacity,
                           AsyncFunc<T> factory,
                           AsyncAction<T>? clean = null,
                           AsyncAction<T>? dispose = null)
        : base(capacity, () => CreateInstanceSync(factory), null, null)
    {
        if (capacity < 1 || capacity > Pool.MaxCapacity)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity,
                $"Pool Capacity must be 1 <= {capacity} <= {Pool.MaxCapacity}");
        }

        _asyncFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        _asyncClean = clean;
        _asyncDispose = dispose;
    }
    
    /// <summary>
    /// When we cannot find an available item quickly with Rent()
    /// </summary>
    private async Task<T> RentSlowAsync()
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
        return await _asyncFactory();
    }

    private async Task ReturnSlowAsync(T instance)
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
        if (_asyncDispose != null)
        {
            await _asyncDispose(instance);
        }
    }

    /// <summary>
    /// Rents a <typeparamref name="T"/> instance that should be <see cref="ReturnAsync"/>ed once it is done being used.
    /// </summary>
    /// <remarks>
    /// Search strategy is a simple linear probing which is chosen for it cache-friendliness.
    /// Note that Free will try to store recycled objects close to the start thus statistically reducing how far we will typically search.
    /// </remarks>
    public async Task<T> RentAsync()
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
            instance = await RentSlowAsync();
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
    public async Task ReturnAsync(T? instance)
    {
        if (instance is null) return;

        // Always clean the item
        if (_asyncClean != null)
        {
            await _asyncClean(instance);
        }
            
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
        await ReturnSlowAsync(instance);
    }
        
    /// <summary>
    /// Frees all stored <typeparamref name="T"/> instances.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Increment(ref _disposed) > 1) return;
        if (_asyncDispose != null)
        {
            var dispose = _asyncDispose!;
            T? item = Reference.Exchange(ref _firstItem, null);
            if (item != null)
                await dispose(item);
            var items = _items;
            for (var i = 0; i < items.Length; i++)
            {
                item = Reference.Exchange(ref items[i].Value, null);
                if (item != null)
                    await dispose(item);
            }
        }
        Debug.Assert(_items.All(item => item.Value is null));
    }
}