namespace Jay.Utilities;

public sealed class CombinedDisposable : IDisposable
{
    private List<IDisposable?>? _disposables;

    public CombinedDisposable(params IDisposable?[]? disposables)
    {
        _disposables = disposables?.ToList();
    }

    public CombinedDisposable(IEnumerable<IDisposable?>? disposables)
    {
        _disposables = disposables?.ToList();
    }

    public void Dispose()
    {
        if (_disposables is not null)
        {
            foreach (IDisposable? disposable in _disposables)
            {
                disposable?.Dispose();
            }
            _disposables = null;
        }
    }
}