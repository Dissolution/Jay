using System;

namespace Jay.Collections.Pools
{
    /// <summary>
    ///     An <see cref="IDisposable"/> wrapper around returning an instance to an object pool
    /// </summary>
    internal sealed class PoolReturner<T> : IDisposable
        where T : class
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
            _pool.Return(_instance);
            _instance = null;
        }
    }
}