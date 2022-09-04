using System.Diagnostics;
using System.Reflection;
using Jay.Reflection;

namespace Jay.Enums;

public abstract class EnumTypeInfo
{
    protected readonly Type _enumType;
    protected readonly Attribute[] _attributes;
    protected readonly int _memberCount;
    protected readonly FieldInfo[] _enumFields;
    protected readonly string[] _names;
    protected readonly ulong[] _values;

    public Type EnumType => _enumType;
    public string Name => _enumType.Name;
    public IReadOnlyList<Attribute> Attributes => _attributes;
    public bool HasFlags { get; }
    public int MemberCount => _values.Length;
    public IReadOnlyList<string> Names => _names;

    protected EnumTypeInfo(Type enumType)
    {
        Debug.Assert(enumType.IsEnum);
        _enumType = enumType;
        _attributes = Attribute.GetCustomAttributes(enumType);
        HasFlags = _attributes.OfType<FlagsAttribute>().Any();
        var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        int len = fields.Length;
        _memberCount = len;
        var names = new string[len];
        var values = new ulong[len];
        for (var i = 0; i < len; i++)
        {
            var field = fields[i];
            names[i] = field.Name;
            values[i] = field.GetStaticValue<ulong>();
        }
        _enumFields = fields;
        _names = names;
        _values = values;
    }
}