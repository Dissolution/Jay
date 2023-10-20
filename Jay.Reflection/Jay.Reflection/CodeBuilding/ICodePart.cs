namespace Jay.Reflection.CodeBuilding;

/// <summary>
/// A thing that can be declared as code
/// </summary>
public interface ICodePart
{
    /// <summary>
    /// Declare this <see cref="ICodePart"/> to the <see cref="CodeBuilder"/>
    /// </summary>
    /// <param name="codeBuilder"></param>
    void DeclareTo(CodeBuilder codeBuilder);
}