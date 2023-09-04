using Jay.Text.Building;
using Jay.Text.Utilities;

namespace Jay.Reflection.CodeBuilding;

internal sealed class NewLineIndents : IDisposable
{
    public static readonly string DEFAULT_NEWLINE = Environment.NewLine;
    public const string DEFAULT_INDENT = "    "; // 4 spaces
    
    private readonly List<string> _indents;
    private char[] _totalString;
    private int _length;

    public string NewLine { get; internal set; } = DEFAULT_NEWLINE;
    public string Indent { get; internal set; } = DEFAULT_INDENT;

    public NewLineIndents() : this(DEFAULT_NEWLINE) { }

    public NewLineIndents(scoped ReadOnlySpan<char> newline)
    {
        _indents = new(0);
        _totalString = TextPool.Rent();
        TextHelper.CopyTo(newline, _totalString);
        _length = newline.Length;
        NewLine = newline.ToString();
    }
    
    public NewLineIndents(string newline)
    {
        _indents = new(0);
        _totalString = TextPool.Rent();
        TextHelper.CopyTo(newline, _totalString);
        _length = newline.Length;
        NewLine = newline;
    }
    
    public void AddIndent(string? indent = null)
    {
        indent ??= Indent;
        
        _indents.Add(indent);
        TextHelper.CopyTo(indent, _totalString.AsSpan(_length));
        _length += indent.Length;
    }

    public void RemoveIndent()
    {
        if (_indents.TryRemoveAt(^1, out var indent))
        {
            _length -= indent!.Length;
        }
    }

    public void Dispose()
    {
        var toReturn = _totalString;
        _totalString = null!;
        TextPool.Return(toReturn);
    }

    public ReadOnlySpan<char> AsSpan()
    {
        return _totalString.AsSpan(0, _length);
    }

    public override string ToString()
    {
        return new string(_totalString, 0, _length);
    }
}