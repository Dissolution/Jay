namespace Jay.Utilities;

public static class Disposable
{
    public static IDisposable FromAction(Action? action)
    {
        return new ActionDisposable(action);
    }

    /// <summary>
    /// Tries to dispose of <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <paramref name="value"/> to dispose</typeparam>
    /// <param name="value">The value to dispose.</param>
    public static void Dispose<T>(T? value)
    {
        if (value is IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // Ignore all exceptions
            }
        }
    }

    public static void DisposeRef<T>(ref T? value)
        where T : class
    {
        var toDispose = Interlocked.Exchange(ref value, null);
        if (toDispose is IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // Ignore all exceptions
            }
        }
    }
}