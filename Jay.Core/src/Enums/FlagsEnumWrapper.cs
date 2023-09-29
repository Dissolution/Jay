namespace Jay.Enums;

public readonly struct FlagsEnumWrapper<TEnum> : 
    IEquatable<TEnum>, IEquatable<FlagsEnumWrapper<TEnum>>,
    IComparable<TEnum>, IComparable<FlagsEnumWrapper<TEnum>>,
    IFormattable
#if NET7_0_OR_GREATER
    , IEqualityOperators<FlagsEnumWrapper<TEnum>, TEnum, bool>
    , IComparisonOperators<FlagsEnumWrapper<TEnum>, TEnum, bool>
    , IBitwiseOperators<FlagsEnumWrapper<TEnum>, TEnum, TEnum>
    , ISpanParsable<FlagsEnumWrapper<TEnum>>
    , IParsable<FlagsEnumWrapper<TEnum>>
    , ISpanFormattable
#endif
    where TEnum : struct, Enum
{
    public static implicit operator TEnum(FlagsEnumWrapper<TEnum> enumWrapper) => enumWrapper._enum;

    public static explicit operator FlagsEnumWrapper<TEnum>(TEnum @enum) => new(@enum);

    public static bool operator ==(FlagsEnumWrapper<TEnum> left, TEnum right) => EnumExtensions.Equal(left._enum, right);

    public static bool operator !=(FlagsEnumWrapper<TEnum> left, TEnum right) => EnumExtensions.NotEqual(left._enum, right);

    public static bool operator >(FlagsEnumWrapper<TEnum> left, TEnum right) => EnumExtensions.GreaterThan(left._enum, right);

    public static bool operator >=(FlagsEnumWrapper<TEnum> left, TEnum right) => EnumExtensions.GreaterThanOrEqual(left._enum, right);

    public static bool operator <(FlagsEnumWrapper<TEnum> left, TEnum right) => EnumExtensions.LessThan(left._enum, right);

    public static bool operator <=(FlagsEnumWrapper<TEnum> left, TEnum right) => EnumExtensions.LessThanOrEqual(left._enum, right);

    public static TEnum operator &(FlagsEnumWrapper<TEnum> left, TEnum right) => FlagsEnumExtensions.And(left._enum, right);

    public static TEnum operator |(FlagsEnumWrapper<TEnum> left, TEnum right) => FlagsEnumExtensions.Or(left._enum, right);

    public static TEnum operator ^(FlagsEnumWrapper<TEnum> left, TEnum right) => FlagsEnumExtensions.Xor(left._enum, right);

    public static TEnum operator ~(FlagsEnumWrapper<TEnum> value) => FlagsEnumExtensions.Not(value._enum);

#if NET7_0_OR_GREATER
    static FlagsEnumWrapper<TEnum> ISpanParsable<FlagsEnumWrapper<TEnum>>.Parse(
        ReadOnlySpan<char> text,
        IFormatProvider? _)
    {
        if (TryParse(text, out var value))
            return value;
        throw new ArgumentException($"Cannot parse '{text}' to a {typeof(FlagsEnumWrapper<TEnum>)}", nameof(text));
    }

    static FlagsEnumWrapper<TEnum> IParsable<FlagsEnumWrapper<TEnum>>.Parse([AllowNull, NotNull] string? str, IFormatProvider? _)
    {
        if (TryParse(str, out var value))
            return value;
        throw new ArgumentException($"Cannot parse '{str}' to a {typeof(FlagsEnumWrapper<TEnum>)}", nameof(str));
    }

    static bool ISpanParsable<FlagsEnumWrapper<TEnum>>.TryParse(
        ReadOnlySpan<char> text,
        IFormatProvider? _,
        out FlagsEnumWrapper<TEnum> enumWrapper) => TryParse(text, out enumWrapper);

    static bool IParsable<FlagsEnumWrapper<TEnum>>.TryParse(
        [AllowNull, NotNullWhen(true)] string? str,
        IFormatProvider? _,
        out FlagsEnumWrapper<TEnum> enumWrapper) => TryParse(str, out enumWrapper);


    public static bool TryParse(ReadOnlySpan<char> text, out FlagsEnumWrapper<TEnum> enumWrapper)
    {
        if (Enum.TryParse<TEnum>(
            text,
            true,
            out var @enum))
        {
            enumWrapper = new(@enum);
            return true;
        }

        //TODO: Other parsing!

        enumWrapper = default;
        return false;
    }
#else
    public static bool TryParse(ReadOnlySpan<char> text, out FlagsEnumWrapper<TEnum> enumWrapper)
    {
        if (Enum.TryParse<TEnum>(
            text.ToString(),        // Have to allocate
            true,
            out var @enum))
        {
            enumWrapper = new(@enum);
            return true;
        }

        //TODO: Other parsing!

        enumWrapper = default;
        return false;
    }
#endif

    public static bool TryParse(
        [AllowNull, NotNullWhen(true)] string? str,
        out FlagsEnumWrapper<TEnum> enumWrapper) => TryParse(str.AsSpan(), out enumWrapper);


    private readonly TEnum _enum;

    public bool IsDefault => EnumExtensions.IsDefault(_enum);

    public FlagsEnumWrapper(TEnum flagsEnum)
    {
        if (!typeof(TEnum).HasAttribute<FlagsAttribute>())
            throw new ArgumentException("You must pass an enum with [Flags] applied to it", nameof(flagsEnum));
        _enum = flagsEnum;
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

    public TEnum WithFlag(TEnum flag)
    {
        return FlagsEnumExtensions.WithFlag(_enum, flag);
    }

    public TEnum WithoutFlag(TEnum flag)
    {
        return FlagsEnumExtensions.WithoutFlag(_enum, flag);
    }

    public int CompareTo(FlagsEnumWrapper<TEnum> enumWrapper)
    {
        return EnumExtensions.CompareTo(_enum, enumWrapper._enum);
    }

    public int CompareTo(TEnum @enum)
    {
        return EnumExtensions.CompareTo(_enum, @enum);
    }

    public bool Equals(FlagsEnumWrapper<TEnum> enumWrapper)
    {
        return EnumExtensions.Equal(_enum, enumWrapper._enum);
    }

    public bool Equals(TEnum @enum)
    {
        return EnumExtensions.Equal(_enum, @enum);
    }

    public override bool Equals(object? obj)
    {
        if (obj is TEnum @enum)
            return Equals(@enum);
        if (obj is FlagsEnumWrapper<TEnum> enumWrapper)
            return Equals(enumWrapper);
        // TODO: Check for int/string?
        return false;
    }

    public override int GetHashCode()
    {
        return _enum.GetHashCode();
    }

    public bool TryFormat(
        Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        throw new NotImplementedException();
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