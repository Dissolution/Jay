using System.Diagnostics;

namespace Jay.Debugging;

/// <summary>
/// A utility for subscribing to all types of unhandled <see cref="Exception"/>s.
/// </summary>
public class UnhandledExceptionWatcher : IDisposable
{
    public event EventHandler<UnhandledExceptionArgs>? UnhandledException;
    
    public UnhandledExceptionWatcher()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    private void CurrentDomainOnUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var unhandledExArgs = new UnhandledExceptionArgs
        {
            Source = nameof(AppDomain), 
            Exception = args.ExceptionObject.AsValid<Exception>(), 
            IsTerminating = args.IsTerminating,
        };
        UnhandledException?.Invoke(sender, unhandledExArgs);
    }

    private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        var unhandledExArgs = new UnhandledExceptionArgs
        {
            Source = nameof(TaskScheduler), 
            Exception = args.Exception, 
            IsObserved = args.Observed,
        };
        UnhandledException?.Invoke(sender, unhandledExArgs);
        // We observed this?
        args.SetObserved();
    }

    public void WriteToDebug()
    {
        this.UnhandledException += static (_, args) => Debug.WriteLine(args.ToString());
    }

    public void WriteToConsole()
    {
        this.UnhandledException += static (_, args) => Console.WriteLine(args.ToString());
    }
    
    /// <summary>
    /// Disposes of our <see cref="UnhandledException"/> <c>event</c> by removing all attached handlers
    /// </summary>
    public virtual void Dispose()
    {
        // By setting the event to null, we no longer hold any references and they can be collected
        Interlocked.Exchange(ref UnhandledException, null);
    }

}