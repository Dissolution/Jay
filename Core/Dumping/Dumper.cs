using System.Linq.Expressions;
using System.Reflection;
using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Building;
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
        AddDumper<bool>(DumpBoolTo);
        AddDumper<Array>(DumpArrayTo);
        AddDumper<Exception>(DumpExceptionTo);
        AddDumper<Type>(DumpTypeTo);
        AddDumper<FieldInfo>(DumpFieldTo);
        AddDumper<PropertyInfo>(DumpPropertyTo);
        AddDumper<EventInfo>(DumpEventTo);
        AddDumper<ConstructorInfo>(DumpConstructorTo);
        AddDumper<MethodBase>(DumpMethodTo);
        AddDumper<MethodInfo>(DumpMethodTo);
        AddDumper<ParameterInfo>(DumpParameterTo);
        AddDumper<Expression>(DumpExpressionTo);
    }

    private static void AddDumper<T>(DumpValueTo<T> valueDumper) => _valueDumpCache.Set<T>(valueDumper);

    #region DumpValueTo Implementations

    private static void DumpBoolTo(bool boolean, TextBuilder text)
    {
        if (boolean)
        {
            text.Write("true");
        }
        else
        {
            text.Write("false");
        }
    }
    
    private static void DumpEnumTo<TEnum>(TEnum value, TextBuilder text)
        where TEnum : struct, Enum
    {
        //value.GetInfo().DumpTo(text);
        text.Write<TEnum>(value);
        
    }

    private static void DumpTupleTo<TTuple>(TTuple? tuple, TextBuilder text)
        where TTuple : ITuple
    {
        text.Write('(');
        if (tuple is not null)
        {
            for (var i = 0; i < tuple.Length; i++)
            {
                if (i > 0) text.Write(',');
                DumpObjectTo(tuple[i], text);
            }
        }
        text.Write(')');
    }

    private static void DumpAsTextTo<T>(T? value, TextBuilder text)
    {
        text.Write<T>(value);
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
                .ThrowIfNull("Could not find Dumper.DumpArrayTo");
            return Delegate.CreateDelegate(delegateType, method);
        }
        
        // Is enum?
        if (valueType.IsEnum)
        {
            method = typeof(Dumper).GetMethod(nameof(DumpEnumTo),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .ThrowIfNull("Could not find Dumper.DumpEnumTo");
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
        if (valueType.Implements(typeof(IEnumerable<>)) && valueType != typeof(string))
        {
            var itemType = valueType.GenericTypeArguments[0];
            method = typeof(Dumper).GetMethod(nameof(DumpEnumerableTo),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .ThrowIfNull("Could not find Dumper.DumpEnumerableTo")
                .MakeGenericMethod(itemType);
            return Delegate.CreateDelegate(delegateType, method);
        }
        
        // Tuple?
        if (valueType.Implements<ITuple>())
        {
            method = typeof(Dumper).GetMethod(nameof(DumpTupleTo),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .ThrowIfNull("Could not find Dumper.DumpTupleTo")
                .MakeGenericMethod(valueType)
                .ThrowIfNull("Invalid Operation");
            return Delegate.CreateDelegate(delegateType, method);
        }
        
        // Any sort of MemberInfo?
        if (valueType.Implements<MemberInfo>())
        {
            if (valueType.Implements<Type>())
            {
                method = typeof(Dumper).GetMethod(nameof(DumpTypeTo),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    .ThrowIfNull("Could not find Dumper.DumpTypeTo");
            }
            else if (valueType.Implements<FieldInfo>())
            {
                method = typeof(Dumper).GetMethod(nameof(DumpFieldTo),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    .ThrowIfNull("Could not find Dumper.DumpFieldTo");
            }
            else if (valueType.Implements<PropertyInfo>())
            {
                method = typeof(Dumper).GetMethod(nameof(DumpPropertyTo),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    .ThrowIfNull("Could not find Dumper.DumpPropertyTo");
            }
            else if (valueType.Implements<EventInfo>())
            {
                method = typeof(Dumper).GetMethod(nameof(DumpEventTo),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    .ThrowIfNull("Could not find Dumper.DumpEventTo");
            }
            else if (valueType.Implements<ConstructorInfo>())
            {
                method = typeof(Dumper).GetMethod(nameof(DumpConstructorTo),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    .ThrowIfNull("Could not find Dumper.DumpConstructorTo");
            }
            else if (valueType.Implements<MethodBase>())
            {
                method = typeof(Dumper).GetMethod(nameof(DumpMethodTo),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    .ThrowIfNull("Could not find Dumper.DumpMethodTo");
            }
            else
            {
                throw new NotImplementedException();
            }

            return Delegate.CreateDelegate(delegateType, method);
        }

        // Fallback to using TextBuilder directly
        method = typeof(Dumper).GetMethod(nameof(DumpAsTextTo),
                BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull("Could not find Dumper.DumpAsTextTo")
            .MakeGenericMethod(valueType);
        return Delegate.CreateDelegate(delegateType, method);
    }

    private static DumpValueTo<T> CreateValueDumpDelegate<T>()
    {
        var del = CreateValueDumpDelegate(typeof(T));
        if (del is DumpValueTo<T> dumpValueTo)
            return dumpValueTo;
        throw new InvalidOperationException();
    }
    
    private static Delegate GetDumpValueDelegate(Type type)
    {
        var del = _valueDumpCache.GetOrAdd(type, _ => CreateValueDumpDelegate(type));
        return del;
    }
    
    private static DumpValueTo<T> GetDumpValueDelegate<T>()
    {
        var del = _valueDumpCache.GetOrAdd<T>(_ => CreateValueDumpDelegate<T>());
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

    public static ref InterpolatedDumpHandler StartDump(ref InterpolatedDumpHandler dumpString)
    {
        return ref dumpString;
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