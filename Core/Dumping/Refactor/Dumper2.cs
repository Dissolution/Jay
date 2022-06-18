using System.Diagnostics;
using System.Reflection;
using Jay.Collections;
using Jay.Reflection.Building;
using Jay.Reflection.Extensions;
using Jay.Text;
using Jay.Validation;

namespace Jay.Dumping.Refactor;

public abstract class DumperBase
{
    private static readonly ConcurrentTypeDictionary<Delegate> _dumpValueCache;

    static DumperBase()
    {
        _dumpValueCache = new()
        {
            [typeof(Type)] = (DumpValue<Type>)TypeDumper.Dump,
            [typeof(IEnumerable)] = (DumpValue<IEnumerable>)EnumerableDumper.Dump,
        };
        // TODO: auto-lookup for IDumper<T> implementations in assemblies
    }

    protected static Delegate CreateConstDumpValueDelegate<T>(Type valueType, T? constValue)
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
        return typeof(DumperBase)
            .GetMethod(nameof(DefaultDumpValue),
                BindingFlags.NonPublic | BindingFlags.Static,
                new Type[] { type, typeof(TextBuilder), typeof(DumpOptions) })
            .ThrowIfNull($"Could not find {nameof(DumperBase)}.{nameof(DefaultDumpValue)}")
            .MakeGenericMethod(type);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DefaultDumpValue<T>(T? value, TextBuilder text, DumpOptions? options)
    {
        text.WriteFormatted(value, options?.Format, options?.FormatProvider);
    }
    
    protected static Delegate GetOrCreateDelegate(Type type)
    {
        return _dumpValueCache.GetOrAdd(type, t => CreateDumpValueDelegate(t));
    }
}