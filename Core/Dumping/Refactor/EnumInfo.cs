using System.Diagnostics;
using System.Reflection;
using InlineIL;

namespace Jay.Dumping.Refactor;

public sealed class EnumInfo<TEnum> : IEquatable<EnumInfo<TEnum>>, 
                                      IEquatable<TEnum>,
                                      IAttributeProvider
    where TEnum : struct, Enum
{
    private readonly TEnum _enum;
    private readonly string _name;
    private readonly Attribute[] _attributes;

    public TEnum Enum => _enum;
    public string Name => _name;
    public IReadOnlyList<Attribute> Attributes => _attributes;

    public EnumInfo(FieldInfo enumMemberField)
    {
        _name = enumMemberField.Name;
        _attributes = Attribute.GetCustomAttributes(enumMemberField, true);
        _enum = (TEnum)enumMemberField.GetValue(null)!;
    }

    public EnumInfo(TEnum flag, string? name = null)
    {
        _enum = flag;
        _name = name ?? flag.ToString();
        _attributes = Array.Empty<Attribute>();
    }

    public TAttribute? GetAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _attributes
            .SelectWhere((Attribute attr, out TAttribute? tAttr) => attr.Is(out tAttr))
            .FirstOrDefault();
    }

    public bool Equals(EnumInfo<TEnum>? enumInfo)
    {
        return enumInfo is not null && EnumTypeInfo<TEnum>.Equals(Enum, enumInfo.Enum);
    }

    public bool Equals(TEnum @enum)
    {
        return EnumTypeInfo<TEnum>.Equals(Enum, @enum);
    }

    public override bool Equals(object? obj)
    {
        if (obj is EnumInfo<TEnum> enumInfo) return Equals(enumInfo);
        if (obj is TEnum @enum) return Equals(@enum);
        if (obj is Enum) Debugger.Break();
        return false;
    }

    public override int GetHashCode()
    {
        IL.Emit.Ldarg_0();
        IL.Emit.Ldfld(FieldRef.Field(typeof(EnumInfo<TEnum>), nameof(_enum)));
        IL.Emit.Conv_I4();
        return IL.Return<int>();
    }

    public override string ToString()
    {
        return $"{EnumTypeInfo<TEnum>.Name}.{Name}";
    }
}