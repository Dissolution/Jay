using System;
using System.Globalization;

namespace Jay.Conversion.New
{
    /// <summary>
    /// A grouping of all the options that can be passed to <see cref="Converter"/> methods.
    /// </summary>
    public readonly struct ConvertOptions
    {
        public static ConvertOptions Number(IFormatProvider? provider = default,
                                            NumberStyles numberStyles = default)
        {
            return new ConvertOptions(provider: provider,
                                      numberStyles: numberStyles);
        }
        public static ConvertOptions DateTime(IFormatProvider? provider = default,
                                              DateTimeStyles dateTimeStyles = default,
                                              string? exactFormat = null,
                                              params string[]? exactFormats)
        {
            return new ConvertOptions(provider: provider,
                                      dateTimeStyles: dateTimeStyles,
                                      exactFormat: exactFormat,
                                      exactFormats: exactFormats);
        }
        public static ConvertOptions TimeSpan(IFormatProvider? provider = default,
                                              TimeSpanStyles timeSpanStyles = default,
                                              string? exactFormat = null,
                                              params string[]? exactFormats)
        {
            return new ConvertOptions(provider: provider,
                                      timeSpanStyles: timeSpanStyles,
                                      exactFormat: exactFormat,
                                      exactFormats: exactFormats);
        }
        
        public static implicit operator ConvertOptions(CultureInfo culture) => new ConvertOptions(provider: culture);
        public static implicit operator ConvertOptions(NumberStyles numberStyles) => new ConvertOptions(numberStyles: numberStyles);
        public static implicit operator ConvertOptions(DateTimeStyles dateTimeStyles) => new ConvertOptions(dateTimeStyles: dateTimeStyles);
        public static implicit operator ConvertOptions(TimeSpanStyles timeSpanStyles) => new ConvertOptions(timeSpanStyles: timeSpanStyles);

        public readonly IFormatProvider? FormatProvider;
        public readonly NumberStyles NumberStyles;
        public readonly DateTimeStyles DateTimeStyles;
        public readonly TimeSpanStyles TimeSpanStyles;
        public readonly bool UseFirstChar;
        private readonly string? _exactFormat;
        private readonly string[]? _exactFormats;
        
        public ConvertOptions(IFormatProvider? provider = default,
                              NumberStyles numberStyles = default,
                              DateTimeStyles dateTimeStyles = default,
                              TimeSpanStyles timeSpanStyles = default,
                              bool useFirstChar = false,
                              string? exactFormat = null,
                              params string[]? exactFormats)
        {
            this.FormatProvider = provider;
            this.NumberStyles = numberStyles;
            this.DateTimeStyles = dateTimeStyles;
            this.TimeSpanStyles = timeSpanStyles;
            this.UseFirstChar = useFirstChar;
            if (string.IsNullOrWhiteSpace(exactFormat))
            {
                _exactFormat = null;
            }
            else
            {
                _exactFormat = exactFormat;
            }
            if (exactFormats is null || exactFormats.Length == 0)
            {
                _exactFormats = null;
            }
            else
            {
                _exactFormats = exactFormats;
            }
        }

        public bool HasExactFormat(out string exactFormat)
        {
            if (_exactFormat is null)
            {
                exactFormat = string.Empty;
                return false;
            }
            else
            {
                exactFormat = _exactFormat;
                return true;
            }
        }

        public bool HasExactFormats(out string[] exactFormats)
        {
            if (_exactFormats is null)
            {
                exactFormats = Array.Empty<string>();
                return false;
            }
            else
            {
                exactFormats = _exactFormats;
                return true;
            }
        }
    }
}