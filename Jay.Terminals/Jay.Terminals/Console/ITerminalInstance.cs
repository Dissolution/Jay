﻿global using SystemConsole = System.Console;

namespace Jay.Terminals.Console;

public interface ITerminalInstance : IDisposable
{
    ITerminalBeep Sound { get; }
    
    ITerminalInput Input { get; }
    ITerminalOutput Output { get; }
    ITerminalErrorOutput Error { get; }
    
    ITerminalBuffer Buffer { get; }
    ITerminalCursor Cursor { get; }
    ITerminalDisplay Display { get; }
    ITerminalWindow Window { get; }

    IDisposable Reset();
}