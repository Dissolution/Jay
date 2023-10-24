namespace Jay.Debugging;

/// <summary>
/// <see cref="EventArgs"/> related to Unhandled <see cref="Exception">Exceptions</see>
/// </summary>
public class UnhandledExceptionArgs : EventArgs
{
    /// <summary>
    /// The source of the Unhandled Exception 
    /// </summary>
    public required string Source { get; init; }

    /// <summary>
    /// The optional Unhandled <see cref="Exception"/>
    /// </summary>
    public required Exception Exception { get; init; }

    /// <summary>
    /// When did this Unhandled Exception occur?
    /// </summary>
    public DateTime Occurrence { get; } = DateTime.Now;
    
    /// <summary>
    /// Did this Unhandled Exception cause this process to terminate?
    /// </summary>
    public bool? IsTerminating { get; init; } = null;

    /// <summary>
    /// Has this Unhandled Exception been observed?
    /// </summary>
    public bool? IsObserved { get; init; } = null;

    public override string ToString() => 
        $"""
        [{Occurrence:yyyy-MM-dd HH:mm:ss.ff}]: Unhandled {Source} Exception
        Observed: {IsObserved.ToString("Yes", "No", "?")}   Terminating: {IsTerminating.ToString("Yes", "No", "?")}
        {Exception.GetType().Name}:
        {Exception.Message}
        {Exception.StackTrace}
        """;
}