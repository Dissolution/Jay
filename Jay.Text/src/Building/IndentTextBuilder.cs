using System.Collections;
using Jay.Text.Splitting;

namespace Jay.Text.Building;

public class IndentTextBuilder<TBuilder> : 
    TextBuilder<TBuilder>, 
    IIndentTextBuilder<TBuilder> 
    where TBuilder : IndentTextBuilder<TBuilder>
{
    protected static readonly string DefaultIndent = "   ";
    
    protected List<string> _indents;
    
    public IndentTextBuilder() : base()
    {
        _indents = new(0);
    }
    public IndentTextBuilder(int minCapacity) 
        : base(minCapacity)
    {
        _indents = new(0);
    }

    protected internal string GetCurrentPositionAsIndent()
    {
        // Start searching all Written for the last newline
        var lastNewLineIndex = Written.LastIndexOf(_newline.AsSpan());
        // If we never wrote one, there's no indent
        if (lastNewLineIndex == -1)
            return string.Empty;
        /* everything after is our indent
         * it would seem we might only want to capture whitespace
         * but this lets us do hacks like indent('-') or indent('*')
         */
        var after = Written.Slice(lastNewLineIndex + _newline.Length);
        return after.ToString();
    }
    
    protected internal TBuilder IndentAwareAction(Action<TBuilder> action)
    {
        // Capture our original indents
        var originalIndents = _indents;
        // Replace them with a single indent based upon this position
        _indents = new List<string>(1) { GetCurrentPositionAsIndent() };
        // perform the action
        action(_builder);
        // restore the indents
        _indents = originalIndents;
        return _builder;
    }

    public override TBuilder NewLine()
    {
        // newline
        base.Write(_newline);
        // all indents
        foreach (var indent in _indents)
        {
            base.Write(indent);
        }
        return _builder;
    }
    
    public override void Write(string? str)
    {
        if (string.IsNullOrEmpty(str)) return;
        // We're going to be splitting on NewLine
        var e = new TextSplitEnumerator(str.AsSpan(), _newline.AsSpan());
        if (!e.MoveNext()) return;
        this.Write(e.Text);
        while (e.MoveNext())
        {
            // Delimit with NewLine
            this.NewLine();
            // Write this slice
            this.Write(e.Text);
        }
    }

    public override void Write(scoped ReadOnlySpan<char> text)
    {
        int textLen = text.Length;
        if (textLen == 0) return;

        // We're going to be splitting on NewLine
        var e = new TextSplitEnumerator(text, _newline.AsSpan());
        if (!e.MoveNext()) return;
        this.Write(e.Text);
        while (e.MoveNext())
        {
            // Delimit with NewLine
            this.NewLine();
            // Write this slice
            this.Write(e.Text);
        }
    }

    public override void Write<T>(T? value, ReadOnlySpan<char> format, IFormatProvider? provider = null) where T : default => Write<T>(value, format.ToString(), provider);
    
    public override void Write<T>(T? value, string? format, IFormatProvider? provider = null) where T : default
    {
        switch (value)
        {
            case null:
            {
                return;
            }
            case Action<TBuilder> tba:
            {
                IndentAwareAction(tb => tba(tb));
                return;
            }
            case string str:
            {
                this.Write(str);
                return;
            }
            case IFormattable formattable:
            {
                base.Write(formattable, format, provider);
                return;
            }
            case IEnumerable enumerable:
            {
                format ??= ",";
                this.Delimit(
                    format,
                    enumerable.Cast<object?>(),
                    (w, v) => w.Append<object?>(v, format, provider)
                );
                return;
            }
            default:
            {
                base.Write<T>(value, format, provider);
                return;
            }
        }
    }
    
    public TBuilder Indented(char indent, Action<TBuilder> indentedAction)
    {
        _indents.Add(indent.ToString());
        indentedAction(_builder);
        _indents.RemoveAt(_indents.Count-1);
        return _builder;
    }
    
    public TBuilder Indented(scoped ReadOnlySpan<char> indent, Action<TBuilder> indentedAction)
    {
        _indents.Add(indent.ToString());
        indentedAction(_builder);
        _indents.RemoveAt(_indents.Count-1);
        return _builder;
    }
    
    
    public TBuilder Indented(string indent, Action<TBuilder> indentedAction)
    {
        _indents.Add(indent);
        indentedAction(_builder);
        _indents.RemoveAt(_indents.Count-1);
        return _builder;
    }
    
    
    /*
    public TBuilder IndentBlock(Action<TBuilder> indentBlock)
    {
        return IndentBlock(DefaultIndent, indentBlock);
    }

    public TBuilder IndentBlock(string indent, Action<TBuilder> indentBlock)
    {
        var oldIndent = _newLineIndent;
        // We might be on a new line, but not yet indented
        if (TextHelper.Equals(CurrentLine, oldIndent))
        {
            Append(indent);
        }

        var newIndent = oldIndent + indent;
        _newLineIndent = newIndent;
        indentBlock(this);
        _newLineIndent = oldIndent;
        // Did we do a newline that we need to decrease?
        if (Written.EndsWith(newIndent.AsSpan()))
        {
            _position -= newIndent.Length;
            Append(oldIndent);
        }
        return this;
    }

    public TBuilder EnsureOnStartOfNewLine()
    {
        if (!Written.EndsWith(_newLineIndent.AsSpan()))
        {
            return NewLine();
        }
        return this;
    }

    public TBuilder BracketBlock(Action<TBuilder> bracketBlock, string? indent = null)
    {
        indent ??= DefaultIndent;
        // Trim all trailing whitespace
        return TrimEnd()
            // Start a new line
            .NewLine()
            // Starting bracket
            .AppendLine('{')
            // Starts an indented block inside of that bracket
            .IndentBlock(indent, bracketBlock)
            // Be sure that we're not putting the end bracket at the end of text
            .EnsureOnStartOfNewLine()
            // Ending bracket
            .Append('}');
    }
    */
}