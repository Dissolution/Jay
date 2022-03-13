using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Dumping;
using Jay.Exceptions;
using Jay.Text;

namespace ConsoleSandbox;

public abstract class ConversionException : Exception
{
    protected static string GetBaseMessage(Type? inputType, Type? outputType, string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return Formatter.Format($"Unable to convert {inputType} to {outputType}");
        }
        else
        {
            return Formatter.Format($"Unable to convert {inputType} to {outputType}: {message}");
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

public class CastException : ConversionException
{
    public CastException(Type? inputType, Type? outputType, string? message = null, Exception? innerException = null) 
        : base(inputType, outputType, message, innerException)
    {
        
    }
}

public class FormatException : ConversionException
{
    public FormatException(Type? inputType, string? message = null, Exception? innerException = null) 
        : base(inputType, typeof(string), message, innerException)
    {
        
    }
}

public readonly struct CastOptions
{
    
}

public interface ICaster
{
    bool CanCastFrom(Type inType);
    bool CanCastTo(Type outType);

    Result TryCast(object? input, [NotNullWhen(true)] out object? output, CastOptions options = default);
}

public interface ICaster<TIn, TOut> : ICaster
{
    bool ICaster.CanCastFrom(Type inType) => inType.IsAssignableTo(typeof(TIn));
    bool ICaster.CanCastTo(Type outType) => outType.IsAssignableTo(typeof(TOut));

    Result ICaster.TryCast(object? input, [NotNullWhen(true)] out object? output, CastOptions options)
    {
        if (input is TIn)
        {
            Result result = TryCast((TIn)input, out TOut? outValue, options);
            if (!result)
            {
                output = null;
                return result;
            }
            else
            {
                output = outValue!;
                return true;
            }
        }
        else
        {
            output = null;
            return new CastException(input?.GetType(), typeof(TOut));
        }
    }

    Result TryCast(TIn? input, [NotNullWhen(true)] out TOut? output, CastOptions options = default);
}

public readonly struct ParseOptions
{
    
}

public interface IParser
{
    Type OutputType { get; }

    Result TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out object? obj, ParseOptions options = default);

    Result TryParse(string? text, [NotNullWhen(true)] out object? obj, ParseOptions options = default)
        => TryParse(text.AsSpan(), out obj, options);
}

public interface IParser<TOut> : IParser
{
    Type IParser.OutputType => typeof(TOut);

    Result IParser.TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out object? obj, ParseOptions options)
    {
        Result result = TryParse(text, out TOut? value, options);
        if (!result)
        {
            obj = null;
            return result;
        }
        else
        {
            obj = value!;
            return true;
        }
    }

    Result TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out TOut? value, ParseOptions options = default);

    Result TryParse(string? text, [NotNullWhen(true)] out TOut? value, ParseOptions options = default)
        => TryParse(text.AsSpan(), out value, options);
}

public readonly struct FormatOptions
{
    public readonly string? Format = null;
    public readonly IFormatProvider? FormatProvider = null;
}

public interface IFormatter
{
    bool CanFormatFrom(Type inputType);

    Result TryFormat(object? input, out string text, FormatOptions options = default);

    Result TryFormat(object? input, Span<char> destination, out int charsWritten, FormatOptions options = default);
   
}

public abstract class TypeCache<TValue>
{
    protected readonly List<TValue> _items;
    protected readonly ConcurrentTypeDictionary<TValue?> _cache;

    public abstract TValue DefaultValue { get; }
    
    protected TypeCache()
    {
        _items = new(0);
        _cache = new(0);
    }

    protected abstract bool Fulfills(TValue value, Type type);
    
    private TValue? FindValue(Type type)
    {
        foreach (TValue item in _items)
        {
            if (Fulfills(item, type))
            {
                return item;
            }
        }
        return default;
    }

    public virtual TypeCache<TValue> Add(TValue value)
    {
        _items.Add(value);
        return this;
    }

    public TypeCache<TValue> Add(params TValue[] values)
    {
        _items.AddRange(values);
        return this;
    }
    
