using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Terminals.Native;

/// <see href="https://learn.microsoft.com/en-us/windows/console/char-info-str"/>
[StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Unicode)]
public struct TerminalCell : IEquatable<TerminalCell>, IEqualityOperators<TerminalCell, TerminalCell, bool>
{
    public static bool operator ==(TerminalCell a, TerminalCell b) => a.Value == b.Value;
    public static bool operator !=(TerminalCell a, TerminalCell b) => a.Value != b.Value;
        
    public static TerminalCell Default { get; } = new TerminalCell(' ', TerminalColor.Gray, TerminalColor.Black);

    [FieldOffset(0)] 
    internal int Value;

    [FieldOffset(0)]
    public CharUnion Char;

    [FieldOffset(2)]
    public TerminalColors Colors;

    [FieldOffset(3)] 
    public CommonLvb CommonLvb;

    [FieldOffset(2)]
    public CharacterAttributes Attributes;

    [SkipLocalsInit]
    public TerminalCell(char unicodeChar, TerminalColor foreColor, TerminalColor backColor)
    {
        Value = 0;
        this.Char.UnicodeChar = unicodeChar;
        this.Colors.Foreground = foreColor;
        this.Colors.Background = backColor;
    }

    public bool Equals(TerminalCell cell) => Value == cell.Value;

    public override bool Equals(object? obj)
    {
        return obj is TerminalCell cell && Value == cell.Value;
    }

    public override int GetHashCode() => Value;

    public override string ToString() => Char.ToString();
}