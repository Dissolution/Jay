namespace Jay.CodeGen.Enums;

[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public class EnumToCodeAttribute : ToCodeAttribute
{
    public string? Delimiter { get; init; } = null;

    public EnumToCodeAttribute()
    {
    }
    public EnumToCodeAttribute(string? code) : base(code)
    {
    }
    public EnumToCodeAttribute(Naming naming) : base(naming)
    {
    }
}