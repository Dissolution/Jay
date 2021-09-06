using System;
using System.Globalization;
using Jay.Debugging.Dumping;
using Jay.Text;

namespace Jay.Conversion
{
    public readonly struct ConvertOptions : IEquatable<ConvertOptions>
    {
        public static readonly ConvertOptions Default = new ConvertOptions();

        public readonly StringComparison StringComparison;
        public readonly bool AllowUseOnlyFirstCharacter;
        public readonly IFormatProvider? FormatProvider;
        public readonly TimeSpanStyles TimeSpanStyles;
        public readonly DateTimeStyles DateTimeStyles;
        public readonly NumberStyles NumberStyles;

        private readonly string? _exactFormat;
        private readonly string[]? _exactFormats;

        public bool HasExactFormat => !string.IsNullOrWhiteSpace(_exactFormat);
        public ReadOnlySpan<char> ExactFormatSpan => _exactFormat;
        public string? ExactFormat => _exactFormat;

        public bool HasExactFormats => !_exactFormats.NullOrNone();
        public string[]? ExactFormats => _exactFormats;
        
        public ConvertOptions(StringComparison stringComparison = StringComparison.CurrentCulture,
                              bool allowUseOnlyFirstCharacter = false)
        {
            this.StringComparison = stringComparison;
            this.AllowUseOnlyFirstCharacter = allowUseOnlyFirstCharacter;
            this._exactFormat = null;
            this._exactFormats = null;
            this.FormatProvider = null;
            this.TimeSpanStyles = default;
            this.DateTimeStyles = default;
            this.NumberStyles = default;
        }
        
        /// <inheritdoc />
        public bool Equals(ConvertOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is ConvertOptions options && Equals(options);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Dumper.DumpProperties<ConvertOptions>(this, true);
        }
    }
}