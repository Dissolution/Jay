namespace Jay.Concurrency;

/// <summary>
/// Utility class for wasting CPU cycles
/// </summary>
public static class Wait
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For(TimeSpan timeout)
    {
        Thread.Sleep((int)timeout.TotalMilliseconds);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task ForAsync(int milliseconds, CancellationToken token = default)
    {
        return Task.Delay(milliseconds, token);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task ForAsync(TimeSpan timeout, CancellationToken token = default)
    {
        return Task.Delay(timeout, token);
    }
}