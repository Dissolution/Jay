using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Jay.NotSafe;

namespace Jay.UI.Terminal.Native
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// sizeof(CharInfo) == sizeof(Int32)
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Auto)]
    public struct Coxel : IEquatable<Coxel>
    {
        public static implicit operator int(Coxel coxel) => Unmanaged.As<Coxel, int>(coxel);
        public static bool operator ==(Coxel a, Coxel b) => Unmanaged.Equal(a, b);
        public static bool operator !=(Coxel a, Coxel b) => Unmanaged.NotEqual(a, b);
        
        [FieldOffset(0)] 
        public byte AsciiChar;
            
        [FieldOffset(0)] 
        public char UnicodeChar;
            
        [FieldOffset(2)] 
        public CoxelColors Colors;

        public Coxel(char unicodeChar, TerminalColor foreColor, TerminalColor backColor)
        {
            AsciiChar = default;
            UnicodeChar = unicodeChar;
            Colors = CoxelColorsExtensions.Combine(foreColor, backColor);
        }
        
        public Coxel(char unicodeChar, CoxelColors colors)
        {
            AsciiChar = default;
            UnicodeChar = unicodeChar;
            Colors = colors;
        }
        
        public TerminalColor ForeColor
        {
            get => Colors.GetForeColor();
            set => Colors.SetForeColor(value);
        }

        public TerminalColor BackColor
        {
            get => Colors.GetBackColor();
            set => Colors.SetBackColor(value);
        }

        public bool Equals(Coxel coxel)
        {
            return NotSafe.Unmanaged.Equal(this, coxel);
        }

        public override bool Equals(object? obj)
        {
            return obj is Coxel terminalPosition && Equals(terminalPosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Unmanaged.As<Coxel, int>(this);

        public override string ToString()
        {
            return UnicodeChar.ToString();
        }
    }
}