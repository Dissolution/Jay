namespace Jay;

public static class InstanceCounter
{
    public static string GetIdentifier<T>(this T value)
        where T : class
    {
        return InstanceCounter<T>.GetIdentifier(value);
    }
}

public static class InstanceCounter<T>
    where T : class
{
    private static ulong _id = 0UL;
    
    private static readonly ConditionalWeakTable<T, string> _instanceIds;

    static InstanceCounter()
    {
        _instanceIds = new();
    }

    public static string GetIdentifier(T value)
    {
        //if (_instanceIds.TryGetValue(value, out string identifier))
        throw new NotImplementedException();
    }
}