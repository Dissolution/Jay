using System;
using System.Diagnostics.CodeAnalysis;
using Jay.Text;

namespace Jay.Conversion.New
{
    /// <summary>
    /// An exception related to a failed conversion
    /// </summary>
    public class ConversionException : Exception
    {
        public static ConversionException Create<TIn, TOut>(Exception? innerException = null)
            => new ConversionException(null, typeof(TIn), typeof(TOut), null, innerException);
        public static ConversionException Create<TIn, TOut>(string? message, Exception? innerException = null)
            => new ConversionException(null, typeof(TIn), typeof(TOut), message, innerException);
        
        public static ConversionException Create<TIn, TOut>([AllowNull] TIn input, Exception? innerException = null)
            => new ConversionException(input, typeof(TIn), typeof(TOut), null, innerException);
        public static ConversionException Create<TIn, TOut>([AllowNull] TIn input, string? message, Exception? innerException = null)
            => new ConversionException(input, typeof(TIn), typeof(TOut), message, innerException);
        
        public static ConversionException Create<TOut>(object? input, Exception? innerException = null)
            => new ConversionException(input, input?.GetType(), typeof(TOut), null, innerException);
        public static ConversionException Create<TOut>(object? input, string? message, Exception? innerException = null)
            => new ConversionException(input, input?.GetType(), typeof(TOut), message, innerException);
        
        public static ConversionException Create(Type? inputType, Type? outputType, Exception? innerException = null)
            => new ConversionException(null, inputType, outputType, null, innerException);
        public static ConversionException Create(Type? inputType, Type? outputType, string? message, Exception? innerException = null)
            => new ConversionException(null, inputType, outputType, message, innerException);
        
        public static ConversionException Create(object? input, Type? outputType, Exception? innerException = null)
            => new ConversionException(input, input?.GetType(), outputType, null, innerException);
        public static ConversionException Create(object? input, Type? outputType, string? message, Exception? innerException = null)
            => new ConversionException(input, input?.GetType(), outputType, message, innerException);
        
        public static ConversionException Create(object? input, Type? inputType, Type? outputType, Exception? innerException = null)
            => new ConversionException(input, inputType ?? input?.GetType(), outputType, null, innerException);
        public static ConversionException Create(object? input, Type? inputType, Type? outputType, string? message, Exception? innerException = null)
            => new ConversionException(input, inputType ?? input?.GetType(), outputType, message, innerException);

        private static string GetMessage(object? input, Type? inputType, Type? outputType, string? message)
        {
            return TextBuilder.Build(text =>
            {
                text.Append("Could not convert ")
                    .If(inputType is null, 
                        t => t.Append("null"), 
                        t => t.AppendDumpType(inputType)
                              .Append(" '")
                              .AppendDumpValue(input)
                              .Append("'"))
                    .Append(" to ")
                    .AppendDumpType(outputType)
                    .If(!string.IsNullOrWhiteSpace(message), 
                        t => t.Append(": ")
                              .Append(message));
            });
        }

        public object? Input { get; }
        public Type? InputType { get; }
        public Type? OutputType { get; }
        
        private ConversionException(object? input,
                                    Type? inputType,
                                    Type? outputType,
                                    string? message,
                                    Exception? innerException)
            : base(GetMessage(input, inputType, outputType, message), innerException)
        {
            this.Input = input;
            this.InputType = inputType;
            this.OutputType = outputType;
        }
    }
}