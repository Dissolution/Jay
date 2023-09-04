using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;
using Jay.SourceGen.Coding;
using Jay.Text.Building;
using Jay.Text.Splitting;
using Jay.Validation;
using TextWriter = Jay.Text.Building.TextWriter;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Jay.SourceGen.CodeBuilding;

public delegate void SCBA(SourceCodeBuilder builder);
public delegate void SCBA<in T>(SourceCodeBuilder builder, T item);
public delegate void SCBIA<in T>(SourceCodeBuilder builder, T item, int index);
public delegate TResult SCBF<out TResult>(SourceCodeBuilder builder);
public delegate TResult SCBF<in T, out TResult>(SourceCodeBuilder builder, T item);
public delegate void SCBTA(SourceCodeBuilder builder, ReadOnlySpan<char> text);
public delegate TResult SCBTF<out TResult>(SourceCodeBuilder builder, ReadOnlySpan<char> text);


public class SourceCodeBuilder : IBuildingText
{
    public static SourceCodeBuilder New => new();
    
    
    private readonly TextWriter _textWriter;
    private NewLineIndents _newLineIndents;

    public SourceCodeBuilder()
    {
        _textWriter = new();
        _newLineIndents = new();
    }

    private NewLineIndents GetPositionNewLineIndents()
    {
        // what we think a newline is
        var newline = _newLineIndents.NewLine;
        // find the last occurrence of this in written
        var written = _textWriter.Written;
        var finalNewlineIndex = written.LastIndexOf(newline.AsSpan());
        // if we didn't find one, we use what we've written thus far
        if (finalNewlineIndex == -1)
        {
            return new NewLineIndents(written);
        }
        // otherwise, a newline is that plus everything else (which captures new indents!)
        return new NewLineIndents(written.Slice(finalNewlineIndex));
    }

    public SourceCodeBuilder Invoke(SCBA scba)
    {
        scba(this);
        return this;
    }

    public SourceCodeBuilder If(bool predicate, SCBA? ifTrue, SCBA? ifFalse = default)
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

    public SourceCodeBuilder Write(char ch)
    {
        _textWriter.Write(ch);
        return this;
    }
    
    public SourceCodeBuilder WriteLine(char ch) => Write(ch).NewLine();

    public SourceCodeBuilder Write(scoped ReadOnlySpan<char> text)
    {
        _textWriter.Write(text);
        return this;
    }
    
    public SourceCodeBuilder WriteLine(scoped ReadOnlySpan<char> text) => Write(text).NewLine();

    public SourceCodeBuilder Write(string? str)
    {
        _textWriter.Write(str);
        return this;
    }
    
    public SourceCodeBuilder WriteLine(string? str) => Write(str).NewLine();


    public SourceCodeBuilder Code(scoped ReadOnlySpan<char> text)
    {
        var lines = text.TextSplit(_newLineIndents.NewLine)
            .GetEnumerator();
        if (!lines.MoveNext())
            return this;

        _textWriter.Write(lines.Text);
        while (lines.MoveNext())
        {
            _textWriter.Write(_newLineIndents.AsSpan());
            _textWriter.Write(lines.Text);
        }
        return this;
    }

    public SourceCodeBuilder Code(string? str)
    {
        var lines = str.TextSplit(_newLineIndents.NewLine)
            .GetEnumerator();
        if (!lines.MoveNext())
            return this;

        _textWriter.Write(lines.Text);
        while (lines.MoveNext())
        {
            _textWriter.Write(_newLineIndents.AsSpan());
            _textWriter.Write(lines.Text);
        }
        return this;
    }

    public SourceCodeBuilder Code([InterpolatedStringHandlerArgument("")] ref InterpolatedCode code)
    {
        throw new NotImplementedException();
    }

    public SourceCodeBuilder Code<T>(T? value)
    {
        switch (value)
        {
            case null:
                return Write("null");
            case SCBA scba:
                return IndentCapturedAction(scba);
            case Delegate del:
            {
                SCBA scba;
                try
                {
                    scba = (SCBA)Delegate.CreateDelegate(typeof(SCBA), del.Target, del.Method);
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    throw;
                }
                return Code<SCBA>(scba);
            }

            // Shouldn't get here
            case string str:
            {
                Debugger.Break();
                _textWriter.Write(str);
                return this;
            }
            case IFormattable:
            {
                _textWriter.Format<T>(value);
                return this;
            }
            case IEnumerable enumerable:
            {
                Debugger.Break();
                return this;
            }
            default:
            {
                CodeManager.WriteCodeTo<T>(value, _textWriter);
                return this;
            }
        }
    }

    public SourceCodeBuilder Code<T>(T? value, string? format)
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

    public SourceCodeBuilder NewLine()
    {
        _textWriter.Write(_newLineIndents.AsSpan());
        return this;
    }

