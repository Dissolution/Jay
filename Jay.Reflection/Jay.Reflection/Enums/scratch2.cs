namespace Jay.Reflection.Enums;

public class EnumTypeInfo
{
    public Type EnumType { get; }
    public IReadOnlyCollection<Attribute> Attributes { get; }
    public bool HasFlagsAttribute { get; }
    
    private readonly ulong[] _memberValues;
    private readonly string[] _memberNames;
    private readonly Attribute[][] _memberAttributes;

    public EnumTypeInfo(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException($"{enumType.NameOf()} is not a valid enum type", nameof(enumType));

        this.EnumType = enumType;
        this.Attributes = Attribute.GetCustomAttributes(enumType);
        this.HasFlagsAttribute = Attributes.Any(attr => attr is FlagsAttribute);
        
        
        var memberFields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        int count = memberFields.Length;
        _memberValues = new ulong[count];
        _memberNames = new string[count];
        _memberAttributes = new Attribute[count][];
        for (var i = 0; i < count; i++)
        {
            var field = memberFields[i];
            _memberNames[i] = field.Name;
            object member = field.GetValue(null).ThrowIfNull();
            _memberValues[i] = (ulong)member;
            _memberAttributes[i] = Attribute.GetCustomAttributes(field);
        }

    }
    
}