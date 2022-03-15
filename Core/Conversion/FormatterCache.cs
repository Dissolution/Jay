
using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Search;

namespace Jay.Conversion;

public sealed class FormatterCache : IFormatter
{
    private sealed class DefaultFormatter : IFormatter
    {
        public bool CanFormat(Type inputType) => true;

        public string Format(object? input, FormatOptions options = default)
        {
            if (input is IFormattable)
            {
                return ((IFormattable)input).ToString(options.Format, options.Provider);
            }
            return input?.ToString() ?? "";
        }

        public Result TryFormat(object? input, Span<char> destination, out int charsWritten, FormatOptions options = default)
        {
            if (input is IFormattable)
            {
                if (input is ISpanFormattable)
                {
                    bool formatted = ((ISpanFormattable)input).TryFormat(destination, out charsWritten, options.Format, options.Provider);
                    if (formatted) return Result.Pass;
                }

                string text = ((IFormattable)input).ToString(options.Format, options.Provider);
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

    private static readonly IFormatter _defaultFormatter = new DefaultFormatter();
    private static readonly Lazy<FormatterCache> _assemblyFormatterCache = new Lazy<FormatterCache>(CreateAssemblyFormatterCache);

    public static FormatterCache AssemblyCache => _assemblyFormatterCache.Value;

    private static FormatterCache CreateAssemblyFormatterCache()
    {
        return new FormatterCache(AppDomain.CurrentDomain
                                           .GetAssemblies()
                                           .SelectMany(assembly =>
                                           {
                                               try
                                               {
                                                   return assembly.ExportedTypes;
                                               }
                                               catch
                                               {
                                                   return Type.EmptyTypes;
                                               }
                                           })
                                           .SelectWhere((Type type, out IFormatter? formatter) =>
                                           {
                                               if (type.Implements<IFormatter>() &&
                                                   type.IsClass && !type.IsAbstract && !type.IsInterface && !type.IsNested &&
                                                   type.HasDefaultConstructor())
                                               {
                                                   try
                                                   {
                                                       formatter = Activator.CreateInstance(type) as IFormatter;
                                                   }
                                                   catch
                                                   {
                                                       formatter = default;
                                                   }
                                               }
                                               else
                                               {
                                                   formatter = default;
                                               }

                                               return formatter is not null;
                                           })!);
    }

    private readonly List<IFormatter> _formatters;
    private readonly ConcurrentTypeDictionary<IFormatter?> _cache;

    public FormatterCache()
    {
        _formatters = new();
        _cache = new();
    }

    public FormatterCache(IEnumerable<IFormatter> formatters)
    {
        _formatters = new List<IFormatter>(formatters);
        _cache = new(_formatters.Count);
    }

    private IFormatter? FindFormatter(Type type)
    {
        return _formatters.FirstOrDefault(f => f.CanFormat(type));
    }

    public FormatterCache AddFormatter(IFormatter formatter)
    {
        _formatters.Add(formatter);
        return this;
    }

    public FormatterCache AddFormatters(params IFormatter[] formatters)
    {
        _formatters.AddRange(formatters);
        return this;
    }
    
    public FormatterCache AddFormatters(IEnumerable<IFormatter> formatters)
    {
        _formatters.AddRange(formatters);
        return this;
    }
    
    public IFormatter GetFormatter(Type type)
    {
        return _cache.GetOrAdd(type, FindFormatter) ?? _defaultFormatter;
    }
   
    public IFormatter GetFormatter<T>() => GetFormatter(typeof(T));
    
    public bool CanFormat(Type inputType)
    {
        return true;
    }

    string IFormatter.Format(object? input, FormatOptions options)
    {
        if (input is null)
        {
            return "";
        }
        return GetFormatter(input.GetType()).Format(input, options);
    }

    Result IFormatter.TryFormat(object? input, Span<char> destination, out int charsWritten, FormatOptions options)
    {
        if (input is null)
        {
            charsWritten = 0;
            return true;
        }
        return GetFormatter(input.GetType()).TryFormat(input, destination, out charsWritten, options);
    }

    public string Format<T>(T? input, FormatOptions options = default)
    {
        if (input is null) return "";
        var formatter = GetFormatter<T>();
        if (formatter is IFormatter<T> typedFormatter)
            return typedFormatter.Format(input, options);
        return formatter.Format(input, options);
    }
    
    public Result TryFormat<T>(T? input, Span<char> destination, out int charsWritten, FormatOptions options = default)
    {
        if (input is null)
        {
            charsWritten = 0;
            return true;
        }
        var formatter = GetFormatter<T>();
        if (formatter is IFormatter<T> typedFormatter)
            return typedFormatter.TryFormat(input, destination, out charsWritten, options);
        return formatter.TryFormat(input, destination, out charsWritten, options);
    }

    public string Format([InterpolatedStringHandlerArgument("")] ref FormatterStringHandler stringHandler)
    {
        return stringHandler.ToStringAndClear();
    }
    
    public string Format([InterpolatedStringHandlerArgument("", "options")] ref FormatterStringHandler stringHandler, FormatOptions options)
    {
        return stringHandler.ToStringAndClear();
    }
    
}