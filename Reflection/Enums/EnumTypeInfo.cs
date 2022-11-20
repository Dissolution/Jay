using System.Diagnostics;
using System.Reflection;

namespace Jay.Reflection.Enums;

public abstract class EnumTypeInfo
{
    protected readonly Type _enumType;
    protected readonly Attribute[] _attributes;
    protected readonly bool _hasFlags;
    protected int _memberCount = 0;
    protected FieldInfo[] _enumFields = Array.Empty<FieldInfo>();
    protected string[] _names = Array.Empty<string>();
    protected ulong[] _values = Array.Empty<ulong>();

    public Type EnumType => _enumType;
    public string Name => _enumType.Name;
    public IReadOnlyList<Attribute> Attributes => _attributes;
    public bool HasFlags => _hasFlags;
    public int MemberCount => _memberCount;
    public IReadOnlyList<string> Names => _names;

    internal IReadOnlyList<FieldInfo> EnumFields => _enumFields;
    internal IReadOnlyList<ulong> Values => _values;

    protected EnumTypeInfo(Type enumType)
    {
        Debug.Assert(enumType.IsEnum);
        Debug.Assert(enumType.IsValueType);
        _enumType = enumType;
        _attributes = Attribute.GetCustomAttributes(enumType);
        _hasFlags = _attributes.OfType<FlagsAttribute>().Any();
    }
}