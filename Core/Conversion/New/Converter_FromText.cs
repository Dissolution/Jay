using System;
using System.Diagnostics.CodeAnalysis;
using Jay.Text;

namespace Jay.Conversion.New
{
    public static partial class Converter
    {
        private static ConversionException GetEx<TResult>(ReadOnlySpan<char> text)
        {
            using var message = TextBuilder.Rent(text.Length * 2);
            message.Append("Cannot parse \"")
                   .Append(text)
                   .Append("\" as a '")
                   .AppendDumpType<TResult>()
                   .Append('\'');
            return ConversionException.Create(typeof(ReadOnlySpan<char>), typeof(TResult), message.ToString());
        }
        
        public static Result TryConvert<TResult>(ReadOnlySpan<char> text, ConvertOptions options, [MaybeNull] out TResult result)
        {
            var valueType = typeof(TResult);
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Empty:
                {
                    result = default;
                    return true;
                }
                case TypeCode.DBNull:
                {
                    Abuse.Hack.Out(DBNull.Value, out result);
                    return true;
                }
                case TypeCode.Boolean:
                {
                    if (bool.TryParse(text, out bool b))
                    {
                        Abuse.Hack.Out(b, out result);
                        return true;
                    }

                    if (options.UseFirstChar || text.Length == 1)
                    {
                        var c = text[0];
                        if (c == 'T' || c == 't' || c == 'Y' || c == 'y' || c == '1')
                        {
                            Abuse.Hack.Out(true, out result);
                            return true;
                        }

                        if (c == 'F' || c == 'f' || c == 'N' || c == 'n' || c == '0')
                        {
                            Abuse.Hack.Out(false, out result);
                            return true;
                        }
                    }

                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.Char:
                {
                    if (options.UseFirstChar || text.Length == 1)
                    {
                        Abuse.Hack.Out(text[0], out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.SByte:
                {
                    if (sbyte.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.Byte:
                {
                    if (byte.TryParse(text, options.NumberStyles, options.FormatProvider,
                                      out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.Int16:
                {
                    if (short.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.UInt16:
                {
                    if (ushort.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.Int32:
                {
                    if (int.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.UInt32:
                {
                    if (uint.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.Int64:
                {
                    if (long.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.UInt64:
                {
                    if (ulong.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.Single:
                {
                    if (float.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.Double:
                {
                    if (double.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.Decimal:
                {
                    if (decimal.TryParse(text, options.NumberStyles, options.FormatProvider,
                                       out var number))
                    {
                        Abuse.Hack.Out(number, out result);
                        return true;
                    }
                    
                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.DateTime:
                {
                    DateTime dt;
                    
                    if (DateTime.TryParse(text, options.FormatProvider, options.DateTimeStyles, 
                                          out dt))
                    {
                        Abuse.Hack.Out(dt, out result);
                        return true;
                    }

                    if (options.HasExactFormat(out var exactFormat) &&
                        DateTime.TryParseExact(text, exactFormat, options.FormatProvider, options.DateTimeStyles,
                                                                                 out dt))
                    {
                        Abuse.Hack.Out(dt, out result);
                        return true;
                    }

                    if (options.HasExactFormats(out var exactFormats) &&
                        DateTime.TryParseExact(text, exactFormats, options.FormatProvider,
                                               options.DateTimeStyles, out dt))
                    {
                        Abuse.Hack.Out(dt, out result);
                        return true;
                    }

                    result = default;
                    return GetEx<TResult>(text);
                }
                case TypeCode.String:
                {
                    Abuse.Hack.Out(new string(text), out result);
                    return true;
                }
                case TypeCode.Object:
                default:
                    break;
            }

            if (valueType == typeof(TimeSpan))
            {
                TimeSpan ts;
                    
                if (TimeSpan.TryParse(text, options.FormatProvider, out ts))
                {
                    Abuse.Hack.Out(ts, out result);
                    return true;
                }

                if (options.HasExactFormat(out var exactFormat) &&
                    TimeSpan.TryParseExact(text, exactFormat, options.FormatProvider, options.TimeSpanStyles, out ts))
                {
                    Abuse.Hack.Out(ts, out result);
                    return true;
                }

                if (options.HasExactFormats(out var exactFormats) &&
                    TimeSpan.TryParseExact(text, exactFormats, options.FormatProvider, options.TimeSpanStyles, out ts))
                {
                    Abuse.Hack.Out(ts, out result);
                    return true;
                }
            }
            
            if (valueType == typeof(Guid))
            {
                Guid g;

                if (Guid.TryParse(text, out g))
                {
                    Abuse.Hack.Out(g, out result);
                    return true;
                }
                
                if (options.HasExactFormat(out var exactFormat) &&
                    Guid.TryParseExact(text, exactFormat, out g))
                {
                    Abuse.Hack.Out(g, out result);
                    return true;
                }
            }
            
            if (valueType == typeof(DateTimeOffset))
            {
                DateTimeOffset dto;
                    
                if (DateTimeOffset.TryParse(text, options.FormatProvider, options.DateTimeStyles, out dto))
                {
                    Abuse.Hack.Out(dto, out result);
                    return true;
                }

                if (options.HasExactFormat(out var exactFormat) &&
                    DateTimeOffset.TryParseExact(text, exactFormat, options.FormatProvider, options.DateTimeStyles, out dto))
                {
                    Abuse.Hack.Out(dto, out result);
                    return true;
                }

                if (options.HasExactFormats(out var exactFormats) &&
                    DateTimeOffset.TryParseExact(text, exactFormats, options.FormatProvider, options.DateTimeStyles, out dto))
                {
                    Abuse.Hack.Out(dto, out result);
                    return true;
                }
            }
            
            // Cannot convert
            result = default;
            return GetEx<TResult>(text);
        }
        
        public static Result TryConvert<TValue>(string? text, ConvertOptions options, [MaybeNull] out TValue value)
        {
            // The only thing we save through this path is if TValue is string
            if (typeof(TValue) == typeof(string))
            {
                Abuse.Hack.Out(text, out value);
                return true;
            }
            return TryConvert<TValue>(text.AsSpan(), options, out value);
        }
    }
}