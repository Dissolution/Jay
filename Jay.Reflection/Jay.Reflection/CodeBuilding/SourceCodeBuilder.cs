using System.Collections;
using System.Diagnostics;
using Jay.Debugging;
using Jay.Text.Building;
using Jay.Text.Splitting;
using TextWriter = Jay.Text.Building.TextWriter;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Jay.Reflection.CodeBuilding;

public delegate void SCBA<in T>(CodeBuilder builder, T item);
public delegate TResult SCBF<in T, out TResult>(CodeBuilder builder, T item);

public class CodeBuilder : IBuildingText
{
    public static CodeBuilder New => new();
    
    
    private readonly ITextBuffer _textBuffer;
    private NewLineIndents _newLineIndents;

    public CodeBuilder()
    {
        _textBuffer = new TextBuilder();
        _newLineIndents = new();
    }

    private NewLineIndents GetPositionNewLineIndents()
    {
        // what we think a newline is
        var newline = _newLineIndents.NewLine;
        // find the last occurrence of this in written
        var written = _textBuffer.Written;
        var finalNewlineIndex = written.LastIndexOf(newline.AsSpan());
        // if we didn't find one, we use what we've written thus far
        if (finalNewlineIndex == -1)
        {
            return new NewLineIndents(written);
        }
        // otherwise, a newline is that plus everything else (which captures new indents!)
        return new NewLineIndents(written.Slice(finalNewlineIndex));
    }

    public CodeBuilder Invoke(Scba scba)
    {
        scba(this);
        return this;
    }

    public CodeBuilder If(bool predicate, Scba? ifTrue, Scba? ifFalse = default)
    {
        if (predicate)
        {
            ifTrue?.Invoke(this);
        }
        else
        {
            ifFalse?.Invoke(this);
        }
        return this;
    }

    public CodeBuilder Write(char ch)
    {
        _textBuffer.Write(ch);
        return this;
    }
    
    public CodeBuilder WriteLine(char ch) => Write(ch).NewLine();

    public CodeBuilder Write(scoped ReadOnlySpan<char> text)
    {
        _textBuffer.Write(text);
        return this;
    }
    
    public CodeBuilder WriteLine(scoped ReadOnlySpan<char> text) => Write(text).NewLine();

    public CodeBuilder Write(string? str)
    {
        _textBuffer.Write(str);
        return this;
    }
    
    public CodeBuilder WriteLine(string? str) => Write(str).NewLine();


    public CodeBuilder Code(scoped ReadOnlySpan<char> text)
    {
        var lines = text.Split(_newLineIndents.NewLine);
        if (!lines.MoveNext())
            return this;

        _textBuffer.Write(lines.Text);
        while (lines.MoveNext())
        {
            _textBuffer.Write(_newLineIndents.AsSpan());
            _textBuffer.Write(lines.Text);
        }
        return this;
    }

    public CodeBuilder Code(string? str)
    {
        var lines = SplitExtensions.Split(str, _newLineIndents.NewLine);
        if (!lines.MoveNext())
            return this;

        _textBuffer.Write(lines.Text);
        while (lines.MoveNext())
        {
            _textBuffer.Write(_newLineIndents.AsSpan());
            _textBuffer.Write(lines.Text);
        }
        return this;
    }

    public CodeBuilder Write([InterpolatedStringHandlerArgument("")] ref InterpolatedCode code)
    {
        throw new NotImplementedException();
    }

    public CodeBuilder Code<T>(T? value)
    {
        switch (value)
        {
            case null:
                return Write("null");
            case Scba scba:
                return IndentCapturedAction(scba);
            case Delegate del:
            {
                Scba scba;
                try
                {
                    scba = (Scba)Delegate.CreateDelegate(typeof(Scba), del.Target, del.Method);
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    Console.WriteLine(ex);
                    throw;
                }
                return Code<Scba>(scba);
            }

            // Shouldn't get here
            case string str:
            {
                Debugger.Break();
                _textBuffer.Write(str);
                return this;
            }
            case IFormattable:
            {
                _textBuffer.Write<T>(value);
                return this;
            }
            case IEnumerable enumerable:
            {
                Hold.Onto(enumerable);
                Debugger.Break();
                return this;
            }
            default:
            {
                CodeManager.WriteCodeTo<T>(value, _textBuffer);
                return this;
            }
        }
    }

