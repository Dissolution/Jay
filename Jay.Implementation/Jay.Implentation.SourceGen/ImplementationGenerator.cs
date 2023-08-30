using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Immutable;
using System.Diagnostics;

namespace IMPL.SourceGen;

[Generator]
public sealed class ImplementationGenerator : IIncrementalGenerator
{
    [Conditional("DEBUG")]
    private static void Log(FormattableString message)
    {
        string? msg;
        if (message.ArgumentCount == 0)
        {
            msg = message.Format;
        }
        else
        {
            var args = message.GetArguments();
            for (var i = 0; i < args.Length; i++)
            {
                object? arg = args[i];
                Type? argType = arg?.GetType();
                // Process???
                // args[i] = arg;
            }
            msg = string.Format(message.Format, args);
        }
        Debug.WriteLine($"{DateTime.Now:t}: {msg}");
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Initial filter for things with our attribute
        var typeDeclarations = context.SyntaxProvider
             .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: ImplementAttributeUtil.FullName,
                predicate: static (syntaxNode, _) => syntaxNode is TypeDeclarationSyntax,
                transform: static (ctx, _) => (TypeDeclarationSyntax)ctx.TargetNode)
             .Where(t => t is not null)!;

        // Combine with Compilation
        var compAndDecl = context.CompilationProvider
            .Combine(typeDeclarations.Collect());

        // Send for further processing
        context.RegisterSourceOutput(compAndDecl,
            static (sourceContext, compAndDecls) => Process(compAndDecls.Left, sourceContext, compAndDecls.Right));
    }

    private static void Process(Compilation compilation,
        SourceProductionContext sourceProductionContext,
        ImmutableArray<TypeDeclarationSyntax> typeDeclarations)
    {
        // If we have nothing to process, exit quickly
        if (typeDeclarations.IsDefaultOrEmpty) return;

#if ATTACH
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
            Log($"Debugger attached");
        }
