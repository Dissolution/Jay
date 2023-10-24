using System.Reflection;

namespace Jay.Concurrency;

/// <summary>
/// A simple <see cref="Mutex"/> that ensures only a single instance of this Application can be running<br/>
/// Use <see cref="Acquire"/> or <see cref="TryAcquire"/> to obtain the <see cref="OnlyApplication"/><br/>
/// Dispose of it at the end of your application's execution with a <c>using</c>:<br/>
/// <c>using var only = OnlyApplication.Acquire();</c><br/>
/// <c>using var only = OnlyApplication.TryAcquire(TimeSpan.FromMinutes(1)).OkValueOrThrowError();</c>
/// </summary>
public sealed class OnlyApplication : IDisposable
{
    private static string GetAppName() => Assembly.GetExecutingAssembly().GetName().FullName;

    public static OnlyApplication Acquire() => TryAcquire().OkValueOrThrowError();
    
    public static Result<OnlyApplication> TryAcquire(TimeSpan? timeout = null, CancellationToken token = default)
    {
        Mutex mutex = new Mutex(
            initiallyOwned: true,
            name: GetAppName(),
            out bool createdNew);
        if (createdNew)
            return new OnlyApplication(mutex);
        if (timeout.TryGetValue(out var timespan))
        {
            try
            {
                createdNew = mutex.WaitOne(timespan);
            }
            catch (AbandonedMutexException amex)
            {
                Console.WriteLine(amex.Message);
                return amex;
            }
        }
        else
        {
            // Spin to check for cancellation
            while (!createdNew && !token.IsCancellationRequested)
            {
                try
                {
                    createdNew = mutex.WaitOne(1_000); // 1 second
                }
                catch (AbandonedMutexException amex)
                {
                    Console.WriteLine(amex.Message);
                    return amex;
                }
            }
        }
        token.ThrowIfCancellationRequested();
        if (createdNew)
            return new OnlyApplication(mutex);
        return new InvalidOperationException("Could not acquire an exclusive SingleInstance");
    }
    
    
    private readonly Mutex _mutex;

    private OnlyApplication(Mutex mutex)
    {
        _mutex = mutex;
    }
    ~OnlyApplication()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        _mutex.ReleaseMutex();
        _mutex.Dispose();
        GC.SuppressFinalize(this);
    }
    
    public override string ToString()
    {
        return $"Single Instance of {GetAppName()}";
    }
}