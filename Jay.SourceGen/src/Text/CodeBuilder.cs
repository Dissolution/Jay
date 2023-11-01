using System.Runtime.CompilerServices;
using Jay.Text.Splitting;

namespace Jay.SourceGen.Text;

/// <summary>
/// <b>C</b>ode <b>B</b>uilder <b>A</b>ction<br/>
/// <see cref="Action{T}">Action&lt;CodeBuilder&gt;</see>
/// </summary>
public delegate void CBA(CodeBuilder codeBuilder);

partial class CodeBuilder
{
    private static readonly string _defaultNewLine = Environment.NewLine;
}

public sealed partial class CodeBuilder : IBuildingText
{
    private readonly TextBuffer _textBuffer;
    private string _newLineAndIndent = _defaultNewLine;

    /// <summary>
    /// Gets internal acces to the underlying <see cref="ITextWriter"/>
    /// </summary>
    internal ITextWriter Writer => _textBuffer;
    
    public CodeBuilder()
    {
        _textBuffer = new();
    }

    /// <summary>
    /// Gets the NewLine + Indent that exists at the current position
    /// </summary>
    /// <returns></returns>
    private ReadOnlySpan<char> GetCurrentNewLineAndIndent()
    {
        var newLine = _defaultNewLine.AsSpan();
        var written = _textBuffer.Written;
        var i = written.LastIndexOf<char>(newLine);
        return i == -1 ? newLine : written.Slice(i);
    }

    internal void IndentAwareAction(CBA cba)
    {
        var oldIndent = _newLineAndIndent;
        var newIndent = GetCurrentNewLineAndIndent();
        if (TextEqual(oldIndent, newIndent))
        {
            cba(this);
        }
        else
        {
            _newLineAndIndent = newIndent.ToString();
            cba(this);
            _newLineAndIndent = oldIndent;
        }
    }
    
    internal void IndentAwareWrite(ReadOnlySpan<char> text)
    {
        var buffer = _textBuffer;
        var newLine = _defaultNewLine.AsSpan();
        var newIndent = GetCurrentNewLineAndIndent();
        if (TextEqual(newIndent, newLine))
        {
            buffer.Write(text);
        }
        else
        {
            // Replace embedded NewLines with NewLine+Indent
            var split = text.TextSplit(newLine);
            while (split.MoveNext())
            {
                buffer.Write(split.Text);
                while (split.MoveNext())
                {
                    buffer.Write(newIndent);
                    buffer.Write(split.Text);
                }
            }
        }
    }

    internal void SmartFormat<T>([AllowNull] T value)
    {
        string? str;
        if (value.Is(out str))
        {
            // str has been assigned
        }
        else if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(default, default);
        }
        else if (value is CBA codeBuild)
        {
            IndentAwareAction(codeBuild);
            return;
        }
        else
        {
            throw new NotImplementedException();
        }
        
        // Write the finalized string
        _textBuffer.Write(str);
    }

    internal void DirectWrite(ReadOnlySpan<char> text)
    {
        _textBuffer.Write(text);
    }
    internal void DirectWrite(string? str)
    {
        _textBuffer.Write(str);
    }

    public CodeBuilder Append([InterpolatedStringHandlerArgument("")] ref InterpolatedCode interpolatedCode)
    {
        // Will already have appended, as we pass this to the InterpolatedCode
        return this;
    }

    public void Clear()
    {
        _textBuffer.Clear();
    }

    public void Dispose()
    {
        _textBuffer.Dispose();
    }

    public string ToStringAndDispose()
    {
        return _textBuffer.ToStringAndDispose();
    }

    public override string ToString()
    {
        return _textBuffer.ToString();
    }
}