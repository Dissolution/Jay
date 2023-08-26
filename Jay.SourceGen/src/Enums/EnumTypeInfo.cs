using Jay.CodeGen.Attributes;
using Jay.CodeGen.Exceptions;

namespace Jay.CodeGen.Enums;

public abstract class EnumTypeInfo : 
    IEquatable<EnumTypeInfo>, IEquatable<Type>,
    IAttributed
{
    private readonly Attribute[] _attributes;

    public int AttributeCount => _attributes.Length;
    
    public Type Type { get; }
    public string Name => Type.Name;

    protected EnumTypeInfo(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.IsEnum)
            throw Except.New<ArgumentException>($"{type} is not a valid Enum Type", nameof(type));
    }

    public bool HasAttribute<TAttribute>() where TAttribute : Attribute
    {
        throw new NotImplementedException();
    }

    public bool Equals(EnumTypeInfo? enumType)
    {
        throw new NotImplementedException();
    }
    public bool Equals(Type? type)
    {
        throw new NotImplementedException();
    }
}

public class EnumMemberInfo : IAttributed
{
    private readonly Attributed _attributed;
    
    public EnumTypeInfo EnumType { get; }
    
    public string Name { get; }
    public object Value { get; }
    public ulong UInt64Value { get; }
    public FieldInfo Field { get; }

    public int AttributeCount => _attributed.AttributeCount;

    public EnumMemberInfo(EnumTypeInfo enumTypeInfo, FieldInfo fieldInfo)
    {
        this.EnumType = enumTypeInfo;
        this.Field = fieldInfo;
        this.Name = fieldInfo.Name;
        this.Value = fieldInfo.GetValue(null)!;
        this.UInt64Value = (ulong)Value;
    }
    
    public bool HasAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _attributed.HasAttribute<TAttribute>();
    }
}