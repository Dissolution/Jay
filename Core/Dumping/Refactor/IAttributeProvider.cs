namespace Jay.Dumping.Refactor;

public interface IAttributeProvider
{
    IReadOnlyList<Attribute> Attributes { get; }

    public bool HasAttribute<TAttribute>() 
        where TAttribute : Attribute
    {
        return Attributes.Any(attr => attr is TAttribute);
    }

    public TAttribute? GetAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return Attributes
            .SelectWhere((Attribute attr, out TAttribute? tAttr) => attr.Is(out tAttr))
            .FirstOrDefault();
    }
}