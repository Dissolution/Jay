namespace Jay.Concurrency;

public sealed class WriteLock : IDisposable
{
    public static WriteLock Acquire(ReaderWriterLockSlim slimLock)
    {
        while (!slimLock.TryEnterWriteLock(1))
            Thread.SpinWait(1);
        return new WriteLock(slimLock);
    }
        
    private readonly ReaderWriterLockSlim _slimLock;

    private WriteLock(ReaderWriterLockSlim slimLock)
    {
        _slimLock = slimLock;
    }

    public void Dispose()
    {
        _slimLock.ExitWriteLock();
    }
}