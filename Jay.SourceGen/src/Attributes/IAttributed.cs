namespace Jay.CodeGen.Attributes;

public interface IAttributed
{
    int AttributeCount { get; }
    
    bool HasAttribute<TAttribute>()
        where TAttribute : Attribute;
}

internal sealed class Attributed : IAttributed
{
    private readonly Attribute[] _attributes;

    public int AttributeCount => _attributes.Length;

    public Attributed(Attribute[] attributes)
    {
        _attributes = attributes;
    }

    public bool HasAttribute<TAttribute>() where TAttribute : Attribute
    {
        return _attributes.Any(attr => attr.GetType() == typeof(TAttribute));
    }
}