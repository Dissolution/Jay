namespace Jay.Concurrency;

public sealed class LockChain : IDisposable
{
    private readonly EventWaitHandle _waitHandle = new ManualResetEvent(false);
    private int _count;

    public int LockCount => _count;

    public IDisposable Lock
    {
        get
        {
            AddLock();
            return Disposable.Disposable.FromAction(RemoveLock);
        }
    }

    public void Dispose()
    {
        _waitHandle.Dispose();
    }

    public void AddLock()
    {
        // If this is the first lock, reset our waiter
        if (Interlocked.Increment(ref _count) == 1)
        {
            _waitHandle.Reset();
        }
    }

    public void RemoveLock()
    {
        // If this is the last lock, trigger our waiter
        if (Interlocked.Decrement(ref _count) == 0)
        {
            _waitHandle.Set();
        }
    }

    public bool WaitForNoLocks()
    {
        return _count == 0 || _waitHandle.WaitOne();
    }
    public bool WaitForNoLocks(TimeSpan timeout)
    {
        return _count == 0 || _waitHandle.WaitOne(timeout);
    }

    public async Task<bool> WaitForNoLocksAsync(CancellationToken token = default)
    {
        if (_count == 0)
            return true;
        return await _waitHandle.WaitAsync(token);
    }

    public async Task<bool> WaitForNoLocksAsync(TimeSpan timeout, CancellationToken token = default)
    {
        if (_count == 0)
            return true;
        return await _waitHandle.WaitAsync(timeout, token);
    }
}