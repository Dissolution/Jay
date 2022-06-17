using System.Diagnostics;
using System.Reflection;
using InlineIL;
using Jay.Dumping.Refactor;

namespace Jay.Enums;

public sealed class EnumMemberInfo<TEnum> : IEquatable<EnumMemberInfo<TEnum>>, 
                                      IEquatable<TEnum>
    where TEnum : struct, Enum
{
    private readonly TEnum _enum;
    private readonly string _name;
    private readonly Attribute[] _attributes;

    public TEnum Enum => _enum;
    public string Name => _name;
    public IReadOnlyList<Attribute> Attributes => _attributes;

    public EnumMemberInfo(FieldInfo enumMemberField)
    {
        _name = enumMemberField.Name;
        _attributes = Attribute.GetCustomAttributes(enumMemberField, true);
        _enum = (TEnum)enumMemberField.GetValue(null)!;
    }

    public EnumMemberInfo(TEnum flag, string? name = null)
    {
        _enum = flag;
        _name = name ?? flag.ToString();
        _attributes = Array.Empty<Attribute>();
    }

    public TAttribute? GetAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _attributes
            .SelectWhere((Attribute attr, [NotNullWhen(true)] out TAttribute? tAttr) => attr.Is(out tAttr))
            .FirstOrDefault();
    }

    public bool Equals(EnumMemberInfo<TEnum>? enumInfo)
    {
        return enumInfo is not null && EnumTypeInfo<TEnum>.Equals(Enum, enumInfo.Enum);
    }

    public bool Equals(TEnum @enum)
    {
        return EnumComparer<TEnum>.Default.Equals(_enum, @enum);
    }

    public override bool Equals(object? obj)
    {
        if (obj is EnumMemberInfo<TEnum> enumInfo) return Equals(enumInfo);
        if (obj is TEnum @enum) return Equals(@enum);
        if (obj is Enum) Debugger.Break();
        return false;
    }

    public override int GetHashCode() => EnumComparer<TEnum>.Default.GetHashCode(_enum);

    public override string ToString()
    {
        return $"{typeof(TEnum)}.{Name}";
    }
}