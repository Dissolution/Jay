using System.Collections.Concurrent;
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

namespace Jay.Dumping.Refactor2;

public static class Scratch
{
    static Scratch()
    {
        var thing = new ConcurrentStack<int>();
    }
}

public static class Dumper
{
    internal static readonly ConcurrentTypeDictionary<Delegate> _valueDumpCache;
    internal static readonly ConcurrentTypeDictionary<DumpValueTo<object>> _objectDumpCache;
    internal static readonly ConcurrentTypeDictionary<string> _typeDumpCache;

    static Dumper()
    {
        _objectDumpCache = new();
        _typeDumpCache = new();
        _valueDumpCache = new();
        _valueDumpCache[typeof(object)] = (DumpValueTo<object>)(DumpObjectTo);
        _valueDumpCache[typeof(Type)] = (DumpValueTo<Type>)(DumpTypeTo);
        _valueDumpCache[typeof(Array)] = (DumpValueTo<Array>)(DumpArrayTo);
    }

    #region DumpValueTo Implementations
    private static void DumpObjectTo(object? obj, TextBuilder text)
    {
        if (obj is null)
        {
            text.Write("null");
            return;
        }

        var del = _objectDumpCache.GetOrAdd(obj.GetType(), type =>
        {
            var valueDumpDel = GetDumpValueDelegate(type);
            var valueDumpDelInvokeMethod = valueDumpDel.Method;
            return RuntimeBuilder.CreateDelegate<DumpValueTo<object>>($"Dump_Object_{type}",
                runtimeMethod =>
                {
                    var emitter = runtimeMethod.Emitter;

                    emitter.Ldarg(runtimeMethod.Parameters[0])
                        .Cast(typeof(object), type)
                        .Ldarg(runtimeMethod.Parameters[1])
                        .Call(valueDumpDelInvokeMethod)
                        .Ret();
                });
        });

        if (del is DumpValueTo<object> objectDump)
        {
            objectDump(obj, text);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private static void DumpEnumTo<TEnum>(TEnum value, TextBuilder text)
        where TEnum : struct, Enum
    {
        value.GetInfo().DumpTo(text);
    }

    private static void DumpTypeTo(Type? type, TextBuilder text)
    {
        if (type is null)
        {
            text.Write("null");
        }
        else
        {
            if (_typeDumpCache.TryGetValue(type, out var str))
            {
                text.Write(str);
            }
            // This writes + creates
            str = CreateTypeString(type, text);
            _typeDumpCache.TryAdd(type, str);
        }
        
    }

    private static string CreateTypeString(Type type, TextBuilder textBuilder)
    {
        var start = textBuilder.Length;
        if (type == typeof(bool))
            textBuilder.Write("bool");
        else if (type == typeof(char))
            textBuilder.Write("char");
        else if (type == typeof(sbyte))
            textBuilder.Write("sbyte");
        else if (type == typeof(byte))
            textBuilder.Write("byte");
        else if (type == typeof(short))
            textBuilder.Write("short");
        else if (type == typeof(ushort))
            textBuilder.Write("ushort");
        else if (type == typeof(int))
            textBuilder.Write("int");
        else if (type == typeof(uint))
            textBuilder.Write("uint");
        else if (type == typeof(long))
            textBuilder.Write("long");
        else if (type == typeof(ulong))
            textBuilder.Write("ulong");
        else if (type == typeof(float))
            textBuilder.Write("float");
        else if (type == typeof(double))
            textBuilder.Write("double");
        else if (type == typeof(decimal))
            textBuilder.Write("decimal");
        else if (type == typeof(string))
            textBuilder.Write("string");
        else if (type == typeof(object))
            textBuilder.Write("object");
        else if (type == typeof(void))
            textBuilder.Write("void");
        else if (type.IsEnum)
            textBuilder.Write(type.Name);
        else
        {
            // Print a Declaring Type for Nested Types
            if (type!.IsNested && !type.IsGenericParameter)
            {
                DumpTypeTo(type.DeclaringType, textBuilder);
                textBuilder.Write('.');
            }

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
            {
                DumpTypeTo(underlyingType, textBuilder);
                textBuilder.Write('?');
            }
            else if (type.IsPointer)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                DumpTypeTo(underlyingType, textBuilder);
                textBuilder.Write('*');
            }
            else if (type.IsByRef)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                textBuilder.Write("ref ");
                DumpTypeTo(underlyingType, textBuilder);
            }
            else if (type.IsArray)
            {
                underlyingType = type.GetElementType();
                Debug.Assert(underlyingType != null);
                DumpTypeTo(underlyingType, textBuilder);
                textBuilder.Write("[]");
            }
            
            string name = type.Name;

            if (type.IsGenericType)
            {
                if (type.IsGenericParameter)
                {
                    textBuilder.Write(name);
                    var constraints = type.GetGenericParameterConstraints();
                    if (constraints.Length > 0)
                    {
                        textBuilder.Write(" : ");
                        Debugger.Break();
                    }
                    Debugger.Break();
                }

                var genericTypes = type.GetGenericArguments();
                var i = name.IndexOf('`');
                Debug.Assert(i >= 0);
                textBuilder.Append(name[..i])
                    .Append('<')
                    .AppendDelimit(",", 
                        genericTypes, 
                        (tb, gt) => DumpTypeTo(gt, tb))
                    .Append('>');
            }
            else
            {
                textBuilder.Write(name);
            }
        }

        var end = textBuilder.Length;
        int length = end - start;
        Debug.Assert(length > 0);
        return new string(textBuilder.Written.Slice(0, length));
    }

