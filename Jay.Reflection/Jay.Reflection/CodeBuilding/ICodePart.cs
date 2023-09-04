namespace Jay.Reflection.CodeBuilding;

/// <summary>
/// A thing that can be declared as code
/// </summary>
public interface ICodePart
{
    /// <summary>
    /// Declare this thing to the <see cref="CodeBuilder"/>
    /// </summary>
    /// <param name="code"></param>
    void DeclareTo(CodeBuilder code);
}