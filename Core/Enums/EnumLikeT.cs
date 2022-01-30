using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Jay.Dumping;

#pragma warning disable CS0659 // Overrides Equals but not GetHashCode

namespace Jay;

public abstract class EnumLike : IComparable, IFormattable
{
    public static bool operator ==(EnumLike? enumLike, EnumLike? @enum) => Equals(enumLike, @enum);
    public static bool operator !=(EnumLike? enumLike, EnumLike? @enum) => !Equals(enumLike, @enum);
    public static bool operator <(EnumLike? enumLike, EnumLike? @enum) => Compare(enumLike, @enum) < 0;
    public static bool operator <=(EnumLike? enumLike, EnumLike? @enum) => Compare(enumLike, @enum) <= 0;
    public static bool operator >(EnumLike? enumLike, EnumLike? @enum) => Compare(enumLike, @enum) > 0;
    public static bool operator >=(EnumLike? enumLike, EnumLike? @enum) => Compare(enumLike, @enum) >= 0;

    internal static bool Equals(EnumLike? left, EnumLike? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    internal static int Compare(EnumLike? left, EnumLike? right)
    {
        if (left == null) return right == null ? 0 : -1;
        if (right == null) return 1;
        return left.CompareTo(right);
    }


    protected readonly int _value;
    protected readonly string _name;
    
    public abstract bool HasFlags { get; }

    protected EnumLike(int value, string? name)
    {
        _value = value;
        _name = name ?? value.ToString("D");
    }

    public abstract int CompareTo(object? obj);

    public abstract override bool Equals(object? obj);

    public sealed override int GetHashCode()
    {
        return _value;
    }

    public virtual string ToString(string? format, IFormatProvider? _ = null)
    {
        if (format is null) return _name;
        var len = format.Length;
        if (len == 0) return _name;
        if (len == 1)
        {
            var ch = format[0];
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

public abstract class EnumLike<TEnum> : EnumLike, IEquatable<TEnum>, IComparable<TEnum>, IFormattable
    where TEnum : EnumLike<TEnum>
{
    public static bool operator ==(EnumLike<TEnum> enumLike, TEnum? @enum) => enumLike.Equals(@enum);
    public static bool operator !=(EnumLike<TEnum> enumLike, TEnum? @enum) => !enumLike.Equals(@enum);
    public static bool operator <(EnumLike<TEnum> enumLike, TEnum? @enum) => enumLike.CompareTo(@enum) < 0;
    public static bool operator <=(EnumLike<TEnum> enumLike, TEnum? @enum) => enumLike.CompareTo(@enum) <= 0;
    public static bool operator >(EnumLike<TEnum> enumLike, TEnum? @enum) => enumLike.CompareTo(@enum) > 0;
    public static bool operator >=(EnumLike<TEnum> enumLike, TEnum? @enum) => enumLike.CompareTo(@enum) >= 0;

    protected static readonly List<TEnum> _members;

    public static TEnum? Default
    {
        get
        {
            if (_members.Count == 0) return default;
            return _members[0];
        }
    }

    public static IReadOnlyList<TEnum> Members => _members;

    static EnumLike()
    {
        _members = new List<TEnum>();
    }

    public static bool TryParse(int value, [NotNullWhen(true)] out TEnum? @enum)
    {
        if (value < _members.Count)
        {
            @enum = _members[value];
            return true;
        }
        else
        {
            @enum = null;
            return false;
        }
    }

    public static bool TryParse(string name, [NotNullWhen(true)] out TEnum? @enum)
    {
        foreach (var member in _members)
        {
            if (string.Equals(member._name, name, StringComparison.OrdinalIgnoreCase))
            {
                @enum = member;
                return true;
            }
        }

        @enum = null;
        return false;
    }
    
    public override bool HasFlags => false;
    
    protected EnumLike([CallerMemberName] string name = "")
        : base(_members.Count, name)
    {
        _members.Add((TEnum)this);
    }

    public virtual int CompareTo(TEnum? @enum)
    {
        // NonNull > null
        if (@enum is null) return 1;
        return _value.CompareTo(@enum._value);
    }

    public sealed override int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is TEnum @enum)
            return CompareTo(@enum);
        throw Dump.GetException<ArgumentException>($"{obj.GetType()} is not a valid {typeof(TEnum)}", nameof(obj));
    }

    public virtual bool Equals(TEnum? @enum)
    {
        if (@enum is null) return false;
        return _value == @enum._value;
    }

    public sealed override bool Equals(object? obj)
    {
        if (obj is TEnum enumLike)
            return Equals(enumLike);
        return false;
    }
}