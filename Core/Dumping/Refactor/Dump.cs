/*using System.Diagnostics;
using System.Reflection;
using Jay.Collections;
using Jay.Reflection.Building;
using Jay.Text;

namespace Jay.Dumping.Refactor;

public static class Dumper
{
    private static readonly ConcurrentTypeDictionary<Delegate> _dumpValueCache;

    static Dumper()
    {
        _dumpValueCache = new()
        {
            [typeof(Type)] = (DumpValue<Type>)TypeDumper.Dump,
            [typeof(IEnumerable)] = (DumpValue<IEnumerable>)EnumerableDumper.Dump,
        };
    }

    public Delegate GetDumpDelegate(Type? type)
    {
        if (type is null) return CreateDumpConstValue<object, string>("(Type)null");
        var del = _dumpValueCache.GetOrAdd(type, t => CreateDumpValueDelegate(t));
        return del;
    }
    
    public DumpValue<T> GetDumpDelegate<T>()
    {
        return _dumpValueCache.Get
    }


    private static Delegate CreateDumpValueDelegate(Type type)
    {
        // Check for and use [DumpAs]
        var attr = type.GetCustomAttribute<DumpAsAttribute>();
        if (attr is not null && attr.DumpString is not null)
        {
            return CreateDumpConstValue<T, string>(attr.DumpString);
        }
       
        // Check for IDumpable
        if (type.Implements<IDumpable>())
        {
            return RuntimeBuilder.CreateDelegate<DumpValue<T>>($"Dump_{type.FullName}",
                dynamicMethod =>
                {
                    var emitter = dynamicMethod.Emitter;
                    var dumpMethod = type.GetMethod(nameof(IDumpable.DumpTo),
                        BindingFlags.Public | BindingFlags.Instance,
                        new Type[] { typeof(TextBuilder), typeof(DumpOptions) });
                    Debug.Assert(dumpMethod is not null);
                    emitter.Ldarg_0()
                        .Ldarg_1()
                        .Ldarg_2()
                        .Call(dumpMethod)
                        .Ret();
                });
        }

        // Trust TextBuilder
        return DefaultDumpValue;
    }
    
    private static DumpValue<T> CreateDumpValueDelegate<T>()
    {
        var type = typeof(T);

        // Check for and use [DumpAs]
        var attr = type.GetCustomAttribute<DumpAsAttribute>();
        if (attr is not null && attr.DumpString is not null)
        {
            return CreateDumpConstValue<T, string>(attr.DumpString);
        }
       
        // Check for IDumpable
        if (type.Implements<IDumpable>())
        {
            return RuntimeBuilder.CreateDelegate<DumpValue<T>>($"Dump_{type.FullName}",
                dynamicMethod =>
                {
                    var emitter = dynamicMethod.Emitter;
                    var dumpMethod = type.GetMethod(nameof(IDumpable.DumpTo),
                        BindingFlags.Public | BindingFlags.Instance,
                        new Type[] { typeof(TextBuilder), typeof(DumpOptions) });
                    Debug.Assert(dumpMethod is not null);
                    emitter.Ldarg_0()
                        .Ldarg_1()
                        .Ldarg_2()
                        .Call(dumpMethod)
                        .Ret();
                });
        }

        // Trust TextBuilder
        return DefaultDumpValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DefaultDumpValue<T>(T? value, TextBuilder text, DumpOptions? options)
    {
        text.WriteFormatted(value, options?.Format, options?.FormatProvider);
    }

    private static DumpValue<T> GetOrCreateDumpDelegate<T>()
    {
        Delegate? del;
        Delegate CreateValue(Type _)
        {
            del = CreateDumpValueDelegate<T>();
            return del;
        }

        del = _dumpValueCache.GetOrAdd<T>(CreateValue);
        if (del is DumpValue<T> dumpValue)
            return dumpValue;
        
        throw new NotImplementedException();
    }

    internal static DumpValue<TDump> CreateDumpConstValue<TDump, TConst>(TConst? constValue)
    {
        return (TDump? _, TextBuilder text, DumpOptions? _) =>
        {
            text.Write<TConst>(constValue);
        };
    }

    public static void AddOrUpdateDumper<T>(DumpValue<T> dumpDelegate)
    {
        _dumpValueCache[typeof(T)] = (DumpValue<T>)dumpDelegate;
    }

    public static bool TryGetDumper<T>([NotNullWhen(true)] out DumpValue<T>? dumpDelegate)
    {
        if (_dumpValueCache.TryGetValue<T>(out var del))
        {
            return del.Is(out dumpDelegate);
        }

        dumpDelegate = null;
        return false;
    }

    public static void Dump<T>(T? value, TextBuilder text, DumpOptions? options = default)
    {
        if (options is null)
            options = DumpOptions.Default;
        var dumpDelegate = GetOrCreateDumpDelegate<T>();
        dumpDelegate(value, text, options);
    }

    public static string Dump<T>(this T? value, DumpOptions? options = default)
    {
        using var text = TextBuilder.Borrow();
        Dump<T>(value, text, options);
        return text.ToString();
    }

    public static TextBuilder AppendDump(this TextBuilder textBuilder, object? value, DumpOptions? options = default)
    {
        Dump<T>(value, textBuilder, options);
        return textBuilder;
    }
    
    public static TextBuilder AppendDump<T>(this TextBuilder textBuilder, T? value, DumpOptions? options = default)
    {
        Dump<T>(value, textBuilder, options);
        return textBuilder;
    }

    public static TextBuilder AppendDump<T>(this TextBuilder textBuilder,
        [InterpolatedStringHandlerArgument("textBuilder")]
        ref InterpolatedDumpHandler dumpString)
    {
        // dumpString evaluation will write to textbuilder
        Debugger.Break();
        return textBuilder;
    }

    public static string Dump(ref InterpolatedDumpHandler dumpString)
    {
        return dumpString.ToStringAndDispose();
    }
}

public abstract class ValueDumper
{
    protected static bool DumpNull<T>([NotNullWhen(false)] T? value, 
        TextBuilder text,
        DumpOptions? options)
    {
        if (value is not null) return false;
        if (options?.Verbose == true)
        {
            text.Append('(')
                .AppendDump(typeof(T))
                .Append(')');
        }
        text.Write("null");
        return true;
    }
}


public abstract class ValueDumper<T> : ValueDumper
{
    protected static DumpValue<T> DumpConst<TConst>(TConst? constValue)
    {
        return (T? value, TextBuilder builder, DumpOptions? options) =>
        {
            builder.WriteFormatted<TConst>(constValue, options?.Format, options?.FormatProvider);
        };
    }

}

public interface IDumper<T>
{
    static abstract void Dump(T? value, TextBuilder textBuilder, DumpOptions? options = default);

   
   
}


/*
public static partial class DumpCache
{
    private static void DumpEnum<TEnum>(TEnum @enum, ref Dumper dumper)
        where TEnum : struct, Enum
    {
        var enumInfo = @enum.GetInfo();
        var dumpAsAttr = enumInfo.GetAttribute<DumpAsAttribute>();
        if ((dumpAsAttr?.DumpString).IsNonWhiteSpace())
        {
            dumper.Append(dumpAsAttr.DumpString);
        }
        else
        {
            dumper.Append(enumInfo.Name);
        }
    }
}

public static partial class DumpCache
{
    private static readonly ConcurrentTypeDictionary<Delegate?> _dumpDelegateCache;

    static DumpCache()
    {
        _dumpDelegateCache = new()
        {
            [typeof(Type)] = (Dump<Type>)DumpType,
        };
    }


    public static string Dump<T>(this T? value)
    {
        var dumper = new Dumper();
        dumper.AppendFormatted<T>(value);
        return dumper.ToStringAndDispose();
    }

    internal static bool TryDump<T>(T? value, ref Dumper dumper)
    {
        if (_dumpDelegateCache.GetOrAdd<T>(FindDumpDelegate) is Dump<T> dump)
        {
            dump(value, ref dumper);
            return true;
        }

        return false;
    }

    private static Delegate? FindDumpDelegate(Type instanceType)
    {
        // Check implements
        // TODO: order by distance between pair.Key and InstanceType
        var implemented = _dumpDelegateCache.Where(pair => pair.Key.Implements(instanceType))
            .Select(pair => pair.Value)
            .FirstOrDefault();
        if (implemented is not null)
        {
            return implemented;
        }

        // Find IDumpable / delegate (duck-typed)
        MethodInfo? method = instanceType.GetMethod(name: "Dump",
            bindingAttr: BindingFlags.Public | BindingFlags.Instance,
            types: new Type[1] {typeof(Dumper).MakeByRefType()});
        if (method is not null)
        {
            return RuntimeBuilder.CreateDelegate(typeof(Dump<>).MakeGenericType(instanceType),
                "Invoke",
                emitter => emitter.Ldarg(0).Ldarg(1).Call(method).Ret());
        }

        // Check for DumpAsAttribute
        DumpAsAttribute? attr = instanceType.GetCustomAttribute<DumpAsAttribute>();
        if (attr is not null && !string.IsNullOrWhiteSpace(attr.DumpString))
        {
            return GetDumpString(instanceType, attr.DumpString);
        }

        // Todo: What else can we manually implement?

        return null;
    }

    private static readonly MethodInfo _dumperAppendLiteral =
        typeof(Dumper).GetMethod(nameof(Dumper.AppendLiteral),
            BindingFlags.Public | BindingFlags.Instance,
            null,
            new Type[1] {typeof(string)},
            null).ThrowIfNull($"Could not find Dumper.AppendLiteral(string?)");

    private static Delegate GetDumpString(Type instanceType, string value)
    {
        return RuntimeBuilder.CreateDelegate(typeof(Dump<>).MakeGenericType(instanceType),
            "DumpAsAttr",
            emitter => emitter
                // we never load TInstance, as it is overridden with this string
                // ref Dumper
                .Ldarg(1)
                // string
                .Ldstr(value)
                // Dumper.AppendLiteral(string)
                .Call(_dumperAppendLiteral)
                .Ret());
    }
}

#1#*/