using System;
using System.Collections;
using System.Collections.Generic;
using Jay.Comparison;

namespace Jay.Constraints
{
    /// <summary>
    /// A constraint on a value that implicitly converts to and from <see cref="string"/> and allows for a <see cref="FormattableString"/> to be passed directly to a method.
    /// </summary>
    public readonly struct NonFormattableString : IEquatable<string>
    {
        private readonly string _string;

        public string Value => _string;

        public NonFormattableString(string str)
        {
            _string = str;
        }

        public bool Equals(string? str)
        {
            return string.Equals(_string, str);
        }
        
        public bool Equals(NonFormattableString nfs)
        {
            return string.Equals(_string, nfs._string);
        }
        
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is string str)
                return string.Equals(str, _string);
            if (obj is NonFormattableString nfs)
                return string.Equals(nfs._string, _string);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _string.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString() => _string;

        public static implicit operator NonFormattableString(string str) => new NonFormattableString(str);

        public static explicit operator string(NonFormattableString nfStr) => nfStr._string;

        public static implicit operator NonFormattableString(FormattableString fStr) => throw new InvalidOperationException();
    }
}