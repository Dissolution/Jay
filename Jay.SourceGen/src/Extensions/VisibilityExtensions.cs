using Jay.Reflection;
using Jay.SourceGen.Reflection;

namespace Jay.SourceGen.Extensions;

public static class VisibilityExtensions
{
    public static Visibility ToVisibility(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.NotApplicable => default,
            Accessibility.Private => Visibility.Private,
            Accessibility.ProtectedAndInternal => Visibility.Protected | Visibility.Internal,
            Accessibility.Protected => Visibility.Protected,
            Accessibility.Internal => Visibility.Internal,
            Accessibility.ProtectedOrInternal => Visibility.Protected | Visibility.Internal,
            Accessibility.Public => Visibility.Public,
            _ => throw new ArgumentException("Invalid Accessibility", nameof(accessibility)),
        };
    }

    public static Visibility GetVisibility(this ISymbol? symbol)
    {
        if (symbol is null) return default;
        var visibility = symbol.DeclaredAccessibility.ToVisibility();
        if (symbol.IsStatic)
            visibility |= Visibility.Static;
        else
            visibility |= Visibility.Instance;
        return visibility;
    }

    public static void WriteTo(this Visibility visibility, CodeBuilder codeBuilder)
    {
        if (visibility.HasFlag(Visibility.Static))
            codeBuilder.Append("static ");
        if (visibility.HasFlag(Visibility.Private))
            codeBuilder.Append("private ");
        if (visibility.HasFlag(Visibility.Protected))
            codeBuilder.Append("protected ");
        if (visibility.HasFlag(Visibility.Internal))
            codeBuilder.Append("internal ");
        if (visibility.HasFlag(Visibility.Public))
            codeBuilder.Append("public ");
    }
}