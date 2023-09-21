using Jay.Concurrency;
using Jay.Terminals.Native;

namespace Jay.Terminals.Console;

internal partial class TerminalInstance : ITerminalInstance
{
    private readonly nint _consoleWindowHandle;
    private readonly ReaderWriterLockSlim _slimLock;
    private ConsoleCancelEventHandler? _cancelEventHandler;
    
    public TerminalInstance()
    {
        _consoleWindowHandle = NativeMethods.GetConsoleHandle();
        _slimLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        _cancelEventHandler = null;
        SystemConsole.CancelKeyPress += ConsoleCancelKeyPress;
    }
    private void ConsoleCancelKeyPress(object? sender, ConsoleCancelEventArgs args)
    {
        _cancelEventHandler?.Invoke(sender, args);
    }

    private void ConsoleAction(Action consoleAction)
    {
        _slimLock.TryEnterWriteLock(-1);
        try
        {
            consoleAction();
        }
        finally
        {
            _slimLock.ExitWriteLock();
        }
    }
    
    private TValue ConsoleFunc<TValue>(Func<TValue> getConsoleValue)
    {
        _slimLock.TryEnterReadLock(-1);
        TValue value;
        try
        {
            value = getConsoleValue();
        }
        finally
        {
            _slimLock.ExitReadLock();
        }
        return value;
    }


    public IDisposable Reset()
    {
        throw new NotImplementedException();
    }

    void IDisposable.Dispose()
    {
        using (_slimLock.GetWriteLock())
        {
            SystemConsole.CancelKeyPress -= ConsoleCancelKeyPress;
            _cancelEventHandler = null;
        }
        _slimLock.Dispose();
    }
}