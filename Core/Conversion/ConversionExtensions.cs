/*using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Conversion
{
    public static class ConversionExtensions
    {
        [return: NotNull]
        public static TOut ConvertTo<TOut>(this ReadOnlySpan<char> input)
        {
            TryConvertTo<TOut>(input, out TOut output).ThrowIfFailed();
            return output;
        }

        public static Result TryConvertTo<TOut>(this ReadOnlySpan<char> input,
                                                [NotNull] out TOut output)
        {
            throw new NotImplementedException();
        }
        
        [return: NotNullIfNotNull("input")]
        public static TOut ConvertTo<TOut>([AllowNull] this string input)
        {
            TryConvertTo<TOut>(input, out TOut output).ThrowIfFailed();
            return output;
        }

        public static Result TryConvertTo<TOut>([AllowNull] this string input,
                                                [NotNullIfNotNull("input")] out TOut output)
        {
            throw new NotImplementedException();
        }
        
        [return: NotNullIfNotNull("input")]
        public static TOut ConvertTo<TOut>([AllowNull] this object input)
        {
            TryConvertTo<TOut>(input, out TOut output).ThrowIfFailed();
            return output;
        }

        public static Result TryConvertTo<TOut>([AllowNull] this object input,
                                                [NotNullIfNotNull("input")] out TOut output)
        {
            throw new NotImplementedException();
        }
        
        
        public static ReadOnlySpan<char> ConvertTo<TIn>([AllowNull] this TIn input)
        {
            TryConvertTo<TIn>(input, out ReadOnlySpan<char> output).ThrowIfFailed();
            return output;
        }

        public static Result TryConvertTo<TIn>([AllowNull] this TIn input,
                                                out ReadOnlySpan<char> output)
        {
            throw new NotImplementedException();
        }
        
        
        [return: NotNullIfNotNull("input")]
        public static TOut ConvertTo<TIn, TOut>([AllowNull] this TIn input)
        {
            TryConvertTo<TIn, TOut>(input, out TOut output).ThrowIfFailed();
            return output;
        }

        public static Result TryConvertTo<TIn, TOut>([AllowNull] this TIn input,
                                                     [NotNullIfNotNull("input")] out TOut output)
        {
            throw new NotImplementedException();
        }
    }
}*/