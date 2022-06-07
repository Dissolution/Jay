using Jay.Exceptions;

namespace Jay.Collections.Pools;

internal class PoolInstance<T> : IPoolInstance<T>
    where T : class
{
    protected readonly IObjectPool<T> _pool;
    protected T? _instance;

    public T Instance => _instance ?? throw new ObjectDisposedException(GetType().Name);
    public IObjectPool<T> Pool => _pool;

    public PoolInstance(IObjectPool<T> pool, T instance)
    {
        _pool = pool;
        _instance = instance;
    }

    public void Dispose()
    {
        T? instance = Interlocked.Exchange(ref _instance, null);
        _pool.Return(instance);
    }

    public override bool Equals(object? obj) => UnsuitableException.ThrowEquals(GetType());

    public override int GetHashCode() => UnsuitableException.ThrowGetHashCode(GetType());

    public override string ToString() => $"Returns {_instance} to {_pool}";
}