    internal SourceCodeBuilder IndentCapturedAction(SCBA scba)
    {
        var knownIndent = _newLineIndents;
        using var currentIndent = GetPositionNewLineIndents();
        _newLineIndents = currentIndent;
        scba(this);
        _newLineIndents = knownIndent;
        return this;
    }


    public SourceCodeBuilder AddIndent(string? indent = null)
    {
        _newLineIndents.AddIndent(indent);
        return this;
    }

    public SourceCodeBuilder RemoveIndent()
    {
        _newLineIndents.RemoveIndent();
        return this;
    }

    public SourceCodeBuilder BracketBlock(SCBA buildBlock)
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
    public SourceCodeBuilder Enumerate(
        TextSplitEnumerable splitEnumerable,
        SCBTA perSplitSection)
    {
        foreach (var splitSection in splitEnumerable)
        {
            perSplitSection(this, splitSection);
        }
        return this;
    }

    public SourceCodeBuilder Enumerate<T>(IEnumerable<T> values, SCBA<T> perValue)
    {
        foreach (var value in values)
        {
            perValue(this, value);
        }
        return this;
    }

    public SourceCodeBuilder Iterate<T>(IEnumerable<T> values, SCBIA<T> perValueIndex)
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
    public SourceCodeBuilder Delimit(
        ReadOnlySpan<char> delimiter,
        TextSplitEnumerable splitEnumerable,
        SCBTA perSplitSection)
    {
        var splitEnumerator = splitEnumerable.GetEnumerator();
        if (!splitEnumerator.MoveNext()) return this;
        perSplitSection(this, splitEnumerator.Current);
        while (splitEnumerator.MoveNext())
        {
            Write(delimiter);
            perSplitSection(this, splitEnumerator.Current);
        }
        return this;
    }

    public SourceCodeBuilder Delimit<T>(SCBA delimit, IEnumerable<T> values, SCBA<T> perValue)
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

    public SourceCodeBuilder Delimit<T>(SCBA delimit, IEnumerable<T> values, SCBIA<T> perValueIndex)
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

    public SourceCodeBuilder Delimit<T>(string delimiter, IEnumerable<T> values, SCBA<T> perValue)
    {
        return Delimit(w => w.Code(delimiter), values, perValue);
    }

    public SourceCodeBuilder Delimit<T>(string delimiter, IEnumerable<T> values, SCBIA<T> perValueIndex)
    {
        return Delimit(w => w.Code(delimiter), values, perValueIndex);
    }

    public SourceCodeBuilder LineDelimit<T>(IEnumerable<T> values, SCBA<T> perValue)
    {
        return Delimit(static w => w.NewLine(), values, perValue);
    }

    public SourceCodeBuilder LineDelimit<T>(IEnumerable<T> values, SCBIA<T> perValueIndex)
    {
        return Delimit(static w => w.NewLine(), values, perValueIndex);
    }
#endregion
    
    
#region Header
    /// <summary>
    /// Adds <c>// &lt;auto-generated/&gt;</c> line(s) with an optional <paramref name="comment"/>
    /// </summary>
    public SourceCodeBuilder AutoGeneratedHeader(string? comment = null)
    {
        if (comment is null)
        {
            return WriteLine("// <auto-generated/>");
        }

        WriteLine("// <auto-generated>");
        foreach (var line in comment.TextSplit(NewLineIndents.DEFAULT_NEWLINE))
        {
            Write("// ").WriteLine(line);
        }
        return WriteLine("// </auto-generated>");
    }

    /// <summary>
    /// Adds <c>#nullable enable|disable</c>
    /// </summary>
    public SourceCodeBuilder Nullable(bool enable = true)
    {
        return Write("#nullable ")
            .Write(enable ? "enable" : "disable")
            .NewLine();
    }

    /// <summary>
    /// Writes a <c>using namespace;</c> line
    /// </summary>
    public SourceCodeBuilder Using(string? @namespace)
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
    public SourceCodeBuilder Usings(params string?[] namespaces)
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
    public SourceCodeBuilder Usings(IEnumerable<string?> namespaces)
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
    public SourceCodeBuilder Namespace(string @namespace)
    {
        Validate.IsNotNullOrWhiteSpace(@namespace);
        return Write("namespace ")
            .Write(@namespace)
            .WriteLine(';')
            .NewLine();
    }

    public SourceCodeBuilder Namespace(
        string @namespace,
        SCBA namespaceBlock)
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
    public SourceCodeBuilder Comment(string? comment)
    {
        /* Most of the time, this is probably a single line.
         * But we do want to watch out for newline characters to turn
         * this into a multi-line comment */

        var comments = comment.TextSplit(NewLineIndents.DEFAULT_NEWLINE)
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

    public SourceCodeBuilder Comment(string? comment, CommentType commentType)
    {
        var splitEnumerable = comment.TextSplit(NewLineIndents.DEFAULT_NEWLINE);
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
                var comments = comment.TextSplit(NewLineIndents.DEFAULT_NEWLINE)
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
        _textWriter.Dispose();
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
        return _textWriter.ToString();
    }
}