using System.Diagnostics;
using System.Reflection;
using Jay.Text;
using DumpAsAttribute = Jay.Dumping.DumpAsAttribute;
using IDumpable = Jay.Dumping.IDumpable;

namespace Jay.Enums;

public sealed class EnumMemberInfo<TEnum> : IEquatable<EnumMemberInfo<TEnum>>, 
                                      IEquatable<TEnum>,
                                      IDumpable
    where TEnum : struct, Enum
{
    public static implicit operator TEnum(EnumMemberInfo<TEnum> enumMemberInfo) => enumMemberInfo.Enum;
    public static implicit operator EnumMemberInfo<TEnum>(TEnum @enum) => EnumInfo.For<TEnum>(@enum);
    public static bool operator ==(EnumMemberInfo<TEnum> x, EnumMemberInfo<TEnum> y) => x.Equals(y);
    public static bool operator !=(EnumMemberInfo<TEnum> x, EnumMemberInfo<TEnum> y) => !(x == y);

    private readonly TEnum _enum;
    private readonly string _name;
    private readonly Attribute[] _attributes;
    private readonly string _dump;

    public TEnum Enum => _enum;
    public string Name => _name;
    public IReadOnlyList<Attribute> Attributes => _attributes;

    public EnumMemberInfo(FieldInfo enumMemberField)
    {
        _name = enumMemberField.Name;
        _attributes = Attribute.GetCustomAttributes(enumMemberField, true);
        _enum = (TEnum)enumMemberField.GetValue(null)!;
        var dumpAttr = GetAttribute<DumpAsAttribute>();
        if (dumpAttr is not null)
        {
            _dump = dumpAttr.DumpAs;
        }
        else
        {
            _dump = _name;
        }
    }

    public EnumMemberInfo(TEnum flag, string? name = null)
    {
        _enum = flag;
        _name = name ?? flag.ToString();
        _attributes = Array.Empty<Attribute>();
        _dump = _name;
    }

    public TAttribute? GetAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _attributes
            .SelectWhere((Attribute attr, [NotNullWhen(true)] out TAttribute? tAttr) => attr.Is(out tAttr))
            .FirstOrDefault();
    }

    public bool Equals(EnumMemberInfo<TEnum>? enumInfo)
    {
        return enumInfo is not null && 
               EnumComparer<TEnum>.Default.Equals(Enum, enumInfo.Enum);
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

    /// <inheritdoc />
    public void DumpTo(TextBuilder text)
    {
        var attr = GetAttribute<DumpAsAttribute>();
        if (attr is not null)
        {
            text.Write(attr.ToString());
        }
        else
        {
            text.Write(Name);
        }
    }

    public override string ToString()
    {
        return $"{typeof(TEnum)}.{Name}";
    }
}