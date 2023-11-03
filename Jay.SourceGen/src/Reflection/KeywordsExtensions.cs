// namespace Jay.SourceGen.Reflection;
//
// public static class KeywordsExtensions
// {
//     public static Keywords GetKeywords(this ISymbol? symbol)
//     {
//         Keywords keywords = default;
//          if (symbol is null)
//              return keywords;
//         if (symbol.IsAbstract)
//             keywords.AddFlag(Keywords.Abstract);
//         if (symbol.IsSealed)
//             keywords.AddFlag(Keywords.Sealed);
//         if (symbol.IsVirtual)
//             keywords.AddFlag(Keywords.Virtual);
//         if (symbol.IsExtern)
//             keywords.AddFlag(Keywords.Extern);
//         if (symbol.IsOverride)
//             keywords.AddFlag(Keywords.Override);a
//         switch (symbol)
//         {
//             case IFieldSymbol fieldSymbol:
//             {
//                 if (fieldSymbol.IsConst)
//                     keywords.AddFlag(Keywords.Const);
//                 if (fieldSymbol.IsRequired)
//                     keywords.AddFlag(Keywords.Required);
//                 if (fieldSymbol.IsVolatile)
//                     keywords.AddFlag(Keywords.Volatile);
//                 if (fieldSymbol.IsReadOnly)
//                     keywords.AddFlag(Keywords.Readonly);
//                 return keywords;
//             }
//             case IMethodSymbol methodSymbol:
//             {
//                 if (methodSymbol.IsInitOnly)
//                     keywords.AddFlag(Keywords.Init);
//                 if (methodSymbol.IsReadOnly)
//                     keywords.AddFlag(Keywords.Readonly);
//                 if (methodSymbol.IsAsync)
//                     keywords.AddFlag(Keywords.Async);
//                 return keywords;
//             }
//             case IEventSymbol eventSymbol:
//             {
//                 keywords.AddFlag(GetKeywords(eventSymbol.AddMethod));
//                 keywords.AddFlag(GetKeywords(eventSymbol.RemoveMethod));
//                 keywords.AddFlag(GetKeywords(eventSymbol.RaiseMethod));
//                 return keywords;
//             }
//             case IPropertySymbol propertySymbol:
//             {
//                 keywords.AddFlag(GetKeywords(propertySymbol.GetMethod));
//                 keywords.AddFlag(GetKeywords(propertySymbol.SetMethod));
//                 return keywords;
//             }
//             case ITypeSymbol typeSymbol:
//             {
//                 if (typeSymbol.IsReadOnly)
//                     keywords.AddFlag(Keywords.Readonly);
//                 return keywords;
//             }
//             default:
//                 return keywords;
//         }
//     }
// }