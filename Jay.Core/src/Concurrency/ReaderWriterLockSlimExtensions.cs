namespace Jay.Concurrency;

/// <summary>
/// Extensions on <see cref="ReaderWriterLockSlim" />
/// </summary>
public static class ReaderWriterLockSlimExtensions
{
    public static void WaitForNoLocks(this ReaderWriterLockSlim slimLock)
    {
        while (slimLock.WaitingReadCount > 0 ||
            slimLock.WaitingWriteCount > 0 ||
            slimLock.WaitingUpgradeCount > 0 ||
            slimLock.CurrentReadCount > 0)
        {
            Thread.SpinWait(1);
        }
    }
    
    public static async Task WaitForNoLocksAsync(this ReaderWriterLockSlim slimLock, CancellationToken token = default)
    {
        while (slimLock.WaitingReadCount > 0 ||
            slimLock.WaitingWriteCount > 0 ||
            slimLock.WaitingUpgradeCount > 0 ||
            slimLock.CurrentReadCount > 0)
        {
            await Task.Delay(1, token);
        }
    }
    
    /// <summary>
    /// Gets an <see cref="IDisposable" /> Read Lock for this <see cref="ReaderWriterLockSlim" />.
    /// </summary>
    public static IDisposable GetReadLock(this ReaderWriterLockSlim slimLock)
    {
        return ReadLock.Acquire(slimLock);
    }
    
    /// <summary>
    /// Gets an <see cref="IDisposable" /> Write Lock for this <see cref="ReaderWriterLockSlim" />.
    /// </summary>
    public static IDisposable GetWriteLock(this ReaderWriterLockSlim slimLock)
    {
        return WriteLock.Acquire(slimLock);
    }
}