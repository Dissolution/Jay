namespace Jay.Concurrency;

public sealed class ReadLock : IDisposable
{
    public static ReadLock Acquire(ReaderWriterLockSlim slimLock)
    {
        while (!slimLock.TryEnterReadLock(1))
        {
            Thread.SpinWait(1);
        }
        return new ReadLock(slimLock);
    }
        
    private readonly ReaderWriterLockSlim _slimLock;

    private ReadLock(ReaderWriterLockSlim slimLock)
    {
        _slimLock = slimLock;
    }

    public void Dispose()
    {
        _slimLock.ExitReadLock();
    }
}