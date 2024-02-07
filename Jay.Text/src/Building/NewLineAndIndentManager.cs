namespace Jay.Text.Building;

public sealed class NewLineAndIndentManager : IDisposable
{
    public static string DefaultNewLine { get; set; } = Environment.NewLine;
    public static string DefaultIndent { get; set; } = "    "; // 4 spaces
    
    private char[] _buffer;
    private int _bufferPosition;
    private readonly Stack<int> _indentOffsets;

    public ReadOnlySpan<char> CurrentNewLineAndIndent => _buffer.AsSpan(0, _bufferPosition);
    
    public NewLineAndIndentManager()
    {
        _buffer = TextPool.Rent();
        var newLine = Environment.NewLine.AsSpan();
        newLine.CopyTo(_buffer);
        _bufferPosition = newLine.Length;
        _indentOffsets = new();
    }
    
    public void AddIndent(char indent)
    {
        int pos = _bufferPosition;
        int newPos = pos + 1;
        if (newPos > _buffer.Length)
            throw new InvalidOperationException();
        _indentOffsets.Push(pos);
        _buffer[pos] = indent;
        _bufferPosition = newPos;
    }

    public void AddIndent(string? indent) => AddIndent(indent.AsSpan());
    

    public void AddIndent(scoped ReadOnlySpan<char> indent)
    {
        int pos = _bufferPosition;
        int newPos = pos + indent.Length;
        if (newPos > _buffer.Length)
            throw new InvalidOperationException();
        _indentOffsets.Push(pos);
        indent.CopyTo(_buffer.AsSpan(pos));
        _bufferPosition = newPos;
    }

    public void RemoveIndent()
    {
        if (_indentOffsets.Count > 0)
        {
            _bufferPosition = _indentOffsets.Pop();
        }
    }

    public void RemoveIndent(out ReadOnlySpan<char> lastIndent)
    {
        if (_indentOffsets.Count > 0)
        {
            var lastIndentIndex = _indentOffsets.Pop();
            lastIndent = _buffer.AsSpan()[new Range(start: lastIndentIndex, end: _bufferPosition)];
            _bufferPosition = lastIndentIndex;
        }
        else
        {
            lastIndent = default;
        }
    }

    
    public void Dispose()
    {
        char[]? toReturn = _buffer;
        _buffer = null!;
        TextPool.Return(toReturn);
    }
}