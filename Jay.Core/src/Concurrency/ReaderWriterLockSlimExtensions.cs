namespace Jay.Concurrency;

/// <summary>
/// Extensions on <see cref="ReaderWriterLockSlim"/>
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
    /// Gets an <see cref="IDisposable"/> Read Lock for this <see cref="ReaderWriterLockSlim"/>.
    /// </summary>
    public static IDisposable GetReadLock(this ReaderWriterLockSlim slimLock)
    {
        while (!slimLock.TryEnterReadLock(1))
        {
            Thread.SpinWait(1);
        }
        return new ReadLock(slimLock);
    }
    
    private sealed class ReadLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _slimLock;

        public ReadLock(ReaderWriterLockSlim slimLock)
        {
            _slimLock = slimLock;
        }

        public void Dispose()
        {
            _slimLock.ExitReadLock();
        }
    }
    
    /// <summary>
    /// Gets an <see cref="IDisposable"/> Write Lock for this <see cref="ReaderWriterLockSlim"/>.
    /// </summary>
    public static IDisposable GetWriteLock(this ReaderWriterLockSlim slimLock)
    {
        while (!slimLock.TryEnterWriteLock(1))
        {
            Thread.SpinWait(1);
        }
        return new WriteLock(slimLock);
    }
    
    private sealed class WriteLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _slimLock;

        public WriteLock(ReaderWriterLockSlim slimLock)
        {
            _slimLock = slimLock;
        }

        public void Dispose()
        {
            _slimLock.ExitWriteLock();
        }
    }
}