    public CodeBuilder Format<T>(T? value, string? format)
    {
        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format, default);
        }
        else
        {
            str = value?.ToString();
        }
        return Write(str);
    }

    public CodeBuilder NewLine()
    {
        _textBuffer.Write(_newLineIndents.AsSpan());
        return this;
    }

    internal CodeBuilder IndentCapturedAction(Scba scba)
    {
        var knownIndent = _newLineIndents;
        using var currentIndent = GetPositionNewLineIndents();
        _newLineIndents = currentIndent;
        scba(this);
        _newLineIndents = knownIndent;
        return this;
    }


    public CodeBuilder AddIndent(string? indent = null)
    {
        _newLineIndents.AddIndent(indent);
        return this;
    }

    public CodeBuilder RemoveIndent()
    {
        _newLineIndents.RemoveIndent();
        return this;
    }

    public CodeBuilder BracketBlock(Scba buildBlock)
    {
        //throw new NotImplementedException();

        /* We assume we're in the position they want us to start the block
         * (this supports end of line { and newline {)
         */
        return Write('{')
            .AddIndent()
            .NewLine()
            .Invoke(buildBlock)
            .RemoveIndent()
            .NewLine()
            .Write('}')
            .NewLine();
    }

    #region Enumerate
    public CodeBuilder Enumerate(
        TextSplitEnumerator textSplitEnumerator,
        Scbta perSplitSection)
    {
        while (textSplitEnumerator.MoveNext())
        {
            perSplitSection(this, textSplitEnumerator.Text);
        }
        return this;
    }

    public CodeBuilder Enumerate<T>(IEnumerable<T> values, SCBA<T> perValue)
    {
        foreach (var value in values)
        {
            perValue(this, value);
        }
        return this;
    }

    public CodeBuilder Iterate<T>(IEnumerable<T> values, Scbia<T> perValueIndex)
    {
        if (values is IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                perValueIndex(this, list[i], i);
            }
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            int i = 0;
            perValueIndex(this, e.Current, i);
            while (e.MoveNext())
            {
                i++;
                perValueIndex(this, e.Current, i);
            }
        }
        return this;
    }
#endregion

#region Delimit
    public CodeBuilder Delimit(
        ReadOnlySpan<char> delimiter,
        TextSplitEnumerator textSplitEnumerator,
        Scbta perSplitSection)
    {
        if (!textSplitEnumerator.MoveNext()) return this;
        perSplitSection(this, textSplitEnumerator.Text);
        while (textSplitEnumerator.MoveNext())
        {
            Write(delimiter);
            perSplitSection(this, textSplitEnumerator.Text);
        }
        return this;
    }

    public CodeBuilder Delimit<T>(Scba delimit, IEnumerable<T> values, SCBA<T> perValue)
    {
        if (values is IList<T> list)
        {
            var count = list.Count;
            if (count == 0)
                return this;

            perValue(this, list[0]);
            for (var i = 1; i < count; i++)
            {
                delimit(this);
                perValue(this, list[i]);
            }
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            perValue(this, e.Current);
            while (e.MoveNext())
            {
                delimit(this);
                perValue(this, e.Current);
            }
        }

        return this;
    }

    public CodeBuilder Delimit<T>(Scba delimit, IEnumerable<T> values, Scbia<T> perValueIndex)
    {
        if (values is IList<T> list)
        {
            var count = list.Count;
            if (count == 0)
                return this;

            perValueIndex(this, list[0], 0);
            for (var i = 1; i < count; i++)
            {
                delimit(this);
                perValueIndex(this, list[i], i);
            }
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            int i = 0;
            perValueIndex(this, e.Current, i);
            while (e.MoveNext())
            {
                i++;
                delimit(this);
                perValueIndex(this, e.Current, i);
            }
        }

        return this;
    }

    public CodeBuilder Delimit<T>(string delimiter, IEnumerable<T> values, SCBA<T> perValue)
    {
        return Delimit(w => w.Code(delimiter), values, perValue);
    }

    public CodeBuilder Delimit<T>(string delimiter, IEnumerable<T> values, Scbia<T> perValueIndex)
    {
        return Delimit(w => w.Code(delimiter), values, perValueIndex);
    }

    public CodeBuilder DelimitCode<T>(string delimiter, IEnumerable<T> values)
    {
        return Delimit(w => w.Code(delimiter), values, static (c,v) => c.Code<T>(v));
    }
    
    public CodeBuilder LineDelimit<T>(IEnumerable<T> values, SCBA<T> perValue)
    {
        return Delimit(static w => w.NewLine(), values, perValue);
    }

    public CodeBuilder LineDelimit<T>(IEnumerable<T> values, Scbia<T> perValueIndex)
    {
        return Delimit(static w => w.NewLine(), values, perValueIndex);
    }
