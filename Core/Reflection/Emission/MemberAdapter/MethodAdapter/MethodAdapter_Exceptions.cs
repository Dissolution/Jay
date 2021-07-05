using System;
using System.Reflection;
using Jay.Text;

namespace Jay.Reflection.Emission
{
    public static partial class MethodAdapter
    {
        private static ConversionException GetConversionException(ArgumentType inputParameter,
                                                                  ArgumentType outputParameter,
                                                                  string? message = null,
                                                                  Exception? innerException = null)
        {
            using (var builder = TextBuilder.Rent())
            {
                builder.Append("Cannot convert from ")
                       .Append(inputParameter)
                       .Append(" to ")
                       .Append(outputParameter);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    builder.Append(": ")
                           .Append(message);
                }
                
                return new ConversionException(builder.ToString(), innerException);
            }
        }
        
        private static ConversionSafetyException GetConversionSafetyException(Safety neededSafety,
                                                                              ArgumentType inputParameter,
                                                                              ArgumentType outputParameter,
                                                                              string? message = null,
                                                                              Exception? innerException = null)
        {
            using (var builder = TextBuilder.Rent())
            {
                builder.Append("Conversion from ")
                       .Append(inputParameter)
                       .Append(" to ")
                       .Append(outputParameter)
                       .Append(" requires ")
                       .Append(neededSafety)
                       .Append(" Conversion Safety");
                if (!string.IsNullOrWhiteSpace(message))
                {
                    builder.Append(": ")
                           .Append(message);
                }
                
                return new ConversionSafetyException(builder.ToString(), innerException);
            }
        }

        private static ConversionException GetConversionException(MethodBase delegateMethod,
                                                                  MethodBase targetMethod,
                                                                  string? message = null,
                                                                  Exception? innerException = null)
        {
            using (var builder = TextBuilder.Rent())
            {
                builder.Append("Cannot adapt ")
                       .Append(delegateMethod)
                       .Append(" to ")
                       .Append(targetMethod);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    builder.Append(": ")
                           .Append(message);
                }
                
                return new ConversionException(builder.ToString(), innerException);
            }
        }        
    }
}