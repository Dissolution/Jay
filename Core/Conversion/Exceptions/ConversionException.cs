namespace Jay.Conversion;

public abstract class ConversionException : Exception
{
    protected static string GetBaseMessage(Type? inputType, Type? outputType, string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return FormatterCache.AssemblyCache.Format($"Unable to convert {inputType} to {outputType}");
        }
        else
        {
            return FormatterCache.AssemblyCache.Format($"Unable to convert {inputType} to {outputType}: {message}");
        }
    }


    public Type? InputType { get; }
    public Type? OutputType { get; }

    protected ConversionException(Type? inputType,
                                  Type? outputType,
                                  string? message = null,
                                  Exception? innerException = null)
        : base(GetBaseMessage(inputType, outputType, message), innerException)
    {
        this.InputType = inputType;
        this.OutputType = outputType;
    }
}