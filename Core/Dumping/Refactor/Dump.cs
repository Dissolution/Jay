﻿using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Jay.Collections;
using Jay.Exceptions;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Text;
using Jay.Validation;
using static InlineIL.IL;

namespace Jay.Dumping.Refactor;

public static class DumperTest
{
    public static string DumpWith(ref Dumper dumper)
    {
        return dumper.ToStringAndDispose();
        //DefaultInterpolatedStringHandler
    }
}

public static class DumperExtensions
{
    public static ref Dumper Append(this ref Dumper dumper, char ch)
    {
        dumper.AppendFormatted(ch);
        Emit.Ldarg(0);
        Emit.Ret();
        throw Unreachable();
    }

    public static ref Dumper Append(this ref Dumper dumper, ReadOnlySpan<char> text)
    {
        dumper.AppendFormatted(text);
        Emit.Ldarg(0);
        Emit.Ret();
        throw Unreachable();
    }

    public static ref Dumper Append(this ref Dumper dumper, object? value)
    {
        dumper.AppendFormatted(value);
        Emit.Ldarg(0);
        Emit.Ret();
        throw Unreachable();
    }


    public static ref Dumper Append<T>(this ref Dumper dumper, T? value)
    {
        dumper.AppendFormatted<T>(value);
        Emit.Ldarg(0);
        Emit.Ret();
        throw Unreachable();
    }
}

[InterpolatedStringHandler]
public ref struct Dumper
{
    private static int GetCapacity(int literalLength, int formattedCount)
        => Math.Max(1024, literalLength + (formattedCount * 16));


    private char[]? _charArray;
    private Span<char> _charSpan;

    private int _index;
    private bool _deep;

    public int Length => _index;

    public bool Deep
    {
        get => _deep;
        set => _deep = value;
    }

    internal Span<char> Written => _charSpan[.._index];
    internal Span<char> Available => _charSpan[_index..];

    public Dumper(int literalLength, int formattedCount)
    {
        _charSpan = _charArray = ArrayPool<char>.Shared.Rent(GetCapacity(literalLength, formattedCount));
        _index = 0;
        _deep = false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int adding)
    {
        // string.MaxLength < array.MaxLength
        const int maxCapacity = 0x3FFFFFDF;
        int newCapacity = Math.Clamp(_index + adding, _charSpan.Length * 2, maxCapacity);
        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        TextHelper.Copy(in _charSpan.GetPinnableReference(),
            ref newArray[0],
            _index);
        char[]? toReturn = _charArray;
        _charSpan = _charArray = newArray;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    public void AppendLiteral(string? text)
    {
        if (text is not null)
        {
            while (!TextHelper.TryCopyTo(text, Available))
            {
                Grow(text.Length);
            }

            _index += text.Length;
        }
    }

    public void AppendFormatted(char ch)
    {
        if (Available.Length == 0)
            Grow(1);
        Available[0] = ch;
        _index++;
    }

    public void AppendFormatted(string? text)
    {
        AppendLiteral(text);
    }

    public void AppendFormatted(ReadOnlySpan<char> text)
    {
        while (!TextHelper.TryCopyTo(text, Available))
        {
            Grow(text.Length);
        }

        _index += text.Length;
    }

    public void AppendFormatted(object? value, string? format = null)
    {
        AppendFormatted<object>(value, format);
    }

    public void AppendFormatted<T>(T? value, string? format = null)
    {
        if (!DumpCache.TryDump<T>(value, ref this))
        {
            if (value is IFormattable)
            {
                if (value is ISpanFormattable)
                {
                    int charsWritten;
                    while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, default))
                    {
                        Grow(charsWritten);
                    }

                    _index += charsWritten;
                }
                else
                {
                    AppendLiteral(((IFormattable)value).ToString(format, null));
                }
            }
            else
            {
                AppendLiteral(value?.ToString());
            }
        }
    }

    public void Clear()
    {
        _index = 0;
    }

    public void Dispose()
    {
        char[]? toReturn = _charArray;
        this = default; // defensive clear
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    public string ToStringAndDispose()
    {
        string str = new string(Written);
        Dispose();
        return str;
    }

    public override bool Equals(object? obj) => UnsuitableException.ThrowEquals(typeof(Dumper));

    public override int GetHashCode() => UnsuitableException.ThrowGetHashCode(typeof(Dumper));

    public override string ToString()
    {
        return new string(Written);
    }
}

public delegate void Dump<in TInstance>(TInstance? instance, ref Dumper dumper);

public interface IDumpable
{
    void Dump(ref Dumper dumper);
}

