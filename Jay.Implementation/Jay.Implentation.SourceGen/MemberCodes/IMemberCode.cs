namespace IMPL.SourceGen.MemberCodes;

public interface IMemberCode
{
    MemberPos Pos { get; }
    void Write(Implementer implementer, CodeBuilder codeBuilder);
}
