namespace Jay.Reflection.CodeBuilding;

[AttributeUsage(AttributeTargets.All)]
public class CodePartAttribute : Attribute
{
    public string Code { get; }

    public CodePartAttribute(string code)
    {
        this.Code = code;
    }
}