namespace Jay.SourceGen.Extensions;

public static class ParameterInfoExtensions
{
    public static RefKind RefKind(this ParameterInfo parameterInfo)
    {
        if (parameterInfo.ParameterType.IsByRef)
        {
            if (parameterInfo.IsIn)
                return Microsoft.CodeAnalysis.RefKind.In;
            if (parameterInfo.IsOut)
                return Microsoft.CodeAnalysis.RefKind.Out;
            return Microsoft.CodeAnalysis.RefKind.Ref;
        }
        return Microsoft.CodeAnalysis.RefKind.None;
    }
}