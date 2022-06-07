namespace Jay.Collections.Pools;

internal class AsyncPoolInstance<T> : PoolInstance<T>, IAsyncPoolInstance<T>
    where T : class
{
    public IAsyncObjectPool<T> AsyncPool => (IAsyncObjectPool<T>)_pool;

    public AsyncPoolInstance(IAsyncObjectPool<T> asyncObjectPool, T instance)
        : base(asyncObjectPool, instance)
    {
        
    }

    public async ValueTask DisposeAsync()
    {
        T? instance = Interlocked.Exchange(ref _instance, null);
        await AsyncPool.ReturnAsync(instance);
    }
}