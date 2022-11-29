using System.ComponentModel;

using Jay.Text;


// ReSharper disable UnusedParameter.Local

namespace Jay.Dumping.Interpolated;

[InterpolatedStringHandler]
public ref struct DumpStringHandler
{
    private CharSpanBuilder _textBuilder;

    public int Length => _textBuilder.Length;

    public DumpStringHandler()
    {
        _textBuilder = new();
    }

    public DumpStringHandler(Span<char> initialBuffer)
    {
        _textBuilder = new(initialBuffer);
    }
    
    public DumpStringHandler(int literalLength, int formatCount)
    {
        _textBuilder = new();
    }

    public DumpStringHandler(int literalLength, int formatCount, Span<char> initialBuffer)
    {
        _textBuilder = new(initialBuffer);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string? text)
    {
        _textBuilder.Write(text);
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T? value)
    {
        Dump<T>(value);
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T? value, string? format)
    {
        Dump<T>(value, format);
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string? text)
    {
        _textBuilder.Write(text);
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(ReadOnlySpan<char> text)
    {
        _textBuilder.Write(text);
    }

    public void Write(char ch)
    {
        _textBuilder.Write(ch);
    }

    public void Write(string? text)
    {
        _textBuilder.Write(text);
    }

    public void Write(ReadOnlySpan<char> text)
    {
        _textBuilder.Write(text);
    }

    public void Write<T>(T? value)
    {
        _textBuilder.Write<T>(value);
    }

    public void Write<T>(T? value, string? format)
    {
        _textBuilder.Write<T>(value, format);
    }

    public void WriteLine()
    {
        _textBuilder.Write(Environment.NewLine);
    }

    public void Dump<T>(T? value, DumpFormat dumpFormat = default)
    {
        if (value is IEnumerable enumerable)
        {
            DumpEnumerable(enumerable, dumpFormat);
        }
        else
        {
            var dumper = DumperCache.GetDumper<T>();
            dumper.DumpTo(ref this, value, dumpFormat);
        }
    }

    private void DumpEnumerable(IEnumerable enumerable, DumpFormat format)
    {
        ReadOnlySpan<char> delimiter = ", ";
        if (format.IsCustom)
            delimiter = format;
        IEnumerator? enumerator = null;
        try
        {
            enumerator = enumerable.GetEnumerator();
            // No values?
            if (!enumerator.MoveNext()) return;
            // Get the value dumper
            var objDumper = DumperCache.GetDumper<object>();
            objDumper.DumpTo(ref this, enumerator.Current, format);
            while (enumerator.MoveNext())
            {
                Write(delimiter);
                objDumper.DumpTo(ref this, enumerator.Current, format);
            }
        }
        finally
        {
            if (enumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        
    }

    public void DumpDelimited<T>(
        ReadOnlySpan<char> delimiter,
        IEnumerable<T> values,
        DumpFormat dumpFormat = default)
    {
        using var e = values.GetEnumerator();
        // No values?
        if (!e.MoveNext()) return;
        // Get the value dumper
        var dumper = DumperCache.GetDumper<T>();
        dumper.DumpTo(ref this, e.Current, dumpFormat);
        while (e.MoveNext())
        {
            Write(delimiter);
            dumper.DumpTo(ref this, e.Current, dumpFormat);
        }
    }

    public void Dispose()
    {
        _textBuilder.Dispose();
    }
    
    public string ToStringAndDispose()
    {
        return _textBuilder.ToStringAndDispose();
    }

    public override string ToString()
    {
        return _textBuilder.ToString();
    }
}