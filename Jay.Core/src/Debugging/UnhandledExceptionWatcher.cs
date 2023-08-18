namespace Jay.Debugging;

/// <summary>
/// A utility for subscribing to all types of unhandled <see cref="Exception" />s.
/// </summary>
public class UnhandledExceptionWatcher : IDisposable
{
    public UnhandledExceptionWatcher()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref UnhandledException, null);
    }

    public event EventHandler<UnhandledExceptionArgs>? UnhandledException;

    private void CurrentDomainOnUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var unhandledExArgs = new UnhandledExceptionArgs
        {
            Source = UnhandledExceptionSource.AppDomain,
            Exception = args.ExceptionObject as Exception,
            Data = args.ExceptionObject is not Exception ? args.ExceptionObject : null,
            IsTerminating = args.IsTerminating,
        };
        UnhandledException?.Invoke(sender, unhandledExArgs);
    }

    private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        var unhandledExArgs = new UnhandledExceptionArgs
        {
            Source = UnhandledExceptionSource.TaskScheduler,
            Exception = args.Exception,
            IsObserved = args.Observed,
        };
        UnhandledException?.Invoke(sender, unhandledExArgs);
        // We observed this!
        args.SetObserved();
    }
}