using System.Text;

namespace Jay.Text.Scratch;

public class StringBuilderTextBuffer : ITextBuffer
{
    private StringBuilder _stringBuilder;

    public char this[int index]
    {
        get => _stringBuilder[index];
        set => _stringBuilder[index] = value;
    }

    public Span<char> this[Range range]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public int Length => _stringBuilder.Length;

    public int Capacity => _stringBuilder.Capacity;

    public Span<char> Written => throw new NotImplementedException();

    public Span<char> Available => throw new NotImplementedException();

    public StringBuilderTextBuffer()
    {
        _stringBuilder = StringBuilderPool.Shared.Rent();
    }

    public StringBuilderTextBuffer(StringBuilder stringBuilder)
    {
        _stringBuilder = stringBuilder;
    }

    public void Write(char ch)
    {
        _stringBuilder.Append(ch);
    }

    public void Write(params char[]? chars)
    {
        _stringBuilder.Append(chars);
    }

    public void Write(scoped ReadOnlySpan<char> text)
    {
        _stringBuilder.Append(text);
    }

    public void Write(string? str)
    {
        _stringBuilder.Append(str);
    }

    public void Write([InterpolatedStringHandlerArgument("")] ref InterpolatedTextWriter interpolatedText)
    {
        // already written
    }

    public void WriteLine()
    {
        _stringBuilder.AppendLine();
    }

    public string ToStringAndDispose()
    {
        var toReturn = Interlocked.Exchange(ref _stringBuilder, null!);
        return StringBuilderPool.Shared.ReturnToString(toReturn);
    }

    public void Dispose()
    {
        var toReturn = Interlocked.Exchange(ref _stringBuilder, null!);
        StringBuilderPool.Shared.Return(toReturn);
    }
}