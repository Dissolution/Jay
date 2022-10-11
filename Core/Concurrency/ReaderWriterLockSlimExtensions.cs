namespace Jay.Concurrency;

/// <summary>
/// Extensions on <see cref="ReaderWriterLockSlim"/>
/// </summary>
public static class ReaderWriterLockSlimExtensions
{
    /// <summary>
    /// Gets an <see cref="IDisposable"/> Read Lock for this <see cref="ReaderWriterLockSlim"/>.
    /// </summary>
    public static IDisposable GetReadLock(this ReaderWriterLockSlim rwLock)
    {
        rwLock.EnterReadLock();
        return Disposable.Action(rwLock.ExitReadLock);
    }
        
    /// <summary>
    /// Gets an <see cref="IDisposable"/> Write Lock for this <see cref="ReaderWriterLockSlim"/>.
    /// </summary>
    public static IDisposable GetWriteLock(this ReaderWriterLockSlim rwLock)
    {
        rwLock.EnterWriteLock();
        return Disposable.Action(rwLock.ExitWriteLock);
    }
}