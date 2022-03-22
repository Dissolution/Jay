using System.Diagnostics;
using System.Reflection;
using Jay;
using Jay.Comparision;
using Jay.Reflection;
using static InlineIL.IL;

namespace ConsoleSandbox;

public enum eSByte : sbyte
{
    First,
    Second,
    Third,
}

public enum eULong : ulong
{
    Default,
    Yes,
    No,
    WE,
}


public static class EnumWrapperExtensions
{
    public static string GetName<TEnum>(this TEnum e)
        where TEnum : struct, Enum
        => EnumWrapper<TEnum>.GetName(e);
}

public partial class EnumWrapper<TEnum>
    where TEnum : struct, Enum
{
    internal static readonly Type EnumType;
    internal static readonly int Size;
    internal static readonly Type UnderlyingType;
    internal static readonly Attribute[] EnumAttributes;
    internal static readonly bool IsFlags;
    
    protected static readonly SortedList<TEnum,EnumWrapper<TEnum>> _definedMembers;

    public static IEqualityComparer<TEnum> EqualityComparer { get; }
    public static IComparer<TEnum> Comparer { get; }

    public static IEnumerable<TEnum> Members => _definedMembers.Keys;
    public static IEnumerable<string> Names => _definedMembers.Values.Select(w => w.Name);

    static EnumWrapper()
    {
        EnumType = typeof(TEnum);
        Debug.Assert(EnumType.IsEnum);
        Debug.Assert(EnumType.IsValueType);
        EnumAttributes = Attribute.GetCustomAttributes(EnumType);
        IsFlags = EnumAttributes.Any(attr => attr is FlagsAttribute);
        Size = Danger.SizeOf<TEnum>();
        var valueField = EnumType.GetFields(Reflect.InstanceFlags).OneOrDefault(null);
        Debug.Assert(valueField is not null);
        UnderlyingType = valueField.FieldType;
        Debug.Assert(UnderlyingType.EqualsAny(typeof(sbyte), typeof(byte), 
                                              typeof(short), typeof(ushort), 
                                              typeof(int), typeof(uint), 
                                              typeof(long), typeof(ulong)));
        var enumFields = EnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        var count = enumFields.Length;
        _definedMembers = new(count);
        for (var i = 0; i < count; i++)
        {
            var field = enumFields[i];
            var @enum = (TEnum)(field.GetValue(null)!);
            var wrapper = new EnumWrapper<TEnum>
            {
                Enum = @enum,
                Name = field.Name,
                Attributes = Attribute.GetCustomAttributes(field),
            };
            _definedMembers.Add(@enum, wrapper);
        }

        EqualityComparer = new FuncEqualityComparer<TEnum>(Equals, GetHashCode);
        Comparer = new FuncComparer<TEnum>(Compare);
    }

    public static bool Equals(TEnum x, TEnum y)
    {
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Ceq();
        return Return<bool>();
    }

    public static int GetHashCode(TEnum e)
    {
        if (Size <= 4)
        {
            Emit.Ldarg(nameof(e));
            Emit.Conv_I4();
            Emit.Ret();
            throw Unreachable();
        }
        else
        {
            Emit.Ldarg(nameof(e));
            Emit.Conv_I4();
            Emit.Ldarg(nameof(e));
            Emit.Ldc_I4(32);
            Emit.Shr();
            Emit.Xor();
            Emit.Ret();
            throw Unreachable();
        }
    }

    public static int Compare(TEnum x, TEnum y)
    {
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Bge("greater_or_equal");
        Emit.Ldc_I4_M1();
        Emit.Ret();
        MarkLabel("greater_or_equal");
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Beq("equal");
        Emit.Ldc_I4_1();
        Emit.Ret();
        MarkLabel("equal");
        Emit.Ldc_I4_0();
        Emit.Ret();
        throw Unreachable();
    }

    public static string GetName(TEnum e)
    {
        if (_definedMembers.TryGetValue(e, out var wrapper))
            return wrapper!.Name;
        Debugger.Break();
        throw new NotImplementedException();
    }
}


public partial class EnumWrapper<TEnum> : IEquatable<EnumWrapper<TEnum>>, IComparable<EnumWrapper<TEnum>>,
                                  IEquatable<TEnum>, IComparable<TEnum>,
                                  IConvertible,
                                  ISpanFormattable, IFormattable
    where TEnum : struct, Enum
{
    public TEnum Enum { get; internal init; }
    public string Name { get; internal init;}
    public IReadOnlyList<Attribute> Attributes { get; internal init; }

    internal EnumWrapper()
    {
       
    }


    public bool Equals(EnumWrapper<TEnum>? other)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(EnumWrapper<TEnum>? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(TEnum other)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(TEnum other)
    {
        throw new NotImplementedException();
    }

    public TypeCode GetTypeCode()
    {
        throw new NotImplementedException();
    }

    public bool ToBoolean(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public byte ToByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public char ToChar(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public DateTime ToDateTime(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public decimal ToDecimal(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public double ToDouble(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public short ToInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public int ToInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public long ToInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public sbyte ToSByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public float ToSingle(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public string ToString(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public object ToType(Type conversionType, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public ushort ToUInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public uint ToUInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public ulong ToUInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        throw new NotImplementedException();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }
}