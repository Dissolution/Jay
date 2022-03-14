using System.Runtime.CompilerServices;
using Jay;
using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Search;

namespace ConsoleSandbox.Conversion;

public sealed class FormatterCache : IFormatter
{
    private sealed class DefaultFormatter : IFormatter
    {
        public bool CanFormatFrom(Type inputType) => true;

        public Result TryFormat(object? input, out string text, FormatOptions options = default)
        {
            if (input is IFormattable)
            {
                text = ((IFormattable)input).ToString(options.Format, options.Provider);
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
                                           .SelectMany(assembly => Result.Swallow(() => assembly.ExportedTypes, Type.EmptyTypes))
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
        return _formatters.FirstOrDefault(f => f.CanFormatFrom(type));
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
    
    public bool CanFormatFrom(Type inputType)
    {
        return true;
    }

    public Result TryFormat(object? input, out string text, FormatOptions options = default)
    {
        if (input is null)
        {
            text = string.Empty;
            return true;
        }
        return GetFormatter(input.GetType()).TryFormat(input, out text, options);
    }

    public Result TryFormat(object? input, Span<char> destination, out int charsWritten, FormatOptions options = default)
    {
        if (input is null)
        {
            charsWritten = 0;
            return true;
        }
        return GetFormatter(input.GetType()).TryFormat(input, destination, out charsWritten, options);
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