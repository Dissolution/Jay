using System.Diagnostics;
using System.Reflection;
using Jay.Collections;
using Jay.Reflection.Building;
using Jay.Reflection.Extensions;
using Jay.Text;
using Jay.Validation;

namespace Jay.Dumping.Refactor;

public abstract class Dumper
{
    private static readonly ConcurrentTypeDictionary<Delegate> _dumpValueCache;

    static Dumper()
    {
        _dumpValueCache = new()
        {
            [typeof(Type)] = (DumpValue<Type>)TypeDumper.Dump,
            [typeof(IEnumerable)] = (DumpValue<IEnumerable>)EnumerableDumper.Dump,
        };
        // TODO: auto-lookup for IDumper<T> implementations in assemblies
    }

    protected static Delegate CreateConstDumpValueDelegate<TConst>(Type valueType, TConst? constValue)
    {
        return RuntimeBuilder.CreateDelegate(typeof(DumpValue<>).MakeGenericType(valueType),
            $"Dump_{valueType.Name}_override",
            dynamicMethod =>
            {
                var emitter = dynamicMethod.GetEmitter();
                emitter.Ldarg_1()
                    .Load(constValue)
                    .Call(TextBuilderReflections.GetWriteValue(valueType))
                    .Ret();
            });
    }
    
    protected static Delegate CreateDumpValueDelegate(Type type)
    {
        // Check for and use [DumpAs]
        var attr = type.GetCustomAttribute<DumpAsAttribute>();
        if (attr is not null && attr.DumpString is not null)
        {
            return CreateConstDumpValueDelegate(type, attr.DumpString);
        }

        var delegateType = typeof(DumpValue<>).MakeGenericType(type);
        
        // Check for IDumpable
        if (type.Implements<IDumpable>())
        {
            return RuntimeBuilder.CreateDelegate(
                delegateType,
                $"Dump_{type.FullName}",
                dynamicMethod =>
                {
                    var emitter = dynamicMethod.GetEmitter();
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
        return Delegate.CreateDelegate(delegateType, GetDumperDefaultDumpValue(type), true)
            .ThrowIfNull();
    }

    private static MethodInfo GetDumperDefaultDumpValue(Type type)
    {
        return typeof(Dumper)
            .GetMethod(nameof(DefaultDumpValue),
                BindingFlags.NonPublic | BindingFlags.Static,
                new Type[] { type, typeof(TextBuilder), typeof(DumpOptions) })
            .ThrowIfNull($"Could not find {nameof(Dumper)}.{nameof(DefaultDumpValue)}")
            .MakeGenericMethod(type);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DefaultDumpValue<T>(T? value, TextBuilder text, DumpOptions? options)
    {
        text.WriteFormatted(value, options?.Format, options?.FormatProvider);
    }
    
    protected static Delegate GetOrCreateDelegate(Type? type)
    {
        if (type is null)
            return
        
        return _dumpValueCache.GetOrAdd(type, t => CreateDumpValueDelegate(t));
    }

    protected static DumpValue<T> GetOrCreateDelegate<T>()
    {
        var dumpValue = GetOrCreateDelegate(typeof(T)) as DumpValue<T>;
        if (dumpValue is null)
            throw new InvalidOperationException();
        return dumpValue;
    }
    
    protected static bool TryDumpNull<T>([NotNullWhen(false)] T? value, 
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

    public static void Dump(object? obj, TextBuilder text, DumpOptions? options = default)
    {
        var dumpDelegate = GetOrCreateDelegate(obj?)
    }
    
    public static void Dump<T>(T? value, TextBuilder text, DumpOptions? options = default)
    {
        var dumpDelegate = GetOrCreateDelegate<T>();
        dumpDelegate(value, text, options);
    }
    
    

    public static string Dump(ref InterpolatedDumpHandler dumpString)
    {
        return dumpString.ToStringAndDispose();
    }

}

public static class DumpExtensions
{
    public static string Dump<T>(this T? value, DumpOptions? options = default)
    {
        using var text = TextBuilder.Borrow();
        Dumper.Dump<T>(value, text, options);
        return text.ToString();
    }

    public static TextBuilder AppendDump(this TextBuilder textBuilder, object? value, DumpOptions? options = default)
    {
        Dumper.Dump(value, textBuilder, options);
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
}

public abstract class Dumper<T> : Dumper
{
    protected static DumpValue<T> CreateConstDumpValueDelegate<TConst>(TConst? constValue)
    {
        return (CreateConstDumpValueDelegate(typeof(T), constValue) as DumpValue<T>)
            .ThrowIfNull();
    }

   
}