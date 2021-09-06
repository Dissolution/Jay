using System;
using System.Runtime.InteropServices;
using Jay.Exceptions;

namespace Jay.Sandbox.Native
{
    [StructLayout(LayoutKind.Explicit, Size = 2, CharSet = CharSet.Unicode)]
    public struct CharUnion : IEquatable<CharUnion>, IEquatable<char>
    {
        public static implicit operator CharUnion(char c) => new CharUnion(c);
        public static implicit operator char(CharUnion charUnion) => charUnion.UnicodeChar;
        
        public static bool operator ==(CharUnion x, CharUnion y) => x.UnicodeChar == y.UnicodeChar;
        public static bool operator !=(CharUnion x, CharUnion y) => x.UnicodeChar != y.UnicodeChar;
        public static bool operator ==(CharUnion x, char y) => x.UnicodeChar == y;
        public static bool operator !=(CharUnion x, char y) => x.UnicodeChar != y;
        
        [FieldOffset(0)] 
        public byte AsciiChar;
        [FieldOffset(0)] 
        public char UnicodeChar;

        public CharUnion(char unicodeChar)
        {
            this.AsciiChar = default;
            this.UnicodeChar = unicodeChar;
        }

        /// <inheritdoc />
        public bool Equals(CharUnion charUnion)
        {
            return charUnion.UnicodeChar == this.UnicodeChar;
        }

        /// <inheritdoc />
        public bool Equals(char c)
        {
            return c == this.UnicodeChar;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is CharUnion charUnion) return charUnion.UnicodeChar == this.UnicodeChar;
            if (obj is char c) return c == this.UnicodeChar;
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeException.Throw(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.UnicodeChar.ToString();
        }
    }
}