namespace Jay.SourceGen.Reflection;

public sealed class Parameters : List<ParameterSignature>
{
    public static Parameters From(ImmutableArray<IParameterSymbol> parameterSymbols)
    {
        var parameters = new Parameters();
        if (!parameterSymbols.IsDefaultOrEmpty)
        {
            parameters.AddRange(parameterSymbols
                    .Select(static p => ParameterSignature.Create(p))
                    .Where(static p => p is not null)!);
        }

        return parameters;
    }
    
    public static Parameters From(ParameterInfo[] parameterInfos)
    {
        var parameters = new Parameters();
        parameters.AddRange(parameterInfos
            .Select(static p => ParameterSignature.Create(p))
            .Where(static p => p is not null)!);
        return parameters;
    }
}