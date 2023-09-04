using System.Collections;

namespace Jay.SourceGen.Reflection;

public sealed class SignatureAttributes : IReadOnlyList<AttributeSignature>, ICodePart
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

    public void DeclareTo(SourceCodeBuilder code)
    {
        if (_attributes.Count == 0) return;

        code.Write('[')
            .Delimit(", ", _attributes, static (cb, a) => a.DeclareTo(cb))
            .Write(']');
    }

    public override string ToString() => this.ToDeclaration();
}