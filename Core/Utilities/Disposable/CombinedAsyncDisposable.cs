#if !NETSTANDARD2_0

namespace Jay.Utilities;

public sealed class CombinedAsyncDisposable : IAsyncDisposable
{
    private List<IAsyncDisposable?>? _disposables;

    public CombinedAsyncDisposable(params IAsyncDisposable?[]? disposables)
    {
        _disposables = disposables?.ToList();
    }

    public CombinedAsyncDisposable(IEnumerable<IAsyncDisposable?>? disposables)
    {
        _disposables = disposables?.ToList();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposables is not null)
        {
            foreach (IAsyncDisposable? asyncDisposable in _disposables)
            {
                if (asyncDisposable is not null)
                {
                    await asyncDisposable.DisposeAsync();
                }
            }
            _disposables = null;
        }
    }
}

#endif