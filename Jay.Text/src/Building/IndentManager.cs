using Jay.Memory;

namespace Jay.Text.Building;

public sealed class IndentManager : IDisposable
{
    private char[] _indents;
    private int _indentsPosition;
    private readonly Stack<int> _indentPositions;

    public ReadOnlySpan<char> CurrentIndent => _indents.AsSpan(0, _indentsPosition);
    
    public IndentManager()
    {
        _indents = TextPool.Rent();
        _indentsPosition = 0;
        _indentPositions = new(0);
    }
    
    public void AddIndent(char indent)
    {
        _indentPositions.Push(_indentsPosition);
        RefArrayWriter.Write(ref _indents, ref _indentsPosition, indent);
    }

    public void AddIndent(string indent)
    {
        _indentPositions.Push(_indentsPosition);
        RefArrayWriter.Write(ref _indents, ref _indentsPosition, indent);
    }

    public void AddIndent(scoped ReadOnlySpan<char> indent)
    {
        _indentPositions.Push(_indentsPosition);
        RefArrayWriter.Write(ref _indents, ref _indentsPosition, indent);
    }

    public void RemoveIndent()
    {
        if (_indentPositions.Count > 0)
        {
            _indentsPosition = _indentPositions.Pop();
        }
    }

    public void RemoveIndent(out ReadOnlySpan<char> lastIndent)
    {
        if (_indentPositions.Count > 0)
        {
            var lastIndentIndex = _indentPositions.Pop();
            lastIndent = _indents.AsSpan(new Range(start: lastIndentIndex, end: _indentsPosition));
            _indentsPosition = lastIndentIndex;
        }
        else
        {
            lastIndent = default;
        }
    }

    
    public void Dispose()
    {
        char[]? toReturn = _indents;
        _indents = null!;
        TextPool.Return(toReturn);
    }
}