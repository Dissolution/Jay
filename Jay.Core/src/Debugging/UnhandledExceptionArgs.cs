namespace Jay.Debugging;

public class UnhandledExceptionArgs : EventArgs
{
    public UnhandledExceptionSource Source { get; init; } = UnhandledExceptionSource.Unknown;

    public Exception? Exception { get; init; } = null;

    public object? Data { get; init; } = null;

    public bool IsTerminating { get; init; } = false;

    public bool IsObserved { get; init; } = false;
}