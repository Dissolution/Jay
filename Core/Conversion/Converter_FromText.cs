using System;

namespace Jay.Conversion
{
    public static partial class Converter
    {
        private static Result TryConvert(ReadOnlySpan<char> text, out bool boolean, ConvertOptions options = default)
        {
            if (text.Length == 0)
            {
                boolean = default;
                return ConversionException.Create<bool>(text, options);
            }
            
            if (text.Length == 1 || options.AllowUseOnlyFirstCharacter)
            {
                char c = text[0];
                if (c is '1' or 'T' or 't' or 'Y' or 'y')
                {
                    boolean = true;
                    return true;
                }
                if (c is '0' or 'F' or 'f' or 'N' or 'n')
                {
                    boolean = false;
                    return true;
                }

                boolean = default;
                return ConversionException.Create<bool>(text, options);
            }

            if (text.Equals(bool.TrueString, options.StringComparison))
            {
                boolean = true;
                return true;
            }

            if (text.Equals(bool.FalseString, options.StringComparison))
            {
                boolean = false;
                return true;
            }

            boolean = default;
            return ConversionException.Create<bool>(text, options);
        }
        
        private static Result TryConvert(ReadOnlySpan<char> text, out char character, ConvertOptions options = default)
        {
            if (text.Length == 0)
            {
                character = default;
                return ConversionException.Create<char>(text, options);
            }
            
            if (text.Length == 1 || options.AllowUseOnlyFirstCharacter)
            {
                character = text[0];
                return true;
            }

            character = default;
            return ConversionException.Create<char>(text, options);
        }

        private static Result TryConvert(ReadOnlySpan<char> text, out Guid guid, ConvertOptions options = default)
        {
            if (options.HasExactFormat && Guid.TryParseExact(text, options.ExactFormatSpan, out guid))
            {
                return true;
            }

            if (Guid.TryParse(text, out guid))
            {
                return true;
            }
            
            guid = Guid.Empty;
            return ConversionException.Create<char>(text, options);
        }
        
        private static Result TryConvert(ReadOnlySpan<char> text, out TimeSpan timeSpan, ConvertOptions options = default)
        {
            if (options.HasExactFormats && TimeSpan.TryParseExact(text,
                                                                  options.ExactFormats,
                                                                  options.FormatProvider,
                                                                  options.TimeSpanStyles,
                                                                  out timeSpan))
            {
                return true;
            }

            if (options.HasExactFormat && TimeSpan.TryParseExact(text,
                                                                 options.ExactFormatSpan,
                                                                 options.FormatProvider,
                                                                 options.TimeSpanStyles,
                                                                 out timeSpan))
            {
                return true;
            }

            if (TimeSpan.TryParse(text, options.FormatProvider, out timeSpan))
            {
                return true;
            }

            timeSpan = TimeSpan.Zero;
            return ConversionException.Create<TimeSpan>(text, options);
        }
        
        private static Result TryConvert(ReadOnlySpan<char> text, out DateTime dateTime, ConvertOptions options = default)
        {
            if (options.HasExactFormats && DateTime.TryParseExact(text,
                                                                  options.ExactFormats,
                                                                  options.FormatProvider,
                                                                  options.DateTimeStyles,
                                                                  out dateTime))
            {
                return true;
            }

            if (options.HasExactFormat && DateTime.TryParseExact(text,
                                                                 options.ExactFormatSpan,
                                                                 options.FormatProvider,
                                                                 options.DateTimeStyles,
                                                                 out dateTime))
            {
                return true;
            }

            if (DateTime.TryParse(text, options.FormatProvider, options.DateTimeStyles, out dateTime))
            {
                return true;
            }

            dateTime = DateTime.Now;
            return ConversionException.Create<DateTime>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out DateTimeOffset dateTimeOffset, ConvertOptions options = default)
        {
            if (options.HasExactFormats && DateTimeOffset.TryParseExact(text,
                                                                        options.ExactFormats,
                                                                        options.FormatProvider,
                                                                        options.DateTimeStyles,
                                                                        out dateTimeOffset))
            {
                return true;
            }

            if (options.HasExactFormat && DateTimeOffset.TryParseExact(text,
                                                                       options.ExactFormatSpan,
                                                                       options.FormatProvider,
                                                                       options.DateTimeStyles,
                                                                       out dateTimeOffset))
            {
                return true;
            }

            if (DateTimeOffset.TryParse(text, options.FormatProvider, options.DateTimeStyles, out dateTimeOffset))
            {
                return true;
            }

            dateTimeOffset = DateTimeOffset.Now;
            return ConversionException.Create<DateTimeOffset>(text, options);
        }

        public static Result TryConvert(ReadOnlySpan<char> text, out byte value, ConvertOptions options = default)
        {
            if (byte.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<byte>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out sbyte value, ConvertOptions options = default)
        {
            if (sbyte.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<sbyte>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out short value, ConvertOptions options = default)
        {
            if (short.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<short>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out ushort value, ConvertOptions options = default)
        {
            if (ushort.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<ushort>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out int value, ConvertOptions options = default)
        {
            if (int.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<int>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out uint value, ConvertOptions options = default)
        {
            if (uint.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<uint>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out long value, ConvertOptions options = default)
        {
            if (long.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<long>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out ulong value, ConvertOptions options = default)
        {
            if (ulong.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<ulong>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out float value, ConvertOptions options = default)
        {
            if (float.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<float>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out double value, ConvertOptions options = default)
        {
            if (double.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<double>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out decimal value, ConvertOptions options = default)
        {
            if (decimal.TryParse(text, options.NumberStyles, options.FormatProvider, out value))
            {
                return true;
            }

            value = default;
            return ConversionException.Create<decimal>(text, options);
        }
        
        public static Result TryConvert(ReadOnlySpan<char> text, out string str, ConvertOptions options = default)
        {
            str = new string(text);
            return true;
        }
        
    }
}