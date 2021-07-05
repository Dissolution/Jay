using Jay.Danger;
using System;
using System.Runtime.InteropServices;

namespace Jay.CLI.Native
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// sizeof(CharInfo) == sizeof(Int32)
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Auto)]
    public struct TermPos : IEquatable<TermPos>
    {
        public static implicit operator int(TermPos terminalPosition) => Abuse.Hack.As<TermPos, int>(terminalPosition);
        public static bool operator ==(TermPos a, TermPos b) => Unmanaged.Equals<TermPos>(a, b);
        public static bool operator !=(TermPos a, TermPos b) => Unmanaged.NotEquals<TermPos>(a, b);
        
        [FieldOffset(0)] 
        public byte AsciiChar;
            
        [FieldOffset(0)] 
        public char UnicodeChar;
            
        [FieldOffset(2)] 
        public CharInfoAttributes Attributes;

        public TermPos(char unicodeChar, TerminalColor foreColor, TerminalColor backColor)
        {
            AsciiChar = default;
            UnicodeChar = unicodeChar;
            Attributes = default;
            Attributes.SetForeColor(foreColor);
            Attributes.SetBackColor(backColor);
        }
        
        public TerminalColor ForeColor
        {
            get => Attributes.GetForeColor();
            set => Attributes.SetForeColor(value);
        }

        public TerminalColor BackColor
        {
            get => Attributes.GetBackColor();
            set => Attributes.SetBackColor(value);
        }

        public (TerminalColor ForeColor, TerminalColor BackColor) Colors
        {
            get => (ForeColor, BackColor);
            set
            {
                Attributes.SetForeColor(value.ForeColor);
                Attributes.SetBackColor(value.BackColor);
            }
        }

        public bool Equals(TermPos terminalPosition)
        {
            return Unmanaged.Equals<TermPos>(this, terminalPosition);
        }

        public override bool Equals(object? obj)
        {
            return obj is TermPos terminalPosition && Equals(terminalPosition);
        }

        public override int GetHashCode()
        {
            return Abuse.Hack.As<TermPos, int>(this);
        }

        public override string ToString()
        {
            return UnicodeChar.ToString();
        }
    }
}