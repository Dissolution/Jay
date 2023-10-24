namespace Jay.Debugging;

public static class InstanceId
{
    public static long GetInstanceId<TInstance>(this TInstance? instance)
        where TInstance : class
        => InstanceId<TInstance>.GetInstanceId(instance);
}

public static class InstanceId<T>
    where T : class
{
    private static readonly ConditionalWeakTable<T, byte[]> _instanceIds = new();
    // ReSharper disable once StaticMemberInGenericType
    // I want there to be a separate _instanceCount field for every T
    private static long _instanceCount = 0L;

    /// <summary>
    /// Gets the unique Instance Identifier for <paramref name="instance"/>
    /// </summary>
    public static long GetInstanceId(T? instance)
    {
        if (instance is null) return -1;

        if (_instanceIds.TryGetValue(instance, out var bytes))
        {
            return BitConverter.ToInt64(bytes, 0);
        }
        long id = Interlocked.Increment(ref _instanceCount);
        bytes = BitConverter.GetBytes(id);
        _instanceIds.Add(instance, bytes);
        return id;
    }
}