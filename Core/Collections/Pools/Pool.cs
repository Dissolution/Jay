namespace Jay.Collections.Pools;

public static class Pool
{
    /// <summary>
    /// The default capacity any pool should start with
    /// </summary>
    internal static readonly int DefaultCapacity = Environment.ProcessorCount * 2;
    /// <summary>
    /// The maximum capacity for any pool
    /// </summary>
    internal static readonly int MaxCapacity = Array.MaxLength;


    public static ObjectPool<T> Create<T>(Action<T>? clean = null, 
                                          Action<T>? dispose = null,
                                          Constraints.IsNew<T> _ = default)
        where T : class, new()
    {
        return new ObjectPool<T>(() => new T(), clean, dispose);
    }

    public static ObjectPool<T> Create<T>(Func<T> factory,
                                          Action<T>? clean = null,
                                          Constraints.IsDisposable<T> _ = default)
        where T : class, IDisposable
    {
        return new ObjectPool<T>(factory, clean, t => t.Dispose());
    }

    public static ObjectPool<T> Create<T>(Action<T>? clean = null,
                                          Constraints.IsNewDisposable<T> _ = default)
        where T : class, IDisposable, new()
    {
        return new ObjectPool<T>(() => new T(), clean, t => t.Dispose());
    }

    public static ObjectPool<T> Create<T>(Func<T> factory,
                                          Action<T>? clean = null,
                                          Action<T>? dispose = null)
        where T : class
    {
        return new ObjectPool<T>(factory, clean, dispose);
    }
}