using System;
using System.Diagnostics.CodeAnalysis;
using Jay.Debugging.Dumping;
using Jay.Text;

namespace Jay.Conversion
{
    public class ConversionException : Exception
    {
        private static string GetMessage(Type inType, Type outType)
        {
            return TextBuilder.Build(text => text.Append("Cannot convert from a ")
                                                 .AppendDump(inType)
                                                 .Append(" to a ")
                                                 .AppendDump(outType)
                                                 .Append(" value"));
        }

        public static ConversionException Create<TIn, TOut>([AllowNull] TIn input,
                                                            Exception? innerException = default)
        {
            var message = TextBuilder.Build(text => text.Append("Cannot convert '")
                                                        .AppendDump(typeof(TIn))
                                                        .Append("' \"")
                                                        .Append(input)
                                                        .Append("\" to a '")
                                                        .AppendDump(typeof(TOut))
                                                        .Append("' value"));
            return new ConversionException(message, innerException);
        }
        
        public static ConversionException Create<TIn, TOut>([AllowNull] TIn input,
                                                            ConvertOptions options,
                                                            Exception? innerException = default)
        {
            var message = TextBuilder.Build(text => text.Append("Cannot convert '")
                                                        .AppendDump(typeof(TIn))
                                                        .Append("' \"")
                                                        .Append(input)
                                                        .Append("\" to a '")
                                                        .AppendDump(typeof(TOut))
                                                        .Append("' value with '")
                                                        .Append(options)
                                                        .Append("' options"));
            return new ConversionException(message, innerException);
        }
        
        public static ConversionException Create<TOut>(ReadOnlySpan<char> text,
                                                       ConvertOptions options,
                                                       Exception? innerException = default)
        {
            var message = TextBuilder.Build(text, (textBuilder, txt) => textBuilder.Append("Cannot convert '")
                                                .AppendDump(typeof(ReadOnlySpan<char>))
                                                .Append("' \"")
                                                .Append(txt)
                                                .Append("\" to a '")
                                                .AppendDump(typeof(TOut))
                                                .Append("' value with '")
                                                .Append(options)
                                                .Append("' options"));
            return new ConversionException(message, innerException);
        }
        
        /// <inheritdoc />
        public ConversionException(string? message = null, 
                                   Exception? innerException = null) 
            : base(message, innerException)
        {
        }

        public ConversionException(Type inType, Type outType,
                                   Exception? innerException = null)
            : base(GetMessage(inType, outType), innerException)
        {
            
        }
    }
}