namespace Jay.Collections.Pools;

public static class Pool
{
    internal static readonly int DefaultCapacity = Environment.ProcessorCount * 2;
    internal const int MaxCapacity = 0X7FEFFFFF; // Array.MaxArrayLength

    public static ObjectPool<T> Create<T>(Func<T> factory)
        where T : class
        => new ObjectPool<T>(factory, null, null);

    public static ObjectPool<T> Create<T>(Func<T> factory,
                                          Action<T>? clean)
        where T : class
        => new ObjectPool<T>(factory, clean, null);
        
    public static ObjectPool<T> Create<T>(Func<T> factory,
                                          Action<T>? clean,
                                          Action<T>? dispose)
        where T : class
        => new ObjectPool<T>(factory, clean, dispose);
}