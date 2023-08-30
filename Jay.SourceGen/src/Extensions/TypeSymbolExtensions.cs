namespace Jay.SourceGen.Extensions;

public static class TypeSymbolExtensions
{
    public static bool CanBeNull(this ITypeSymbol typeSymbol)
    {
        return !typeSymbol.IsValueType;
    }

    public static bool HasInterface<TInterface>(this ITypeSymbol type)
       where TInterface : class
    {
        var interfaceType = typeof(TInterface);
        if (!interfaceType.IsInterface)
            throw new ArgumentException("The generic type must be an Interface type", nameof(TInterface));
        var interfaceFullName = interfaceType.FullName;

        return type.AllInterfaces
            .Any(ti => ti.GetFullName() == interfaceFullName);
    }

    public static bool IsType<T>(this ITypeSymbol typeSymbol)        
    {
         return string.Equals(typeSymbol.GetFullName(), typeof(T).FullName);
    }
}
