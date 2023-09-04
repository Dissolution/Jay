namespace Jay.SourceGen.CodeBuilding;

public interface ICodePart
{
    void DeclareTo(SourceCodeBuilder code);
}

[AttributeUsage(AttributeTargets.All)]
public class CodePartAttribute : Attribute
{
    public string Code { get; }

    public CodePartAttribute(string code)
    {
        this.Code = code;
    }
}

public static class CodePart
{
    public static string ToDeclaration(this ICodePart codePart)
    {
        return SourceCodeBuilder.New
            .Invoke(codePart.DeclareTo)
            .ToStringAndDispose();
    }
}