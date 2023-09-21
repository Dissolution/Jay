using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Terminals.Native;

/// <summary>
/// A union struct between a 4-bit Foreground <see cref="TerminalColor"/> and a 4-bit Background <see cref="TerminalColor"/>
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = sizeof(byte))]
public struct TerminalColors : 
    IEquatable<TerminalColors>, 
    IEqualityOperators<TerminalColors, TerminalColors, bool>
{
    public static implicit operator TerminalColors(byte value) => new TerminalColors(value);
    public static implicit operator TerminalColors((TerminalColor ForeColor, TerminalColor BackColor) colors)
        => new TerminalColors(colors.ForeColor, colors.BackColor);

    public static bool operator ==(TerminalColors x, TerminalColors y) => x.Value == y.Value;
    public static bool operator !=(TerminalColors x, TerminalColors y) => x.Value != y.Value;
        
    public static readonly TerminalColors None = new TerminalColors();
    public static readonly TerminalColors Default = new TerminalColors(TerminalColor.Gray, TerminalColor.Black);
    
    [FieldOffset(0)] 
    internal byte Value;

    public TerminalColor Foreground
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (TerminalColor) (Value & 0b00001111);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Value = (byte) ((Value & 0b11110000) | (int) value);
    }
        
    public TerminalColor Background
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (TerminalColor) ((Value & 0b11110000) >> 4);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Value = (byte) ((Value & 0b00001111) | ((int) value << 4));
    }

    private TerminalColors(byte value)
    {
        Value = value;
    }
    
    public TerminalColors(TerminalColor foreColor, TerminalColor backColor)
    {
        Value = (byte) ((int) foreColor | ((int) backColor << 4));
    }
    public void Deconstruct(out TerminalColor foreColor, out TerminalColor backColor)
    {
        byte value = Value;
        foreColor = (TerminalColor) (value & 0b00001111);
        backColor = (TerminalColor) ((value & 0b11110000) >> 4);
    }
    
    public bool Equals(TerminalColors colors)
    {
        return colors.Value == this.Value;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is TerminalColors colors && colors.Value == this.Value;
    }
    
    public override int GetHashCode()
    {
        return Value;
    }
    
    public override string ToString()
    {
        return $"{Foreground}/{Background}";
    }
}