    private static void DumpArrayTo(Array? array, TextBuilder textBuilder)
    {
        if (array is null)
        {
            textBuilder.Write("null");
            return;
        }
        // T
        DumpTypeTo(array.GetType().GetElementType(), textBuilder);
        textBuilder.Append('[')
            .AppendDelimit(",", Enumerable.Range(0, array.Rank), (tb, dimension) =>
            {
                tb.Write(array.GetLength(dimension));
            })
            .Append("]");
        if (array.Rank == 1)
        {
            textBuilder.Append('{')
                .AppendDelimit(",", array, (tb, obj) => DumpObjectTo(obj, tb))
                .Append('}');
        }
        else
        {
            // Todo: Proper multi-rank array support
            throw new NotImplementedException();
        }
    }

    private static void DumpEnumerableTo<T>(IEnumerable<T> enumerable, TextBuilder textBuilder)
    {
        // TEnumerable<T>
        DumpTypeTo(enumerable.GetType(), textBuilder);
        
        // Can we show count?
        if (enumerable.TryGetNonEnumeratedCount(out int count))
        {
            textBuilder.Append('[')
                .Append(count)
                .Append(']');
        }
        
        // Dump each item in turn
        textBuilder.Append('{')
            .AppendDelimit(",", enumerable, (tb, obj) => DumpObjectTo(obj, tb))
            .Append('}');
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

public static class DumpExtensions
{
    public static string Dump<T>(this T? value)
    {
        using var text = TextBuilder.Borrow();
        Dumper.DumpValueTo<T>(value, text);
        return text.ToString();
    }

    public static void DumpTo<T>(this T? value, TextBuilder textBuilder)
    {
        Dumper.DumpValueTo<T>(value, textBuilder);
    }

    public static TextBuilder AppendDump<T>(this TextBuilder textBuilder, T? value)
    {
        Dumper.DumpValueTo<T>(value, textBuilder);
        return textBuilder;
    }

    public static TextBuilder AppendDump<T>(this TextBuilder textBuilder,
        [InterpolatedStringHandlerArgument("textBuilder")]
        ref InterpolatedDumpHandler dumpString)
    {
        // dumpString evaluation will write to textbuilder
        dumpString.ToString();
        Debugger.Break();
        return textBuilder;
    }
}