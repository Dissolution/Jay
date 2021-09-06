/*using System.Diagnostics.CodeAnalysis;

namespace Jay.Conversion
{
    public readonly struct ConversionOptions
    {
        public static bool operator ==(ConversionOptions x, ConversionOptions y) => x.Equals(y);
        public static bool operator !=(ConversionOptions x, ConversionOptions y) => !x.Equals(y);
        
        public static readonly ConversionOptions Default = new ConversionOptions();
    }

    public static partial class Converter
    {
        public static Result TryConvert<TIn, TOut>([AllowNull] TIn input,
                                                   ConversionOptions options,
                                                   [NotNullIfNotNull("input")] out TOut output)
        {
            if (input is null)
            {
                output = default(TOut)!;
                return typeof(TOut).CanBeNull();
            }

            if (typeof(TOut) == typeof(string))
            {
                output = input
            }
        }
    }
}*/