using System.Collections;

namespace Jay.SourceGen.Reflection;

public sealed class SignatureAttributes : IReadOnlyList<AttributeSignature>
{
    private readonly List<AttributeSignature> _attributes;

    public int Count => _attributes.Count;

    public AttributeSignature this[int index] => _attributes[index];

    public SignatureAttributes(ISymbol symbol)
    {
        _attributes = symbol
            .GetAttributes()
            .Select(a => new AttributeSignature(a))
            .ToList();
    }

    public SignatureAttributes(MemberInfo member)
    {
        _attributes = member
            .GetCustomAttributesData()
            .Select(static a => new AttributeSignature(a))
            .ToList();
    }
    
    public bool HasAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        string attributeName = typeof(TAttribute).Name;
        return _attributes.Any(attr => attr.Name == attributeName);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<AttributeSignature> GetEnumerator()
    {
        return _attributes
            .GetEnumerator();
    }

    public override string ToString()
    {
        if (_attributes.Count == 0) return "";

        return TextBuilder.New
            .Append('[')
            .Delimit(", ", _attributes, static (cb, a) => cb.Write(a))
            .Append(']')
            .ToStringAndDispose();
    }
}