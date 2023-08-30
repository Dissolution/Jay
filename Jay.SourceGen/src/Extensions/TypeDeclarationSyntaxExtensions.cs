using Microsoft.CodeAnalysis.CSharp;

namespace Jay.SourceGen.Extensions;

public static class TypeDeclarationSyntaxExtensions
{
    public static bool HasKeyword(
        this TypeDeclarationSyntax typeDeclarationSyntax,
        SyntaxKind keyword)
    {
        return typeDeclarationSyntax.Modifiers.Any(m => m.IsKind(keyword));
    }

    public static IEnumerable<UsingDirectiveSyntax> GetImports(
        this TypeDeclarationSyntax typeDeclaration)
    {
        var root = typeDeclaration.SyntaxTree.GetRoot();
        if (root is CompilationUnitSyntax cus)
            return cus.Usings;
        return Enumerable.Empty<UsingDirectiveSyntax>();
    }
    
    public static bool IsPartial(this TypeDeclarationSyntax declaration) =>
        declaration.Modifiers.Any(SyntaxKind.PartialKeyword);
}