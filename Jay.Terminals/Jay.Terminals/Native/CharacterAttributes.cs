using System.Runtime.InteropServices;

namespace Jay.Terminals.Native;

/// <summary>
/// 
/// </summary>
/// <see href="https://learn.microsoft.com/en-us/windows/console/console-screen-buffers#character-attributes"/>
[StructLayout(LayoutKind.Explicit, Size = 2)]
public struct CharacterAttributes
{
    [FieldOffset(0)]
    public TerminalColors Colors;

    [FieldOffset(1)] 
    public CommonLvb CommonLvb;
}