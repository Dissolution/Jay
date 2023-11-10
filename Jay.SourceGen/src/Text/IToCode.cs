namespace Jay.SourceGen.Text;

/// <summary>
/// This value can be written as C# code to a <see cref="CodeBuilder"/>
/// </summary>
public interface IToCode
{
    /// <summary>
    /// Writes a C# code representation of this value to the given <paramref name="codeBuilder"/>
    /// </summary>
    /// <param name="codeBuilder">The <see cref="CodeBuilder"/> to write to</param>
    /// <returns>
    /// <c>true</c> if we wrote anything; otherwise, <c>false</c>
    /// </returns>
    bool WriteTo(CodeBuilder codeBuilder);
}