    public TypeCache<TValue> Add(IEnumerable<TValue> values)
    {
        _items.AddRange(values);
        return this;
    }

    public virtual TValue Lookup(Type type)
    {
        return _cache.GetOrAdd(type, FindValue) ?? DefaultValue;
    }
    
    public TValue Lookup<T>() => Lookup(typeof(T));
}

public sealed class FuncTypeCache<TValue> : TypeCache<TValue>
{
    private readonly Func<TValue, Type, bool> _fulfills;
    
    public override TValue DefaultValue { get; }

    public FuncTypeCache(Func<TValue> createDefaultValue, Func<TValue, Type, bool> fulfills)
    {
        _fulfills = fulfills;
        this.DefaultValue = createDefaultValue();
    }

    protected override bool Fulfills(TValue value, Type type) => _fulfills(value, type);
}

public interface IFormatterCache
{
    
}


public static class Formatter
{
    internal sealed class DefaultFormatter : IFormatter
    {
        public bool CanFormatFrom(Type inputType) => true;

        public Result TryFormat(object? input, out string text, FormatOptions options = default)
        {
            
            if (input is IFormattable)
            {
                text = ((IFormattable)input).ToString(options.Format, options.FormatProvider);
                return true;
            }
            text = input?.ToString() ?? "";
            return true;
        }

        public Result TryFormat(object? input, Span<char> destination, out int charsWritten, FormatOptions options = default)
        {
            if (input is IFormattable)
            {
                if (input is ISpanFormattable)
                {
                    bool formatted = ((ISpanFormattable)input).TryFormat(destination, out charsWritten, options.Format, options.FormatProvider);
                    if (formatted) return Result.Pass;
                }

                string text = ((IFormattable)input).ToString(options.Format, options.FormatProvider);
                charsWritten = text.Length;
                return text.TryCopyTo(destination);
            }
            else
            {
                ReadOnlySpan<char> text = input?.ToString();
                charsWritten = text.Length;
                return text.TryCopyTo(destination);
            }
        }
    }
    
    
    private static readonly FuncTypeCache<IFormatter> _cache;

    internal static IFormatter Default { get; } = new DefaultFormatter();
    
    static Formatter()
    {
        _cache = new FuncTypeCache<IFormatter>(() => Default, Fulfills);
    }

    private static bool Fulfills(IFormatter formatter, Type type) => formatter.CanFormatFrom(type);

    public static string Format(/*[InterpolatedStringHandlerArgument("")] */FormatterStringHandler stringHandler)
    {
        throw new NotImplementedException();
    }

    public static Result TryFormat<TIn>(TIn? input, Span<char> destination, out int charsWritten, FormatOptions options = default)
    {
        return _cache.Lookup(typeof(TIn)).TryFormat(input, destination, out charsWritten, options);
    }
}



[InterpolatedStringHandler]
public ref struct FormatterStringHandler
{
    //private readonly IFormatter _formatter;
    private readonly TextBuilder _text;

    public FormatterStringHandler(int literalLength, int formatCount//,IFormatter formatter)
    )
    {
        //_formatter = formatter;
        _text = new TextBuilder();
    }

    public void AppendLiteral(string? str)
    {
        _text.Write(str);
    }

    public void AppendFormatted<T>(T? value)
    {
        _text.AppendDump<T>(value);
    }

    public void AppendFormatted<T>(T? value, string? format)
    {
        _text.AppendDump<T>(value, new DumpOptions(false, format));
    }

    public string ToStringAndClear()
    {
        var str = _text.ToString();
        _text.Dispose();
        return str;
    }

    public override bool Equals(object? obj)
    {
        return UnsuitableException.ThrowEquals(typeof(DumpStringHandler));
    }

    public override int GetHashCode()
    {
        return UnsuitableException.ThrowGetHashCode(typeof(DumpStringHandler));
    }

    public override string ToString()
    {
        throw new InvalidOperationException($"You MUST call {nameof(ToStringAndClear)}() in order to get the string output of resolving the {nameof(DumpStringHandler)}");
    }
}