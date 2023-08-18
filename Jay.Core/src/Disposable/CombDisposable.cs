namespace Jay.Disposable;

public sealed class MultiDisposable : IDisposable
#if !(NET48 || NETSTANDARD2_0)
, IAsyncDisposable
#endif
{
    public static MultiDisposable Create<T>(params T?[]? disposables)
    {
        var md = new MultiDisposable();
        if (disposables is not null)
        {
            foreach (T? value in disposables)
            {
                md.Add(value);
            }
        }
        return md;
    }
    public static MultiDisposable Create<T>(IEnumerable<T?>? disposables)
    {
        var md = new MultiDisposable();
        if (disposables is not null)
        {
            foreach (T? value in disposables)
            {
                md.Add(value);
            }
        }
        return md;
    }

    private List<object?>? _disposables;

    private MultiDisposable()
    {
        _disposables = new(0);
    }

    public void Add(object? disposable)
    {
        if (_disposables is null)
            throw new ObjectDisposedException(nameof(MultiDisposable));
        _disposables.Add(disposable);
    }

    public void Dispose()
    {
        var trash = Interlocked.Exchange(ref _disposables, null);
        if (trash is not null)
        {
            foreach (object? item in trash)
            {
                if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
#if !(NET48 || NETSTANDARD2_0)
                else if (item is IAsyncDisposable asyncDisposable)
                {
                    // HACK
                    asyncDisposable.DisposeAsync()
                        .GetAwaiter().GetResult();
                }
                #endif
            }
        }
    }
    
#if !(NET48 || NETSTANDARD2_0)
    public async ValueTask DisposeAsync()
    {
        var trash = Interlocked.Exchange(ref _disposables, null);
        if (trash is not null)
        {
            foreach (object? item in trash)
            {
                if (item is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
#endif
}