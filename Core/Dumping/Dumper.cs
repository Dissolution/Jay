using System.Diagnostics;
using System.Reflection;
using Jay.Collections;
using Jay.Enums;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Extensions;
using Jay.Reflection.Search;
using Jay.Text;
using Jay.Validation;

namespace Jay.Dumping;

public static partial class Dumper
{
    private static readonly ConcurrentTypeDictionary<Delegate> _valueDumpCache;

    static Dumper()
    {
        _valueDumpCache = new();
        AddDumper<object>(DumpObjectTo);
        AddDumper<Array>(DumpArrayTo);
        AddDumper<Exception>(DumpExceptionTo);
        AddDumper<Type>(DumpTypeTo);
        AddDumper<FieldInfo>(DumpFieldTo);
        AddDumper<PropertyInfo>(DumpPropertyTo);
        AddDumper<EventInfo>(DumpEventTo);
        AddDumper<ConstructorInfo>(DumpConstructorTo);
        AddDumper<MethodInfo>(DumpMethodTo);
        AddDumper<ParameterInfo>(DumpParameterTo);
    }

    private static void AddDumper<T>(DumpValueTo<T> valueDumper) => _valueDumpCache.Set<T>(valueDumper);

    #region DumpValueTo Implementations
    private static void DumpEnumTo<TEnum>(TEnum value, TextBuilder text)
        where TEnum : struct, Enum
    {
        value.GetInfo().DumpTo(text);
    }
    #endregion

    #region Cache Methods
    private static Delegate CreateValueDumpDelegate(Type valueType)
    {
        var delegateType = typeof(DumpValueTo<>).MakeGenericType(valueType);
        MethodInfo? method;
        
        // Array
        if (valueType.IsArray)
        {
            method = typeof(Dumper).GetMethod(nameof(DumpArrayTo),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .ThrowIfNull("Could not find Dump_Cache.DumpArrayTo");
            return Delegate.CreateDelegate(delegateType, method);
        }
        
        // Is enum?
        if (valueType.IsEnum)
        {
            method = typeof(Dumper).GetMethod(nameof(DumpEnumTo),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .ThrowIfNull("Could not find Dump_Cache.DumpEnumTo");
            return Delegate.CreateDelegate(delegateType, method);
        }
        
        // Look for IDumpable (duck)
        method = valueType.GetMethod(nameof(IDumpable.DumpTo),
            BindingFlags.Public | BindingFlags.Instance,
            new Type[1] { typeof(TextBuilder) });
        if (method is not null)
        {
            return Delegate.CreateDelegate(delegateType, method);
        }
        
        // IEnumerable?
        if (valueType.Implements(typeof(IEnumerable<>)))
        {
            var itemType = valueType.GenericTypeArguments[0];
            method = typeof(Dumper).GetMethod(nameof(DumpEnumerableTo),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .ThrowIfNull("Could not find Dump_Cache.DumpEnumerableTo")
                .MakeGenericMethod(itemType);
            return Delegate.CreateDelegate(delegateType, method);
        }
        
        // Fallback to using TextBuilder directly
        method = TextBuilderReflections.GetWriteValue(valueType);
        return RuntimeBuilder.CreateDelegate(delegateType, runtimeMethod =>
        {
            var emitter = runtimeMethod.Emitter;
            emitter.Ldarg(runtimeMethod.Parameters[1])
                .Ldarg(runtimeMethod.Parameters[0])
                .Call(method)
                .Ret();
        });
    }

    private static DumpValueTo<T> CreateValueDumpDelegate<T>()
    {
        return (CreateValueDumpDelegate(typeof(T)) as DumpValueTo<T>)!;
    }
    
    private static Delegate GetDumpValueDelegate(Type type)
    {
        var del = _valueDumpCache.GetOrAdd(type, CreateValueDumpDelegate);
        return del;
    }
    
    private static DumpValueTo<T> GetDumpValueDelegate<T>()
    {
        var del = _valueDumpCache.GetOrAdd<T>(CreateValueDumpDelegate);
        if (del is DumpValueTo<T> valueDump)
            return valueDump;
        throw new InvalidOperationException();
    }
    #endregion
  
    #region Misc Dumping
    public static void DumpValueTo<T>(T? value, TextBuilder text)
    {
        GetDumpValueDelegate<T>()(value, text);
    }

    public static string Dump<T>(T? value)
    {
        using var text = TextBuilder.Borrow();
        DumpValueTo<T>(value, text);
        return text.ToString();
    }

    public static string Dump(ref InterpolatedDumpHandler dumpString)
    {
        return dumpString.ToStringAndDispose();
    }
    #endregion
    
    #region Exceptions
    public static TException GetException<TException>(ref InterpolatedDumpHandler message, Exception? innerException = null)
        where TException : Exception
    {
        var ctor = ExceptionBuilder.GetCommonConstructor<TException>();
        var ex = ctor(message.ToStringAndDispose(), innerException);
        return ex;
    }
    
    public static TException GetException<TException>(ref InterpolatedDumpHandler message, params object?[] args)
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
    public static void ThrowException<TException>(ref InterpolatedDumpHandler message, Exception? innerException = null)
        where TException : Exception
    {
        throw GetException<TException>(ref message, innerException);
    }

    [DoesNotReturn]
    public static void ThrowException<TException>(ref InterpolatedDumpHandler message, params object?[] args)
        where TException : Exception
    {
        throw GetException<TException>(ref message, args);
    }
    #endregion
}