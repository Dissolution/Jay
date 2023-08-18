namespace Jay.Debugging;

public static class InstIdCounter<T>
    where T : class
{
    // 0L is for null
    private static long _id = 1L;
    // CWT to the rescue
    private static readonly ConditionalWeakTable<T, UIID> _instanceIds = new();

    /// <summary>
    /// Gets the unique Instance Identifier for <paramref name="value"/>
    /// </summary>
    public static UIID GetInstanceId(T? value)
    {
        if (value is null) return UIID.Zero;
        if (_instanceIds.TryGetValue(value, out var uiid))
        {
            return uiid;
        }
        long id = Interlocked.Increment(ref _id);
        uiid = new(id);
#if NET48 || NETSTANDARD2_0
        _instanceIds.Add(value, uiid);
#else
        _instanceIds.AddOrUpdate(value, uiid);
#endif
        return uiid;
    }
}