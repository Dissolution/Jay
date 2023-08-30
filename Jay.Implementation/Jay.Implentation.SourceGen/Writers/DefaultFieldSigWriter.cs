namespace IMPL.SourceGen.Writers;

public sealed class DefaultFieldSigWriter : IFieldSigWriter
{
    public static IFieldSigWriter Instance { get; } = new DefaultFieldSigWriter();

    public void Write(FieldSig fieldSig, CodeBuilder codeBuilder)
    {
        codeBuilder
            .Append(fieldSig.Visibility, "lc")
            .AppendIf(fieldSig.Instic == Instic.Instance, " ", " static ")
            .AppendKeywords(fieldSig.Keywords)
            .Append(fieldSig.FieldType)
            .Append(' ')
            .Append(fieldSig.Name)
            .AppendLine(';');
    }
}
