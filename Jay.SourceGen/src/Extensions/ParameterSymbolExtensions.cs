namespace Jay.SourceGen.Extensions
{
    public static class ParameterSymbolExtensions
    {
        public static bool IsType<T>(this IParameterSymbol parameterSymbol)
        {
            return parameterSymbol.Type.GetFullName() == typeof(T).FullName;
        }
    }
}
