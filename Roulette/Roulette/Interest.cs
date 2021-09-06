using System;
using Jay.Constraints;
using Jay.Text;

namespace Jay.Roulette
{
    public sealed class Interest
    {
        public static Interest Create(FormattableString formattableString)
        {
            return new Interest(formattableString.Format, formattableString.GetArguments());
        }

        public static Interest Create(NonFormattableString format, params object?[] args)
        {
            return new Interest((string)format, args);
        }
        
        internal readonly string _format;
        internal readonly object?[] _args;

        public Interest(string format, params object?[] args)
        {
            _format = format;
            _args = args;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(_format, _args);
        }
    }
}