#endregion
    
    
#region Header
    /// <summary>
    /// Adds <c>// &lt;auto-generated/&gt;</c> line(s) with an optional <paramref name="comment"/>
    /// </summary>
    public CodeBuilder AutoGeneratedHeader(string? comment = null)
    {
        if (comment is null)
        {
            return WriteLine("// <auto-generated/>");
        }

        WriteLine("// <auto-generated>");
        foreach (var line in SplitExtensions.Split(comment, NewLineIndents._defaultNewline))
        {
            Write("// ").WriteLine(line);
        }
        return WriteLine("// </auto-generated>");
    }

    /// <summary>
    /// Adds <c>#nullable enable|disable</c>
    /// </summary>
    public CodeBuilder Nullable(bool enable = true)
    {
        return Write("#nullable ")
            .Write(enable ? "enable" : "disable")
            .NewLine();
    }

    /// <summary>
    /// Writes a <c>using namespace;</c> line
    /// </summary>
    public CodeBuilder Using(string? @namespace)
    {
        if (!string.IsNullOrWhiteSpace(@namespace))
        {
            return Write("using ")
                .Write(@namespace)
                .WriteLine(';');
        }
        return this;
    }

    /// <summary>
    /// Writes multiple <c>using</c> <paramref name="namespaces"/>
    /// </summary>
    public CodeBuilder Usings(params string?[] namespaces)
    {
        foreach (var nameSpace in namespaces)
        {
            Using(nameSpace);
        }
        return this;
    }

    /// <summary>
    /// Writes multiple <c>using</c> <paramref name="namespaces"/>
    /// </summary>
    public CodeBuilder Usings(IEnumerable<string?> namespaces)
    {
        foreach (var nameSpace in namespaces)
        {
            Using(nameSpace);
        }
        return this;
    }

    /// <summary>
    /// Writes the start of a <c>namespace</c>
    /// </summary>
    /// <param name="namespace"></param>
    /// <returns></returns>
    public CodeBuilder Namespace(string @namespace)
    {
        Validate.IsNotNullOrWhiteSpace(@namespace);
        return Write("namespace ")
            .Write(@namespace)
            .WriteLine(';')
            .NewLine();
    }

    public CodeBuilder Namespace(
        string @namespace,
        Scba namespaceBlock)
    {
        Validate.IsNotNullOrWhiteSpace(@namespace);
        return Write("namespace ")
            .WriteLine(@namespace)
            .BracketBlock(namespaceBlock)
            .NewLine();
    }


    /// <summary>
    /// Writes the given <paramref name="comment"/> as a comment line / lines
    /// </summary>
    public CodeBuilder Comment(string? comment)
    {
        /* Most of the time, this is probably a single line.
         * But we do want to watch out for newline characters to turn
         * this into a multi-line comment */

        var comments = SplitExtensions.Split(comment, NewLineIndents._defaultNewline)
            .GetEnumerator();
        if (!comments.MoveNext())
        {
            // Null or empty comment is blank
            return WriteLine("// ");
        }
        var cmnt = comments.Text;
        if (!comments.MoveNext())
        {
            // Only a single comment
            return Write("// ")
                .WriteLine(cmnt);
        }

        // Multiple comments
        Write("/* ").WriteLine(cmnt)
            .Write(" * ").WriteLine(comments.Text);
        while (comments.MoveNext())
        {
            Write(" * ")
                .WriteLine(comments.Text);
        }
        return WriteLine(" */");
    }

    public CodeBuilder Comment(string? comment, CommentType commentType)
    {
        var splitEnumerable = SplitExtensions.Split(comment, NewLineIndents._defaultNewline);
        switch (commentType)
        {
            case CommentType.SingleLine:
            {
                foreach (var line in splitEnumerable)
                {
                    Write("// ")
                        .WriteLine(line);
                }
                break;
            }
            case CommentType.Xml:
            {
                foreach (var line in splitEnumerable)
                {
                    Write("/// ")
                        .WriteLine(line);
                }
                break;
            }
            case CommentType.MultiLine:
            {
                var comments = SplitExtensions.Split(comment, NewLineIndents._defaultNewline)
                    .GetEnumerator();
                if (!comments.MoveNext())
                {
                    // Null or empty comment is blank
                    return WriteLine("/* */");
                }
                var cmnt = comments.Text;
                if (!comments.MoveNext())
                {
                    // Only a single comment
                    return Write("/* ")
                        .Write(cmnt)
                        .WriteLine(" */");
                }

                // Multiple comments
                Write("/* ")
                    .WriteLine(cmnt);
                Write(" * ")
                    .WriteLine(comments.Text);
                while (comments.MoveNext())
                {
                    Write(" * ")
                        .WriteLine(comments.Text);
                }
                return WriteLine(" */");
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(commentType));
        }
        return this;
    }
#endregion

    public void Dispose()
    {
        _textBuffer.Dispose();
        _newLineIndents.Dispose();
    }

    public string ToStringAndDispose()
    {
        var str = ToString();
        Dispose();
        return str;
    }

    public override string ToString()
    {
        return _textBuffer.ToString()!;
    }
}