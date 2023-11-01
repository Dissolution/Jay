using Jay.Text.Splitting;

namespace Jay.Text.Building;

public class IndentTextBuilder<B> : TextBuilder<B>, IIndentTextBuilder<B>
    where B : IndentTextBuilder<B>
{
    protected IndentManager _indents;

    public IndentTextBuilder()
        : this(TextPool.MINIMUM_CAPACITY)
    { }

    public IndentTextBuilder(int minCapacity) 
        : base(minCapacity)
    {
        _indents = new();
    }

    protected ReadOnlySpan<char> GetArgIndent()
    {
        /* When we want to capture a new indent for a formatting argument,
         * we're looking for the last NewLine. Everything after that is the
         * indent to this position.*/

        var written = this.Written;
        var lastNewLineIndex = written.LastIndexOf<char>(_newline.AsSpan());
        // If we never wrote one, there is no indent
        if (lastNewLineIndex == -1)
            return default;
        var after = written.Slice(lastNewLineIndex + _newline.Length);
        return after;
    }

    protected B IndentAwareInvoke(Action<B> build)
    {
        // Capture our original indents
        var original = _indents;
        // Replace them with a single indent based upon this position
        var argIndent = GetArgIndent();
        _indents = new();
        _indents.AddIndent(argIndent);
        // perform the action
        build(_builder);
        // restore the indents
        _indents.Dispose();
        _indents = original;
        // fluent
        return _builder;
    }

    protected void IndentAwareWrite(scoped ReadOnlySpan<char> text)
    {
        var e = text.TextSplit(_newline);
        if (!e.MoveNext()) return;
        base.Write(e.Text);
        while (e.MoveNext())
        {
            base.Write(_newline);
            base.Write(e.Text);
        }
    }
    
    
    public override B NewLine()
    {
        base.Write(_newline);
        base.Write(_indents.CurrentIndent);
        return _builder;
    }

    public override void Write(params char[]? characters) 
        => IndentAwareWrite(characters.AsSpan());

    public override void Write(scoped ReadOnlySpan<char> text) 
        => IndentAwareWrite(text);

    public override void Write(string? str) 
        => IndentAwareWrite(str.AsSpan());

    public override void Write<T>([AllowNull] T value)
    {
        switch (value)
        {
            case null:
            {
                return;
            }
            case Action<B> build:
            {
                IndentAwareInvoke(build);
                return;
            }
            case string str:
            {
                this.Write(str);
                return;
            }
            default:
            {
                this.Write(value.ToString());
                return;
            }
        }
    }

    public B AddIndent(char indent)
    {
        _indents.AddIndent(indent);
        return _builder;
    }

    public B AddIndent(string indent)
    {
        _indents.AddIndent(indent);
        return _builder;
    }

    public B AddIndent(scoped ReadOnlySpan<char> indent)
    {
        _indents.AddIndent(indent);
        return _builder;
    }

    public B RemoveIndent()
    {
        _indents.RemoveIndent();
        return _builder;
    }

    public B RemoveIndent(out ReadOnlySpan<char> lastIndent)
    {
        _indents.RemoveIndent(out lastIndent);
        return _builder;
    }

    public B Indented(char indent, Action<B> buildIndentedText) 
        => AddIndent(indent)
            .Invoke(buildIndentedText)
            .RemoveIndent();

    public B Indented(string indent, Action<B> buildIndentedText)
        => AddIndent(indent)
            .Invoke(buildIndentedText)
            .RemoveIndent();

    public B Indented(scoped ReadOnlySpan<char> indent, Action<B> buildIndentedText)
        => AddIndent(indent)
            .Invoke(buildIndentedText)
            .RemoveIndent();

    public override void Dispose()
    {
        _indents.Dispose();
        base.Dispose();
    }
}