namespace Jay.Utilities;

public static class InstanceCounter
{
    public static string GetInstanceId<T>(this T value)
        where T : class
    {
        return InstanceCounter<T>.GetInstanceId(value);
    }
}

public static class InstanceCounter<T>
    where T : class
{
    private static long _id = 0;

    private static readonly ConditionalWeakTable<T, string> _instanceIds;

    static InstanceCounter()
    {
        _instanceIds = new();
    }

    public static string GetInstanceId(T? value)
    {
        if (value is null) return "null";
        if (_instanceIds.TryGetValue(value, out var identifier))
        {
            return identifier;
        }
        var id = Interlocked.Increment(ref _id);
        identifier = $"{typeof(T).Name}_{id}";
#if NETSTANDARD2_0
        _instanceIds.Add(value, identifier);
#else
        _instanceIds.AddOrUpdate(value, identifier);
#endif
        return identifier;
    }
}