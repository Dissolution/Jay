namespace Jay.SourceGen.Reflection;

public abstract record class Signature : IToCode
{
    public string? Name { get; set; } = null;
    public Visibility Visibility { get; set; } = Visibility.None;
    public Keywords Keywords { get; set; } = new();
    public Attributes Attributes { get; set; } = new();

    public virtual bool WriteTo(CodeBuilder codeBuilder)
    {
        return codeBuilder.Wrote(cb => cb
            .IfAppend(Visibility, ' ')
            .IfAppend(Keywords, ' ')
            .Append(Name));
    }
}