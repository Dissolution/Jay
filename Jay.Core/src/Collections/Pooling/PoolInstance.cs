namespace Jay.Collections.Pooling;

internal sealed class PoolInstance<T> : IPoolInstance<T>
    where T : class
{
    private readonly IObjectPool<T> _pool;
    private T? _instance;

    public PoolInstance(IObjectPool<T> pool, T instance)
    {
        _pool = pool;
        _instance = instance;
    }

    public T Instance => _instance ?? throw new ObjectDisposedException(nameof(Instance));

    public void Dispose()
    {
        T? instance = Interlocked.Exchange(ref _instance, null);
        _pool.Return(instance);
    }

    public override bool Equals(object? _) => throw new NotSupportedException();

    public override int GetHashCode() => throw new NotSupportedException();

    public override string ToString()
    {
        if (_instance is null)
            return "Disposed";
        return $"Instance of {typeof(T).Name} '{_instance}'";
    }
}