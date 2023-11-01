global using SystemConsole = System.Console;

namespace Jay.Terminals.Console;

public interface ITerminalInstance : IDisposable
{
    ITerminalBeep Sound { get; }
    
    ITerminalInput Input { get; }
    ITerminalOutput Output { get; }
    ITerminalErrorOutput Error { get; }
    
    ITerminalBuffer Buffer { get; }
    ITerminalWindow Window { get; }
    ITerminalCursor Cursor { get; }
    ITerminalDisplay Display { get; }

    IDisposable Reset();
}