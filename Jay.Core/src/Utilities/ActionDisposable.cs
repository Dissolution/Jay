namespace Jay.Utilities;

/// <summary>
/// An <see cref="IDisposable"/> that executes an <see cref="Action"/> when it is disposed
/// </summary>
public sealed class ActionDisposable : IDisposable
{
    private Action? _action;

    /// <summary>
    /// Construct a new <see cref="ActionDisposable"/> that will execute the given
    /// <paramref name="action"/> when it is disposed
    /// </summary>
    /// <param name="action">
    /// The optional <see cref="Action"/> to execute when <see cref="Dispose"/> is called
    /// </param>
    public ActionDisposable(Action? action)
    {
        _action = action;
    }

    /// <summary>
    /// Executes an optional disposal <see cref="Action"/>
    /// </summary>
    public void Dispose()
    {
        var action = Interlocked.Exchange(ref _action, null);
        action?.Invoke();
    }
}