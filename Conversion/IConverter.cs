/*
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Conversion
{
    public readonly struct TextConvertOptions
    {
        public readonly FormatStrings Format = default;
        public readonly NumberStyles NumberStyle = NumberStyles.Any;
        public readonly DateTimeStyles DateTimeStyle = DateTimeStyles.None;
        public readonly bool UseFirstChar = false;
        public readonly bool CharToNumber = false;

        public bool FirstChar(string? value, out char c)
        {
            if (value is not null && value.Length != 0 && (value.Length == 1 || UseFirstChar))
            {
                c = value[0];
                return true;
            }

            c = default;
            return false;
        }

        public bool FirstChar(ReadOnlySpan<char> value, out char c)
        {
            if (value.Length != 0 && (value.Length == 1 || UseFirstChar))
            {
                c = value[0];
                return true;
            }

            c = default;
            return false;
        }
    }

    public readonly struct ConvertOptions
    {
        public readonly IFormatProvider? FormatProvider = null;
        public readonly TextConvertOptions Text = new TextConvertOptions();
    }

    public interface IConverter
    {
        bool TryConvert(object? input, Type? outType, out object? output, ConvertOptions options = default);
    }

    public interface IInputConverter<TIn> : IConverter
    {
        bool TryConvert(TIn? input, Type? outType, out object? output, ConvertOptions options = default);
    }

    public interface IOutputConverter<TOut> : IConverter
    {
        bool TryConvert(object? input, out TOut? output, ConvertOptions options = default);
    }

    public interface IConverter<TIn, TOut> : IInputConverter<TIn>,
                                             IOutputConverter<TOut>,
                                             IConverter
    {
        bool TryConvert(TIn? input, out TOut? output, ConvertOptions options = default);

        bool IConverter.TryConvert(object? input, Type? outType, out object? output, ConvertOptions options)
        {
            if (input is TIn typedInput)
            {
                return TryConvert(typedInput, outType, out output, options);
            }

            output = default;
            return true;
        }

        bool IInputConverter<TIn>.TryConvert(TIn? input, Type? outType, out object? output, ConvertOptions options)
        {
            if (outType is null)
            {
                output = default;
                return true;
            }

            if (!outType.IsAssignableFrom(typeof(TOut)))
            {
                output = default;
                return false;
            }

            if (TryConvert(input, out TOut? typedOutput, options))
            {
                output = typedOutput;
                return true;
            }

            output = default;
            return false;
        }

        bool IOutputConverter<TOut>.TryConvert(object? input, out TOut? output, ConvertOptions options)
        {
            if (input is TIn typedInput)
            {
                return TryConvert(typedInput, out output, options);
            }

            output = default;
            return false;
        }
    }

    public interface ITextInputConverter : IInputConverter<char>,
                                           IInputConverter<string>,
                                           IInputConverter<char[]>,
                                           IInputConverter<IEnumerable<char>>,
                                           IInputConverter<IFormattable>,
                                           IConverter
    {
        bool TryConvert(ReadOnlySpan<char> input, Type? outType, out object? output, ConvertOptions options = default);

        bool IConverter.TryConvert(object? input, Type? outType, out object? output, ConvertOptions options)
        {
            if (input is char c)
                return TryConvert(c, outType, out output, options);
            if (input is string str)
                return TryConvert(str, outType, out output, options);
            if (input is char[] charArray)
                return TryConvert(charArray, outType, out output, options);
            if (input is IEnumerable<char> chars)
                return TryConvert(chars, outType, out output, options);
            return TryConvert(input?.ToString(), outType, out output, options);
        }

        bool IInputConverter<char[]>.TryConvert(char[]? input, Type? outType, out object? output, ConvertOptions options) 
            => TryConvert((ReadOnlySpan<char>)input, outType, out output, options);

        bool IInputConverter<IEnumerable<char>>.TryConvert(IEnumerable<char>? input, Type? outType, out object? output, ConvertOptions options)
            => TryConvert(new string(input?.ToArray()), outType, out output, options);

        bool IInputConverter<IFormattable>.TryConvert(IFormattable? input, Type? outType, out object? output, ConvertOptions options)
            => TryConvert(input?.ToString(options.Text.Format, options.FormatProvider), outType, out output, options);
    }

    public interface ITextConverter<TOut> : ITextInputConverter,
                                            IConverter<char, TOut>,
                                            IConverter<string, TOut>,
                                            IConverter<char[], TOut>,
                                            IConverter<IEnumerable<char>, TOut>,
                                            IConverter<IFormattable, TOut>,
                                            IConverter
    {
        bool TryConvert(ReadOnlySpan<char> input, out TOut? output, ConvertOptions options = default);
       
        bool IOutputConverter<TOut>.TryConvert(object? input, out TOut? output, ConvertOptions options)
        {
            if (input is char c)
                return TryConvert(c, out output, options);
            if (input is string str)
                return TryConvert(str, out output, options);
            if (input is char[] charArray)
                return TryConvert(charArray, out output, options);
            if (input is IEnumerable<char> chars)
                return TryConvert(chars, out output, options);
            return TryConvert(input?.ToString(), out output, options);
        }

        bool IConverter<char[], TOut>.TryConvert(char[]? input, out TOut? output, ConvertOptions options) 
            => TryConvert((ReadOnlySpan<char>)input, out output, options);

        bool IConverter<IEnumerable<char>, TOut>.TryConvert(IEnumerable<char>? input, out TOut? output, ConvertOptions options)
            => TryConvert(new string(input?.ToArray()), out output, options);

        bool IConverter<IFormattable, TOut>.TryConvert(IFormattable? input, out TOut? output, ConvertOptions options)
            => TryConvert(input?.ToString(options.Text.Format, options.FormatProvider), out output, options);
    }

    public abstract class Converter
    {
        public static ConverterCache Default { get; }
    }

    public abstract class ConverterCache : IConverter
    {
        protected readonly ConcurrentDictionary<ConvTypes, IConverter> _cache;

        protected ConverterCache()
        {
            _cache = new ConcurrentDictionary<ConvTypes, IConverter>();
        }

        protected virtual bool TryGetConverter(Type? inType, Type? outType, [NotNullWhen(true)] out IConverter? converter)
        {
            return _cache.TryGetValue((inType, outType), out converter);
        }
        
        public bool TryConvert(object? input, Type? outType, out object? output, ConvertOptions options = default)
        {
            if (TryGetConverter(input?.GetType(), outType, out var converter))
            {
                return converter.TryConvert(input, outType, out output, options);
            }
            output = null;
            return false;
        }

        public bool TryConvert<TIn>(TIn? input, Type? outType, out object? output, ConvertOptions options = default)
        {
            if (TryGetConverter(typeof(TIn), outType, out var converter))
            {
                if (converter is IInputConverter<TIn> inputConverter)
                {
                    return inputConverter.TryConvert(input, outType, out output, options);
                }
                return converter.TryConvert(input, outType, out output, options);
            }
            output = null;
            return false;
        }

        public bool TryConvert<TOut>(object? input, out TOut? output, ConvertOptions options = default)
        {
            if (TryGetConverter(input?.GetType(), typeof(TOut), out var converter))
            {
                if (converter is IOutputConverter<TOut> outputConverter)
                {
                    return outputConverter.TryConvert(input, out output, options);
                }

                if (converter.TryConvert(input, typeof(TOut), out object? obj, options))
                {
                    return obj.Is<TOut>(out output);
                }
            }
            output = default;
            return false;
        }

        public bool TryConvert<TIn, TOut>(TIn? input, out TOut? output, ConvertOptions options = default)
        {
            if (TryGetConverter(typeof(TIn), typeof(TOut), out var converter))
            {
                if (converter is IConverter<TIn, TOut> ioConverter)
                {
                    return ioConverter.TryConvert(input, out output, options);
                }

                if (converter is IOutputConverter<TOut> outputConverter)
                {
                    return outputConverter.TryConvert(input, out output, options);
                }

                object? obj;

                if (converter is IInputConverter<TIn> inputConverter)
                {
                    if (inputConverter.TryConvert(input, typeof(TOut), out obj, options))
                    {
                        return obj.Is<TOut>(out output);
                    }
                }

                if (converter.TryConvert(input, typeof(TOut), out obj, options))
                {
                    return obj.Is<TOut>(out output);
                }
            }
            output = default;
            return false;
        }
    }

    internal sealed class SmartConverterCache : ConverterCache
    {
        protected override bool TryGetConverter(Type? inType, Type? outType, [NotNullWhen(true)] out IConverter? converter)
        {
            converter = _cache.GetOrAdd((inType, outType), CreateConverter);
            return true;
        }

        private IConverter CreateConverter(ConvTypes types)
        {
            if (types.OutType == typeof(string))
                return new FormattableConverter();

            throw new NotImplementedException();
        }
    }

    internal sealed class FormattableConverter : IConverter,
                                                 IConverter<IFormattable, string>,
                                                 IOutputConverter<string>
    {
        public bool TryConvert(IFormattable? input, out string? output, ConvertOptions options = default)
        {
            output = input?.ToString(options.Text.Format, options.FormatProvider);
            return true;
        }

        public bool TryConvert(object? input, out string? output, ConvertOptions options = default)
        {
            if (input is IFormattable formattable)
                return TryConvert(formattable, out output, options);
            output = input?.ToString();
            return true;
        }
    }

    internal sealed class TextTryParseConverter : IConverter,
                                                  ITextInputConverter,
                                                  ITextConverter<bool>,
                                                  ITextConverter<byte>
    {
        private static readonly HashSet<Type> _inputTypes;
        private static readonly HashSet<Type> _outputTypes;

        static TextTryParseConverter()
        {
            _inputTypes = new HashSet<Type>
            {
                typeof(string),
                typeof(char[]),
                typeof(IEnumerable<char>),
                typeof(ReadOnlySpan<char>),
                typeof(IFormattable),
                typeof(object),
            };

            _outputTypes = new HashSet<Type>
            {
                typeof(byte),
            };
        }

        public bool TryConvert(char input, Type? outType, out object? output, ConvertOptions options = default)
        {
            throw new NotImplementedException();
        }

        public bool TryConvert(string? input, Type? outType, out object? output, ConvertOptions options = default)
        {
            throw new NotImplementedException();
        }

        public bool TryConvert(ReadOnlySpan<char> input, Type? outType, out object? output, ConvertOptions options = default)
        {
            throw new NotImplementedException();
        }

        public bool TryConvert(char input, out bool output, ConvertOptions options = default)
        {
            if (input == 't' || input == 'T' || input == '1' || input == 'Y' || input == 'y')
            {
                output = true;
                return true;
            }

            if (input == 'f' || input == 'F' || input == '0' || input == 'N' || input == 'n')
            {
                output = false;
                return true;
            }

            output = false;
            return false;
        }

        public bool TryConvert(string? input, out bool output, ConvertOptions options = default)
        {
            if (bool.TryParse(input, out output))
                return true;
            if (options.Text.FirstChar(input, out var c))
                return TryConvert(c, out output, options);
            output = default;
            return false;
        }

        public bool TryConvert(ReadOnlySpan<char> input, out bool output, ConvertOptions options = default)
        {
            if (bool.TryParse(input, out output))
                return true;
            if (options.Text.FirstChar(input, out var c))
                return TryConvert(c, out output, options);
            output = default;
            return false;
        }

        public bool TryConvert(char input, out byte output, ConvertOptions options = default)
        {
            if (byte.TryParse(input.ToReadOnlySpan(), options.Text.NumberStyle, options.FormatProvider, out output))
                return true;
            if (options.Text.CharToNumber)
            {
                output = (byte)input;
                return true;
            }
            output = default;
            return false;
        }

        public bool TryConvert(string? input, out byte output, ConvertOptions options = default)
        {
            if (byte.TryParse(input, options.Text.NumberStyle, options.FormatProvider, out output))
                return true;
            if (input?.Length == 1 && options.Text.CharToNumber)
            {
                output = (byte)input[0];
                return true;
            }
            output = default;
            return false;
        }

        public bool TryConvert(ReadOnlySpan<char> input, out byte output, ConvertOptions options = default)
        {
            if (byte.TryParse(input, options.Text.NumberStyle, options.FormatProvider, out output))
                return true;
            if (input.Length == 1 && options.Text.CharToNumber)
            {
                output = (byte)input[0];
                return true;
            }
            output = default;
            return false;
        }
    }
}
*/
