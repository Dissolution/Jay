using Jay.Terminals.Native;

namespace Jay.Terminals;

/// <summary>
/// Modifies a <c>ref</c> <see cref="TerminalCell"/> argument
/// </summary>
public delegate void ModifyCell(ref TerminalCell cell);