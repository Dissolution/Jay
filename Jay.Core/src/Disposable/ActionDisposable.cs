namespace Jay.Disposable;

public sealed class ActionDisposable : IDisposable
{
    private Action? _action;

    public ActionDisposable(Action? action)
    {
        _action = action;
    }

    public void Dispose()
    {
        var action = Interlocked.Exchange(ref _action, null);
        action?.Invoke();
    }
}