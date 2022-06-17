using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Extensions;
using Jay.Reflection.Search;
using Jay.Text;
using Jay.Validation;

namespace Jay.Dumping;

internal class DefaultDumper : IObjectDumper
{
    public static DefaultDumper Instance { get; } = new DefaultDumper();
    
    /// <inheritdoc />
    public bool CanDump(Type objType) => true;

    /// <inheritdoc />
    public void DumpObject(TextBuilder text, object? obj, DumpOptions options = default)
    {
        if (Dumper.DumpNull(text, obj, options)) return;
        text.WriteFormatted(obj, options.Format);
    }
}

internal sealed class DefaultDumper<T> : DefaultDumper, IValueDumper<T>
{
    new public static DefaultDumper<T> Instance { get; } = new DefaultDumper<T>();

    /// <inheritdoc />
    public void DumpValue(TextBuilder text, T? value, DumpOptions options = default)
    {
        if (Dumper.DumpNull(text, value, options)) return;
        text.WriteFormatted(value, options.Format);
    }
}

internal sealed class ObjectDumper : IValueDumper<object>, IObjectDumper
{
    /// <inheritdoc />
    public bool CanDump(Type objType)
    {
        return objType == typeof(object);
    }

    /// <inheritdoc />
    public void DumpObject(TextBuilder text, object? obj, DumpOptions options = default)
    {
        if (Dumper.DumpNull(text, obj, options)) return;
        var valueType = obj.GetType();
        if (valueType == typeof(object))
        {
            text.Write("(object)");
        }
        else
        {
            var dumper = Dump.GetDumper(valueType);
            dumper.DumpObject(text, obj, options);
        }
    }

    /// <inheritdoc />
    public void DumpValue(TextBuilder text, object? value, DumpOptions options = default)
    {
        DumpObject(text, value, options);
    }
}

/// <summary>
/// Dumper Cache
/// </summary>
public static partial class Dump
{
    private static readonly List<IObjectDumper> _dumpers;
    private static readonly ConcurrentTypeDictionary<IObjectDumper?> _dumperMap;

    static Dump()
    {
        var dumperTypes = AppDomain.CurrentDomain
                                   .GetAssemblies()
                                   .Where(assembly => !assembly.IsDynamic)
                                   .SelectMany(assembly =>
                                   {
                                       return Result.Result.InvokeOrDefault(() => assembly.ExportedTypes, Type.EmptyTypes);
                                   })
                                   .Where(type => type.Implements(typeof(Dumper<>)))
                                   .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && !type.IsNested)
                                   .SelectWhere((Type type, out IObjectDumper dumper) =>
                                   {
                                       return Result.Result.TryInvoke(() => Activator.CreateInstance(type) as IObjectDumper!, out dumper!);
                                   });
        _dumpers = new List<IObjectDumper>(dumperTypes)
        {
            new Format<TimeSpan>("G", "g"),
            new Format<DateTime>("O", "yyyy-MM-dd HH:mm:ss.f"),
            new Format<DateTimeOffset>("O", "yyyy-MM-dd HH:mm:ss.f zzz"),
            new Format<Guid>("D", "D"),
            new Format<decimal>("N", "N"),
        };
        //Debug.Assert(_dumpers.Any(d => d is ObjectDumper));
        _dumperMap = new(_dumpers.Count);
    }

    private static IObjectDumper? GetObjectDumper(Type type)
    {
        return _dumperMap.AddOrUpdate(type, 
            t => _dumpers.FirstOrDefault(d => d.CanDump(t)), 
            (t, objDumper) => objDumper ?? _dumpers.FirstOrDefault(d => d.CanDump(t)));
    }
    
    internal static IValueDumper<T> GetDumper<T>()
    {
        var dumper = GetObjectDumper(typeof(T));
        
        if (dumper is IValueDumper<T> valueDumper)
        {
            return valueDumper;
        }

        return DefaultDumper<T>.Instance;
    }

    internal static IObjectDumper GetDumper(Type? type)
    {
        if (type is null) return DefaultDumper.Instance;
        
        var dumper = GetObjectDumper(type);
        
        if (dumper is not null)
        {
            return dumper;
        }
        
        return DefaultDumper.Instance;
    }
}

/// <summary>
/// Extensions
/// </summary>
public static partial class Dump
{
    public static TextBuilder AppendDump<T>(this TextBuilder text, 
                                            T? value, 
                                            DumpOptions options = default)
    {
        var dumper = GetDumper<T>();
        dumper.DumpValue(text, value, options);
        return text;
    }
}

public static partial class Dump
{
    public static string Text(ref DumpStringHandler dumpFormattedString)
    {
        return dumpFormattedString.ToStringAndClear();
    }

    public static string Value<T>(T? value, DumpOptions options = default)
    {
        using var text = TextBuilder.Borrow();
        var dumper = GetDumper<T>();
        dumper.DumpValue(text, value, options);
        return text.ToString();
    }
}

public static partial class Dump
{
    internal static TException GetException<TException>(ref DumpStringHandler message, Exception? innerException = null)
        where TException : Exception
    {
        var ctor = ExceptionBuilder.GetCommonConstructor<TException>();
        var ex = ctor(message.ToStringAndClear(), innerException);
        return ex;
    }

    [DoesNotReturn]
    public static void ThrowException<TException>(ref DumpStringHandler message, Exception? innerException = null)
        where TException : Exception
    {
        throw GetException<TException>(ref message, innerException);
    }

    internal static TException GetException<TException>(ref DumpStringHandler message, params object?[] args)
        where TException : Exception
    {
        var argTypes = new Type?[args.Length + 1];
        argTypes[0] = typeof(string);
        for (var i = 0; i < args.Length; i++)
        {
            argTypes[i + 1] = args[0]?.GetType();
        }

        var ctor = MemberSearch.FindBestConstructor(typeof(TException),
                                   Reflect.InstanceFlags,
                                   MemberSearch.MemberExactness.CanIgnoreInputArgs,
                                   argTypes)
                               .ThrowIfNull();
        var ex = ctor.Construct<TException>(args);
        return ex;
    }

    [DoesNotReturn]
    public static void ThrowException<TException>(ref DumpStringHandler message, params object?[] args)
        where TException : Exception
    {
        throw GetException<TException>(ref message, args);
    }
}