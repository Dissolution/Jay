namespace Jay.CodeGen.CSharpCode;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class ToCodeAttribute : Attribute
{
    public string? Code { get; init; } = null;

    public Naming Naming { get; init; } = Naming.None;
    
    public ToCodeAttribute() { }
    
    public ToCodeAttribute(string? code)
    {
        this.Code = code;
    }

    public ToCodeAttribute(Naming naming)
    {
        this.Naming = naming;
    }
}