/// <summary>
/// DumpCache: MemberInfo
/// </summary>
public static partial class DumpCache
{
    private static readonly HashSet<string> _ignoredNamespaces = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "System",
        "Microsoft",
    };

    private static void DumpType(Type? type, ref Dumper dumper)
    {
        if (type is null)
        {
            dumper.AppendLiteral("null");
            return;
        }

        if (type == typeof(bool))
        {
            dumper.AppendLiteral("bool");
            return;
        }

        if (type == typeof(char))
        {
            dumper.AppendLiteral("char");
            return;
        }

        if (type == typeof(sbyte))
        {
            dumper.AppendLiteral("sbyte");
            return;
        }

        if (type == typeof(byte))
        {
            dumper.AppendLiteral("byte");
            return;
        }

        if (type == typeof(short))
        {
            dumper.AppendLiteral("short");
            return;
        }

        if (type == typeof(ushort))
        {
            dumper.AppendLiteral("ushort");
            return;
        }

        if (type == typeof(int))
        {
            dumper.AppendLiteral("int");
            return;
        }

        if (type == typeof(uint))
        {
            dumper.AppendLiteral("uint");
            return;
        }

        if (type == typeof(long))
        {
            dumper.AppendLiteral("long");
            return;
        }

        if (type == typeof(ulong))
        {
            dumper.AppendLiteral("ulong");
            return;
        }

        if (type == typeof(float))
        {
            dumper.AppendLiteral("float");
            return;
        }

        if (type == typeof(double))
        {
            dumper.AppendLiteral("double");
            return;
        }

        if (type == typeof(decimal))
        {
            dumper.AppendLiteral("decimal");
            return;
        }

        if (type == typeof(string))
        {
            dumper.AppendLiteral("string");
            return;
        }

        if (type == typeof(object))
        {
            dumper.AppendLiteral("object");
            return;
        }

        // TODO: deep print namespace

        Type? underlyingType;

        // Enums
        if (type.IsEnum)
        {
            dumper.AppendLiteral(type.Name);
            return;
        }

        // Print a Declaring Type for Nested Types
        if (type.IsNested && !type.IsGenericParameter)
        {
            DumpType(type.DeclaringType, ref dumper);
            dumper.AppendFormatted('.');
        }

        underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is not null)
        {
            DumpType(underlyingType, ref dumper);
            dumper.AppendFormatted('?');
            return;
        }

        if (type.IsPointer)
        {
            underlyingType = type.GetElementType();
            Debug.Assert(underlyingType != null);
            DumpType(underlyingType, ref dumper);
            dumper.AppendLiteral("*");
            return;
        }

        if (type.IsByRef)
        {
            underlyingType = type.GetElementType();
            Debug.Assert(underlyingType != null);
            dumper.AppendLiteral("ref ");
            DumpType(underlyingType, ref dumper);
            return;
        }

        if (type.IsArray)
        {
            underlyingType = type.GetElementType();
            Debug.Assert(underlyingType != null);
            DumpType(underlyingType, ref dumper);
            dumper.AppendLiteral("[]");
            return;
        }

        string name = type.Name;

        if (type.IsGenericType)
        {
            if (type.IsGenericParameter)
            {
                dumper.AppendLiteral(name);
                var constraints = type.GetGenericParameterConstraints();
                if (constraints.Length > 0 && dumper.Deep)
                {
                    dumper.AppendLiteral(" : ");
                    Debugger.Break();
                }

                return;
            }

            var genericTypes = type.GetGenericArguments();
            var i = name.IndexOf('`');
            Debug.Assert(i >= 0);
            dumper.Append(name[..i]).Append('<');
            for (i = 0; i < genericTypes.Length; i++)
            {
                if (i > 0) dumper.Append(',');
                DumpType(genericTypes[i], ref dumper);
            }

            dumper.Append('>');
        }
        else
        {
            dumper.Append(name);
        }
    }

    private static void DumpField(FieldInfo? field, ref Dumper dumper)
    {
        if (dumper.Deep)
        {
            var visibility = field.Visibility();
        }
    }
}

public static partial class DumpCache
{
    private static void DumpEnum<TEnum>(TEnum @enum, ref Dumper dumper)
        where TEnum : struct, Enum
    {
        var enumInfo = @enum.GetInfo();
        var dumpAsAttr = enumInfo.GetAttribute<DumpAsAttribute>();
        if ((dumpAsAttr?.Value).IsNonWhiteSpace())
        {
            dumper.AppendLiteral(dumpAsAttr.Value);
        }
        else
        {
            dumper.AppendLiteral(enumInfo.Name);
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
        if (attr is not null && !string.IsNullOrWhiteSpace(attr.Value))
        {
            return GetDumpString(instanceType, attr.Value);
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