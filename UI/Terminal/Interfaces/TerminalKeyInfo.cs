using Jay.Text;
using System;

namespace Jay.CLI.Interfaces
{
    public readonly struct TerminalKeyInfo : IEquatable<TerminalKeyInfo>
    {
        public static bool operator ==(TerminalKeyInfo a, TerminalKeyInfo b) => a.Equals(b);
        public static bool operator !=(TerminalKeyInfo a, TerminalKeyInfo b) => !a.Equals(b);
        
        public readonly char Char;
        public readonly TerminalKey Key;
        public readonly KeyModifiers Modifiers;

        public TerminalKeyInfo(char @char,
                               TerminalKey terminalKey,
                               KeyModifiers modifiers)
        {
            this.Char = @char;
            this.Key = terminalKey;
            this.Modifiers = modifiers;
        }

        public bool Equals(TerminalKeyInfo info)
        {
            return info.Char == this.Char &&
                   info.Key == this.Key &&
                   info.Modifiers == this.Modifiers;
        }

        public override bool Equals(object? obj)
        {
            return obj is TerminalKeyInfo info && Equals(info);
        }

        public override int GetHashCode()
        {
            // For all normal cases we can fit all bits losslessly into the hash code:
            // _keyChar could be any 16-bit value (though is most commonly ASCII). Use all 16 bits without conflict.
            // _key is 32-bit, but the ctor throws for anything over 255. Use those 8 bits without conflict.
            // _mods only has enum defined values for 1,2,4: 3 bits. Use the remaining 8 bits.
            return (int)this.Char | ((int)Key << 16) | ((int)Modifiers << 24);

        }

        public override string ToString() 
            => TextBuilder.Build(this, (tb, info) =>
            {
                if (info.Modifiers != KeyModifiers.None)
                {
                    tb.Append(info.Modifiers)
                      .Append(" + ");
                }
                tb.Append(info.Key)
                  .Append(" = '")
                  .Append(info.Char)
                  .Append("'");
            });
    }
}