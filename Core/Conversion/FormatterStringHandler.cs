using System.Buffers;
using Jay.Dumping;
using Jay.Exceptions;
using Jay.Text;

namespace Jay.Conversion;

[InterpolatedStringHandler]
public ref struct FormatterStringHandler
{
    private readonly FormatterCache _formatterCache;
    private readonly FormatOptions _formatOptions;
    private char[] _chars;
    private int _length;

    private Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.AsSpan(0, _length);
    }
    
    private Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.AsSpan(_length);
    }

    internal FormatterStringHandler(int literalLength, int formatCount, FormatterCache formatterCache)
        : this(literalLength, formatCount, default(FormatOptions), formatterCache) { }
    
    internal FormatterStringHandler(int literalLength, int formatCount, FormatOptions options, FormatterCache formatterCache)
    {
        _formatterCache = formatterCache;
        _formatOptions = options;
        _chars = ArrayPool<char>.Shared.Rent(Math.Max(literalLength + (formatCount * 8), 1024));
        _length = 0;
    }
    
    private void Grow()
    {
        var newArray = ArrayPool<char>.Shared.Rent(_chars.Length * 2);
        TextHelper.CopyTo(Written, newArray);
        ArrayPool<char>.Shared.Return(_chars);
        _chars = newArray;
    }

    public void AppendLiteral(char ch)
    {
        if (_length >= _chars.Length)
            Grow();
        _chars[_length++] = ch;
    }
    
    public void AppendLiteral(ReadOnlySpan<char> text)
    {
        while (!TextHelper.TryCopyTo(text, Available))
        {
            Grow();
        }
        _length += text.Length;
    }
    
    public void AppendLiteral(string? text)
    {
        if (text is not null)
        {
            while (!TextHelper.TryCopyTo(text, Available))
            {
                Grow();
            }
            _length += text.Length;
        }
    }

    public void AppendFormatted(ReadOnlySpan<char> text)
    {
        while (!TextHelper.TryCopyTo(text, Available))
        {
            Grow();
        }
        _length += text.Length;
    }
    
    public void AppendFormatted<T>(T? value)
    {
        var formatter = _formatterCache.GetFormatter<T>();
        int charsWritten;
        while (!formatter.TryFormat(value, Available, out charsWritten, _formatOptions))
        {
            Grow();
        }
        _length += charsWritten;
    }

    public void AppendFormatted<T>(T? value, string? format)
    {
        var formatter = _formatterCache.GetFormatter<T>();
        int charsWritten;
        var options = _formatOptions with { Format = format };
        while (!formatter.TryFormat(value, Available, out charsWritten, options))
        {
            Grow();
        }
        _length += charsWritten;
    }

    public string ToStringAndClear()
    {
        var str = new string(Written);
        Dispose();
        return str;
    }

    public void Dispose()
    {
        ArrayPool<char>.Shared.Return(_chars);
    }
    
    public override bool Equals(object? obj)
    {
        return UnsuitableException.ThrowEquals(typeof(DumpStringHandler));
    }

    public override int GetHashCode()
    {
        return UnsuitableException.ThrowGetHashCode(typeof(DumpStringHandler));
    }

    public override string ToString()
    {
        throw new InvalidOperationException($"You MUST call {nameof(ToStringAndClear)}() to retrieve the resolved {nameof(FormatterStringHandler)} string");
    }
}