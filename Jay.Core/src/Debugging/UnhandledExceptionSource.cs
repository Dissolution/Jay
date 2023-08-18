namespace Jay.Debugging;

public enum UnhandledExceptionSource
{
    Unknown = 0,

    /// <summary>
    /// <see cref="E:AppDomain.CurrentDomain.UnhandledException" />
    /// </summary>
    AppDomain = 1,

    /// <summary>
    /// <see cref="E:TaskScheduler.UnobservedTaskException" />
    /// </summary>
    TaskScheduler = 2,
}