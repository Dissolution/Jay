﻿using Jay.SourceGen.Reflection;

namespace Jay.SourceGen.Extensions;

public static class AttributeExtensions
{
    public static AttributeArguments GetArguments(this AttributeData attributeData)
    {
        return AttributeArguments.From(attributeData);
    }

    // public static AttributeArguments GetArguments(this CustomAttributeData customAttributeData)
    // {
    //     return new AttributeArguments(customAttributeData);
    // }

    public static string? GetFullTypeName(this AttributeData attributeData)
    {
        return attributeData.AttributeClass?.GetFullName();
    }

    public static IReadOnlyList<AttributeData> GetAttributes(this AttributeListSyntax attributes, Compilation compilation)
    {
        // Collect pertinent syntax trees from these attributes
        var acceptedTrees = new HashSet<SyntaxTree>();
        foreach (var attribute in attributes.Attributes)
        {
            acceptedTrees.Add(attribute.SyntaxTree);
        }

        var parentSymbol = attributes.Parent!.GetDeclaredSymbol(compilation)!;
        var parentAttributes = parentSymbol.GetAttributes();
        var ret = new List<AttributeData>();
        foreach (var attribute in parentAttributes)
        {
            if (acceptedTrees.Contains(attribute.ApplicationSyntaxReference!.SyntaxTree))
                ret.Add(attribute);
        }

        return ret;
    }

    public static AttributeData? FindByFqn(this ImmutableArray<AttributeData> attributes, string? attributeFqn)
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            string? attrFqn = attributes[i].AttributeClass?.GetFullName();
            if (string.Equals(attrFqn, attributeFqn, StringComparison.Ordinal))
                return attributes[i];
        }
        return null;
    }
}
