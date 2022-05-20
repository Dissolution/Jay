using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using InlineIL;
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
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Empty:
                dumper.AppendLiteral("null");
                return;
            case TypeCode.DBNull:
                dumper.AppendLiteral(nameof(DBNull));
                return;
            case TypeCode.Boolean:
                dumper.AppendLiteral("bool");
                return;
            case TypeCode.Char:
                dumper.AppendLiteral("char");
                return;
            case TypeCode.SByte:
                dumper.AppendLiteral("sbyte");
                return;
            case TypeCode.Byte:
                dumper.AppendLiteral("byte");
                return;
            case TypeCode.Int16:
                dumper.AppendLiteral("short");
                return;
            case TypeCode.UInt16:
                dumper.AppendLiteral("ushort");
                return;
            case TypeCode.Int32:
                dumper.AppendLiteral("int");
                return;
            case TypeCode.UInt32:
                dumper.AppendLiteral("uint");
                return;
            case TypeCode.Int64:
                dumper.AppendLiteral("long");
                return;
            case TypeCode.UInt64:
                dumper.AppendLiteral("ulong");
                return;
            case TypeCode.Single:
                dumper.AppendLiteral("float");
                return;
            case TypeCode.Double:
                dumper.AppendLiteral("double");
                return;
            case TypeCode.Decimal:
                dumper.AppendLiteral("decimal");
                return;
            case TypeCode.DateTime:
                dumper.AppendLiteral(nameof(DateTime));
                return;
            case TypeCode.String:
                dumper.AppendLiteral("string");
                return;
            case TypeCode.Object:
            default:
                break;
        }
        Debug.Assert(type != null);
        Type? underlyingType;

        if (type == typeof(object))
        {
            dumper.AppendLiteral("object");
            return;
        }
        
        // TODO: deep print namespace

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
        throw new NotImplementedException();
    }
}

public static class EnumExtensions
{
    private static class EnumTypeInfo<TEnum> where TEnum : struct, Enum
    {
        private sealed class EnumInfo : IEquatable<EnumInfo>,
                                        IEquatable<TEnum>
        {
            public TEnum Enum { get; }
            public string Name { get; }
            public Attribute[] Attributes { get; }

            public EnumInfo(FieldInfo enumMemberField)
            {
                this.Name = enumMemberField.Name;
                this.Attributes = Attribute.GetCustomAttributes(enumMemberField, true);
                this.Enum = (TEnum)enumMemberField.GetValue(null)!;
            }

            public bool Equals(EnumInfo? enumInfo)
            {
                return enumInfo is not null && EnumTypeInfo<TEnum>.Equals(Enum, enumInfo.Enum);
            }

            public bool Equals(TEnum @enum)
            {
                return EnumTypeInfo<TEnum>.Equals(Enum, @enum);
            }

            public override bool Equals(object? obj)
            {
                if (obj is EnumInfo enumInfo) return Equals(enumInfo);
                if (obj is TEnum @enum) return Equals(@enum);
                if (obj is Enum) Debugger.Break();
                return false;
            }

            public override int GetHashCode()
            {
                Emit.Ldarg_0();
                Emit.Ldfld(FieldRef.Field(typeof(EnumInfo), nameof(Enum)));
                Emit.Conv_I4();
                return Return<int>();
            }

            public override string ToString()
            {
                return $"({EnumTypeInfo<TEnum>.Name}){Name}";
            }
        }


        static EnumTypeInfo()
        {
            Type = typeof(TEnum);
            Debug.Assert(Type.IsEnum);
            Attributes = Attribute.GetCustomAttributes(Type, true);
            Name = Type.Dump();
        }

        public static Type Type { get; }
        public static Attribute[] Attributes { get; }
        public static string Name { get; }

        private static readonly Dictionary<TEnum, EnumInfo> _enumInfos;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(TEnum x, TEnum y)
        {
            Emit.Ldarg(nameof(x));
            Emit.Ldarg(nameof(y));
            Emit.Ceq();
            return Return<bool>();
        }

    }

    public static Attribute[] GetAttributes<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        //return EnumTypeInfo<TEnum>
        throw new NotImplementedException();
    }
}

[AttributeUsage(AttributeTargets.Enum)]
public class DumpAsAttribute : Attribute
{
    public string? Dump { get; }

    public DumpAsAttribute(char ch)
    {
        if (ch == default)
        {
            Dump = default;
        }
        else
        {
            Dump = new string(ch, 1);
        }
    }

    public DumpAsAttribute(string? dump)
    {
        if (string.IsNullOrWhiteSpace(dump))
        {
            Dump = default;
        }
        else
        {
            Dump = dump;
        }
    }

    public override string ToString()
    {
        return $"Dump as '{Dump}'";
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
        using var dumper = new Dumper();
        dumper.AppendFormatted<T>(value);
        return dumper.ToString();
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

        MethodInfo? method;
        
        // Find IDumpable / delegate (duck-typed)
        method = instanceType.GetMethod(name: "Dump",
            bindingAttr: BindingFlags.Public | BindingFlags.Instance,
            types: new Type[1] { typeof(Dumper).MakeByRefType() });
        if (method is not null)
        {
            return RuntimeBuilder.CreateDelegate(typeof(Dump<>).MakeGenericType(instanceType),
                "Invoke",
                emitter => emitter.Ldarg(0).Ldarg(1).Call(method).Ret());
        }

        // Todo: What else can we manually implement?

        return null;
    }
}