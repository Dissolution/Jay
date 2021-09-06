using System;

namespace Jay.Conversion
{
    public static partial class Converter
    {
        public static Result TryConvert(object? value, out string text, ConvertOptions options = default)
        {
            if (value is null)
            {
                text = string.Empty;
            }
            else if (value is IFormattable formattable)
            {
                text = formattable.ToString(options.ExactFormat, options.FormatProvider);
            }
            else
            {
                text = value.ToString() ?? string.Empty;
            }
            return true;
        }
    }
}