#endif

        // Get a passable CancellationToken
        var token = sourceProductionContext.CancellationToken;

        // Load our attribute's symbol
        INamedTypeSymbol? attributeSymbol = compilation
            .GetTypesByMetadataName(ImplementAttributeUtil.FullName)
            .FirstOrDefault();
        if (attributeSymbol is null)
        {
            // Cannot!
            throw new InvalidOperationException($"Could not load {nameof(INamedTypeSymbol)} for {ImplementAttributeUtil.FullName}");
        }

        // As per several examples, we need a distinct list or a grouping on SyntaxTree
        // I'm going with System.Text.Json's example

        foreach (var group in typeDeclarations.GroupBy(static td => td.SyntaxTree))
        {
            SyntaxTree syntaxTree = group.Key;
            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
            CompilationUnitSyntax unitSyntax = (syntaxTree.GetRoot(token) as CompilationUnitSyntax)!;

            // Now, process each thing our attribute is on
            foreach (TypeDeclarationSyntax typeDeclaration in typeDeclarations)
            {
                // Type's Symbol
                INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;
                if (typeSymbol is null)
                {
                    throw new InvalidOperationException($"Could not get Declared {nameof(INamedTypeSymbol)} for {nameof(TypeDeclarationSyntax)}");
                }

                // ImplementAttribute
                AttributeData? implementAttributeData = typeSymbol
                    .GetAttributes()
                    .FirstOrDefault(attr => string.Equals(attr.AttributeClass?.GetFullName(), ImplementAttributeUtil.FullName));

                if (implementAttributeData is null)
                {
                    throw new InvalidOperationException($"Could not get find our {ImplementAttributeUtil.FullName}");
                }

                // We have our starting typesig
                var typeSig = new TypeSig(typeSymbol);
                if (typeDeclaration.HasKeyword(SyntaxKind.PartialKeyword))
                    typeSig.Keywords |= Keywords.Partial;

                // Non-interfaces must be partial
                if (typeSig.ObjType != ObjType.Interface)
                {
                    if (!typeSig.Keywords.HasFlag(Keywords.Partial))
                        throw new InvalidOperationException("Source must be partial");
                }

                // Check for specifiers from the ImplementAttribute
                var attrData = implementAttributeData.GetArgs();

                // Name override?
                if (attrData.TryGetValue(nameof(ImplementAttribute.Name), out string? name) && !string.IsNullOrWhiteSpace(name))
                {
                    if (typeSig.ObjType != ObjType.Interface)
                        throw new InvalidOperationException("Name may only be specified on Interfaces");

                    typeSig.Name = name!;
                    var ns = typeSymbol.GetNamespace();
                    if (ns is null)
                        typeSig.FullName = name!;
                    else
                        typeSig.FullName = $"{ns}.{name}";
                }

                // Keywords override?
                if (attrData.TryGetValue(nameof(ImplementAttribute.Declaration), out string? words))
                {
                    if (!ImplementAttributeUtil.TryParseDeclaration(words, out var typewords))
                    {
                        throw new InvalidOperationException("Invalid Keywords");
                    }

                    // Visibility
                    if (typewords.Visibility != default)
                    {
                        // Always override interface
                        if (typeSig.ObjType == ObjType.Interface)
                        {
                            typeSig.Visibility = typewords.Visibility;
                        }
                        // can override others if they didn't specify
                        else if (typeSig.Visibility == default)
                        {
                            typeSig.Visibility = typewords.Visibility;
                        }
                        else
                        {
                            throw new InvalidOperationException("cannot override existing visibility");
                        }
                    }

                    // Instic
                    if (typewords.Instic != default)
                    {
                        // Experimental!
                        throw new NotImplementedException();
                    }

                    // Keywords
                    if (typewords.Keywords != default)
                    {
                        // Always override interface
                        if (typeSig.ObjType == ObjType.Interface)
                        {
                            typeSig.Keywords = typewords.Keywords;
                        }
                        // can override others if they didn't specify
                        else if (typeSig.Keywords == default)
                        {
                            typeSig.Keywords = typewords.Keywords;
                        }
                        else
                        {
                            // Selectively implement!
                            throw new NotImplementedException();
                        }
                    }

                    // Objtype
                    if (typewords.ObjType != default)
                    {
                        // Always override interface
                        if (typeSig.ObjType == ObjType.Interface)
                        {
                            typeSig.ObjType = typewords.ObjType;
                        }
                        // can override others if they didn't specify
                        else if (typeSig.ObjType == default)
                        {
                            typeSig.ObjType = typewords.ObjType;
                        }
                        else
                        {
                            throw new InvalidOperationException("cannot override existing objtype");
                        }
                    }
                }

                // We now have a typesig, let's fill in the rest

                // Members
                var memberSigs = new List<MemberSig>();

                // Interface Types
                var interfaceSymbols = typeSymbol.AllInterfaces;
                var interfaceSigs = new List<TypeSig>();

                // if this type is an interface itself, add it as the 'most complex' (first) type
                if (typeSymbol.TypeKind == TypeKind.Interface)
                {
                    interfaceSigs.Add(new TypeSig(typeSymbol));
                    foreach (var memberSymbol in typeSymbol.GetMembers())
                    {
                        memberSigs.Add(MemberSig.Create(memberSymbol)!);
                    }
                }
                // The rest
                foreach (var interfaceSymbol in interfaceSymbols)
                {
                    interfaceSigs.Add(new TypeSig(interfaceSymbol));
                    foreach (var memberSymbol in interfaceSymbol.GetMembers())
                    {
                        memberSigs.Add(MemberSig.Create(memberSymbol)!);
                    }
                }

                // base types
                var baseTypeSigs = new List<TypeSig>();
                var baseTypeSymbol = typeSymbol.BaseType;
                while (baseTypeSymbol is not null)
                {
                    baseTypeSigs.Add(new TypeSig(baseTypeSymbol));
                    baseTypeSymbol = baseTypeSymbol.BaseType;
                }

                // If we started with an interface
                if (typeSymbol.TypeKind == TypeKind.Interface)
                {
                    // Clean up 'abstract' off of everything's keywords
                    memberSigs.ForEach(s => s.Keywords &= ~Keywords.Abstract);
                }

                // If we are still building an interface, we'll change it!
                if (typeSig.ObjType == ObjType.Interface)
                {
                    typeSig.ObjType = ObjType.Class;
                    typeSig.Keywords &= ~Keywords.Abstract;
                    if (typeSig.Name == typeSymbol.Name)
                    {
                        typeSig.Name = typeSig.Name[1..];
                    }
                }

                // Build our spec
                var implSpec = new ImplSpec
                {
                    ImplType = typeSig,
                    InterfaceTypes = interfaceSigs,
                    Members = memberSigs,
                };

                // Create the implementation
                var implementation = new Implementer(implSpec);

                SourceCode sourceCode = implementation.Implement();

                // Add it to the source output
                sourceProductionContext.AddSource(sourceCode.FileName, sourceCode.Code);
            }
        }
    }
}




