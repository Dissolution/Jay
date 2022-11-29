namespace Jay.Reflection.Enums;

public abstract class EnumLike<TEnum> :
    IEquatable<TEnum>,
    IComparable<TEnum>, IComparable,
    IFormattable
    where TEnum : EnumLike<TEnum>
{
    public static bool operator ==(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.Equals(right);
    public static bool operator !=(EnumLike<TEnum> left, EnumLike<TEnum> right) => !left.Equals(right);
    public static bool operator <(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.CompareTo((TEnum)right) < 0;
    public static bool operator <=(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.CompareTo((TEnum)right) <= 0;
    public static bool operator >(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.CompareTo((TEnum)right) > 0;
    public static bool operator >=(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.CompareTo((TEnum)right) >= 0;

    public static bool operator ==(EnumLike<TEnum> left, TEnum right) => left.Equals(right);
    public static bool operator !=(EnumLike<TEnum> left, TEnum right) => !left.Equals(right);
    public static bool operator <(EnumLike<TEnum> left, TEnum right) => left.CompareTo(right) < 0;
    public static bool operator <=(EnumLike<TEnum> left, TEnum right) => left.CompareTo(right) <= 0;
    public static bool operator >(EnumLike<TEnum> left, TEnum right) => left.CompareTo(right) > 0;
    public static bool operator >=(EnumLike<TEnum> left, TEnum right) => left.CompareTo(right) >= 0;

    protected static readonly List<TEnum> _members;

    public static bool HasFlags { get; } = false;
    public static IReadOnlyList<Attribute> Attributes { get; } = Attribute.GetCustomAttributes(typeof(TEnum));
    public static IReadOnlyList<TEnum> Members => _members;

    static EnumLike()
    {
        _members = new List<TEnum>();
    }

    public static bool TryParse(ulong value, [NotNullWhen(true)] out TEnum? enumLike)
    {
        // _members[x]'s value is x
        if (value <= (ulong)_members.Count)
        {
            enumLike = _members[(int)value];
            return true;
        }

        enumLike = null;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out TEnum? enumLike)
    {
        foreach (var member in _members)
        {
            if (text.Equals(member._name, StringComparison.OrdinalIgnoreCase))
            {
                enumLike = member;
                return true;
            }
        }

        enumLike = null;
        return false;
    }

    protected readonly ulong _value;
    protected readonly string _name;

    public string Name => _name;

    protected EnumLike([CallerMemberName] string memberName = "")
    {
        if (string.IsNullOrWhiteSpace(memberName))
            throw new ArgumentNullException(nameof(memberName));
        _value = (ulong)_members.Count;
        _name = memberName;
        _members.Add((TEnum)this);
    }

    public int CompareTo(TEnum? enumLike)
    {
        if (enumLike is not null)
            return _value.CompareTo(enumLike._value);
        return 1;
    }

    int IComparable.CompareTo(object? obj)
    {
        if (obj is TEnum enumLike)
            return _value.CompareTo(enumLike._value);
        return 1;
    }

    public bool Equals(TEnum? enumLike)
    {
        return enumLike is not null && _value == enumLike._value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is TEnum enumLike)
            return _value == enumLike._value;
        return false;
    }

    public override int GetHashCode()
    {
        return (int)((uint)(_value >> 32) ^ (uint)_value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.enum.format?view=net-6.0"/>
    public virtual string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        if (format is not null && format.Length == 1)
        {
            var ch = format[0];

            /* We aren't flags (in this base), so ignore G + F
            if (ch == 'G' || ch == 'g')
            {
                // We're not Flags
                return _name;
            }

            if (ch == 'F' || ch == 'f')
            {
                // We're not Flags
                return _name;
            }
            */
            if (ch == 'D' || ch == 'd')
            {
                return _value.ToString("D");
            }

            if (ch == 'X' || ch == 'x')
            {
                return _value.ToString("x8");
            }
        }

        // Always fallback to name
        return _name;
    }

    public override string ToString()
    {
        return _name;
    }
}

public sealed class MemberFlags : EnumLike<MemberFlags>
{
    public static MemberFlags Public { get; } = new(BindingFlags.Public);
    public static MemberFlags Internal { get; } = new(BindingFlags.NonPublic);
    public static MemberFlags Protected { get; } = new(BindingFlags.NonPublic);
    public static MemberFlags Private { get; } = new(BindingFlags.NonPublic);

    public BindingFlags BindingFlags { get; }

    private MemberFlags(BindingFlags bindingFlags,
        [CallerMemberName] string memberName = "")
        : base(memberName)
    {
        BindingFlags = bindingFlags;
    }
}