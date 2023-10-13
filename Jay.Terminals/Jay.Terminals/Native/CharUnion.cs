using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Terminals.Native;

/// <summary>
/// A <c>union</c> between an <c>ASCII</c> <see cref="byte"/> <see cref="AsciiChar"/> and <br/>
/// a <c>UTF16</c> <see cref="char"/> <see cref="UnicodeChar"/>
/// </summary>
/// <see href="https://learn.microsoft.com/en-us/windows/console/char-info-str"/>
[StructLayout(LayoutKind.Explicit, Size = 2, CharSet = CharSet.Unicode)]
public struct CharUnion : 
    IEqualityOperators<CharUnion, CharUnion, bool>,
    IEqualityOperators<CharUnion, char, bool>,
    IEquatable<CharUnion>, 
    IEquatable<char>
{
    public static implicit operator CharUnion(char ch) => new CharUnion(ch);
    public static implicit operator char(CharUnion charUnion) => charUnion.UnicodeChar;
        
    public static bool operator ==(CharUnion x, CharUnion y) => x.UnicodeChar == y.UnicodeChar;
    public static bool operator !=(CharUnion x, CharUnion y) => x.UnicodeChar != y.UnicodeChar;
    public static bool operator ==(CharUnion x, char y) => x.UnicodeChar == y;
    public static bool operator !=(CharUnion x, char y) => x.UnicodeChar != y;
        
    [FieldOffset(0)] 
    public byte AsciiChar;
    [FieldOffset(0)] 
    public char UnicodeChar;

    [SkipLocalsInit]
    public CharUnion(char unicodeChar)
    {
        this.UnicodeChar = unicodeChar;
    }

    public bool Equals(CharUnion charUnion)
    {
        return charUnion.UnicodeChar == this.UnicodeChar;
    }

    public bool Equals(char ch)
    {
        return ch == this.UnicodeChar;
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            CharUnion charUnion => charUnion.UnicodeChar == this.UnicodeChar,
            char ch => ch == this.UnicodeChar,
            _ => false,
        };
    }

    public override int GetHashCode() => (int)UnicodeChar;
    
    public override string ToString()
    {
        return this.UnicodeChar.ToString();
    }
}