using System.Numerics;
using Jay.Enums;
using Jay.Reflection.Extensions;
// ReSharper disable InvokeAsExtensionMethod

namespace Jay.Reflection;

public struct Dynum<TEnum> :
    IComparisonOperators<Dynum<TEnum>, TEnum, bool>,
    IEqualityOperators<Dynum<TEnum>, TEnum, bool>,
    IBitwiseOperators<Dynum<TEnum>, TEnum, TEnum>,
    IEquatable<TEnum>,
    IComparable<TEnum>,
    ISpanParsable<Dynum<TEnum>>, 
    IParsable<Dynum<TEnum>>,
    ISpanFormattable, IFormattable
    where TEnum : struct, Enum
{
    public static implicit operator Dynum<TEnum>(TEnum @enum) => new Dynum<TEnum>(@enum);
    public static implicit operator TEnum(Dynum<TEnum> dynum) => dynum._enum;

    public static bool operator ==(Dynum<TEnum> left, TEnum right)
    {
        return EnumExtensions.Equal(left._enum, right);
    }
    public static bool operator !=(Dynum<TEnum> left, TEnum right)
    {
        return EnumExtensions.NotEqual(left._enum, right);
    }
    public static bool operator >(Dynum<TEnum> left, TEnum right)
    {
        return EnumExtensions.GreaterThan(left._enum, right);
    }
    public static bool operator >=(Dynum<TEnum> left, TEnum right)
    {
        return EnumExtensions.GreaterThanOrEqual(left._enum, right);
    }
    public static bool operator <(Dynum<TEnum> left, TEnum right)
    {
        return EnumExtensions.LessThan(left._enum, right);
    }
    public static bool operator <=(Dynum<TEnum> left, TEnum right)
    {
        return EnumExtensions.LessThanOrEqual(left._enum, right);
    }
    public static TEnum operator &(Dynum<TEnum> left, TEnum right)
    {
        return FlagsEnumExtensions.And(left._enum, right);
    }
    public static TEnum operator |(Dynum<TEnum> left, TEnum right)
    {
        return FlagsEnumExtensions.Or(left._enum, right);
    }
    public static TEnum operator ^(Dynum<TEnum> left, TEnum right)
    {
        return FlagsEnumExtensions.Xor(left._enum, right);
    }
    public static TEnum operator ~(Dynum<TEnum> value)
    {
        return FlagsEnumExtensions.Not(value._enum);
    }
    
    public static bool IsFlags { get; } = typeof(TEnum).HasAttribute<FlagsAttribute>();

    private TEnum _enum;

    public bool IsDefault => EnumExtensions.IsDefault(_enum);

    public Dynum(TEnum @enum)
    {
        _enum = @enum;
    }

    public bool HasFlag(TEnum flag)
    {
        return FlagsEnumExtensions.HasFlag(_enum, flag);
    }
    
    public bool HasAnyFlags(params TEnum[] flags)
    {
        return FlagsEnumExtensions.HasAnyFlags(_enum, flags);
    }
    
    public bool HasAllFlags(params TEnum[] flags)
    {
        return FlagsEnumExtensions.HasAllFlags(_enum, flags);
    }

    public void AddFlag(TEnum flag)
    {
        FlagsEnumExtensions.AddFlag(ref _enum, flag);
    }
    
    public void RemoveFlag(TEnum flag)
    {
        FlagsEnumExtensions.RemoveFlag(ref _enum, flag);
    }
    
    public static Dynum<TEnum> Parse(string? text, IFormatProvider? provider)
    {
        if (!TryParse((ReadOnlySpan<char>)text, provider, out var result))
            throw new ArgumentException($"Unable to parse \"{text}\" to a {typeof(TEnum)}", nameof(text));
        return result;
    }
    public static bool TryParse(string? text, IFormatProvider? provider, out Dynum<TEnum> result)
    {
        return TryParse((ReadOnlySpan<char>)text, provider, out result);
    }
    public static Dynum<TEnum> Parse(ReadOnlySpan<char> text, IFormatProvider? provider)
    {
        if (!TryParse(text, provider, out var result))
            throw new ArgumentException($"Unable to parse \"{text}\" to a {typeof(TEnum)}", nameof(text));
        return result;
    }
    public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, out Dynum<TEnum> result)
    {
        // TODO: Better implementation
        if (Enum.TryParse<TEnum>(text, true, out TEnum @enum))
        {
            result = @enum;
            return true;
        }
        result = default;
        return false;
    }

    public int CompareTo(TEnum @enum)
    {
        return EnumExtensions.CompareTo(_enum, @enum);
    }
    
    public bool Equals(TEnum @enum)
    {
        return EnumExtensions.Equal(_enum, @enum);
    }

    public override bool Equals(object? obj)
    {
        if (obj is TEnum @enum) return EnumExtensions.Equal(_enum, @enum);
        if (obj is Dynum<TEnum> dynum) return EnumExtensions.Equal(_enum, dynum._enum);
        return false;
    }

    public override int GetHashCode()
    {
        return _enum.GetHashCode();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, 
        ReadOnlySpan<char> format = default, IFormatProvider? _ = default)
    {
        //TODO: Name cache

        string? name = Enum.GetName<TEnum>(_enum);
        
        // TODO: Support for flags

        if (name is null)
        {
            charsWritten = 0;
            return true;
        }

        if (name.TryCopyTo(destination))
        {
            charsWritten = name.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }
    
    public string ToString(string? format, IFormatProvider? _ = default)
    {
        return _enum.ToString(format);
    }

    public override string ToString()
    {
        return _enum.ToString();
    }
}