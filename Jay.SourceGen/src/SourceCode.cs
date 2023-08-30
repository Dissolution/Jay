using Microsoft.CodeAnalysis.Text;

using System.Text;

namespace Jay.SourceGen;

public readonly record struct FileSourceCode(string FileName, SourceText Code)
{
    public FileSourceCode(string hintName, string code)
        : this(hintName, SourceText.From(code, Encoding.UTF8))
    {

    }
}
