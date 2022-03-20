using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using InlineIL;
using Jay.Reflection;
using Jay.Text;

namespace Jay.Enums;

public abstract class Enumeration : IEquatable<Enumeration>
{
    public static bool operator ==(Enumeration @enum, long id) => @enum._id == id;
    public static bool operator !=(Enumeration @enum, long id) => @enum._id != id;

    public static EnumWrapper<TEnum> Wrap<TEnum>(TEnum @enum)
        where TEnum : struct, Enum
    {
        return EnumWrapper<TEnum>.Wrap(@enum);
    }

    protected readonly long _id;
    protected readonly string _name;
    
    protected Enumeration(long id, string? name)
    {
        _id = id;
        // ReSharper disable once VirtualMemberCallInConstructor
        _name = string.IsNullOrWhiteSpace(name) ? CreateName() : name;
    }

    protected virtual string CreateName() => $"{GetType().Name}_{_id}";

    public bool Equals(long id) => _id == id;

    public bool Equals(Enumeration? enumeration)
    {
        return enumeration is not null &&
               enumeration.GetType() == this.GetType() &&
               enumeration._id == _id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Enumeration enumeration)
            return Equals(enumeration);
        if (obj is long id64)
            return _id == id64;
        if (obj is int id32)
            return _id == id32;
        return false;
    }

    public sealed override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public override string ToString()
    {
        return _name;
    }
}

public abstract class Enumeration<TSelf> : Enumeration, IEquatable<TSelf>
    where TSelf : Enumeration<TSelf>
{
    public static bool operator ==(Enumeration<TSelf> left, TSelf right) => left.Equals(right);
    public static bool operator !=(Enumeration<TSelf> left, TSelf right) => !left.Equals(right);

    protected static readonly List<TSelf> _members;

    public static IReadOnlyList<TSelf> Members => _members;

    static Enumeration()
    {
        /* Enumeration member values must be declared:
         * public static TSelf MemberName { get; }
         *
         */
        _members = typeof(TSelf).GetProperties(BindingFlags.Public | BindingFlags.Static)
                                .Where(property => property.PropertyType.Implements<TSelf>() &&
                                                   property.CanRead)
                                .Select(property => (property.GetValue(null) as TSelf)!)
                                .ToList();
    }

    public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out TSelf? enumeration)
    {
        if (long.TryParse(text, out long id))
        {
            enumeration = _members.FirstOrDefault(member => member._id == id);
            return enumeration is not null;
        }

        foreach (var member in _members)
        {
            if (TextHelper.Equals(member._name, text, StringComparison.OrdinalIgnoreCase))
            {
                enumeration = member;
                return true;
            }
        }
        enumeration = null;
        return false;
    }

    protected Enumeration(long id, [CallerMemberName] string? name = "")
        : base(id, name) { }
    protected Enumeration([CallerMemberName] string? name = "")
        : base(_members.Count, name) { }

    public bool Equals(TSelf? enumeration)
    {
        return enumeration is not null &&
               enumeration._id == _id;
    }

#pragma warning disable CS0659
    public override bool Equals(object? obj)
#pragma warning restore CS0659
    {
        if (obj is TSelf enumeration) return enumeration._id == _id;
        return base.Equals(obj);
    }
}

public class EnumWrapper<TEnum> : Enumeration<EnumWrapper<TEnum>>, IEquatable<TEnum>
    where TEnum : struct, Enum
{
    public static implicit operator EnumWrapper<TEnum>(TEnum @enum) => Wrap(@enum);
    public static implicit operator TEnum(EnumWrapper<TEnum> @enum) => @enum._enum;

    public static bool IsFlags { get; }
    
    static EnumWrapper()
    {
        IsFlags = typeof(TEnum).HasAttribute<FlagsAttribute>();
        Debug.Assert(_members.Count == 0);
        var fields = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var member = new EnumWrapper<TEnum>((TEnum)field.GetValue(null)!);
            _members.Add(member);
        }
    }
    
    protected static long GetId(TEnum @enum)
    {
        IL.Emit.Ldarg(nameof(@enum));
        IL.Emit.Conv_U8();
        return IL.Return<long>();
    }

    protected static bool Equals(TEnum left, TEnum right)
    {
        IL.Emit.Ldarg(nameof(left));
        IL.Emit.Ldarg(nameof(right));
        IL.Emit.Ceq();
        return IL.Return<bool>();
    }

    protected static TEnum[] GetFlags(TEnum e)
    {
        var id = GetId(e);
        var flags = new TEnum[BitOperations.PopCount((ulong)id)];
        int f = 0;
        foreach (var member in _members)
        {
            if (e.HasFlag(member._enum))
            {
                flags[f++] = member._enum;
            }
        }
        Debug.Assert(f == flags.Length);
        return flags;
    }
    
    public static EnumWrapper<TEnum> Wrap(TEnum @enum)
    {
        if (IsFlags)
        {
            return new EnumWrapper<TEnum>(@enum);
        }
        else
        {
            foreach (var member in _members)
            {
                if (Equals(member._enum, @enum))
                    return member;
            }

            throw new InvalidOperationException();
        }
    }
    
    protected TEnum _enum;

    protected EnumWrapper(TEnum @enum)
        : base(GetId(@enum), Enum.GetName(@enum))
    {
        _enum = @enum;
    }

    public bool Equals(TEnum @enum)
    {
        return Equals(@enum, _enum);
    }

    public override bool Equals(object? obj)
    {
        if (obj is TEnum @enum) return Equals(@enum, _enum);
        return base.Equals(obj);
    }
}