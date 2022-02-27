using Jay.Text;

namespace Jay.Dumping;


internal sealed class Format<T> : IValueDumper<T>, IValueDumper<T?>, IObjectDumper
    where T : struct, ISpanFormattable, IFormattable
{
    private readonly string? _detailedFormat;
    private readonly string? _format;

    public Format(string? detailedFormat, string? format = null)
    {
        _detailedFormat = detailedFormat;
        _format = null;
    }

    /// <inheritdoc />
    void IObjectDumper.DumpObject(TextBuilder text, object? obj, DumpOptions options)
    {
        if (obj is null)
        {
            this.DumpValue(text, (T?)null, options);
        }
        else if (obj is T value)
        {
            this.DumpValue(text, value, options);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public bool CanDump(Type objType)
    {
        return objType == typeof(T) || objType == typeof(Nullable<T>);
    }
    
    /// <inheritdoc />
    public void DumpValue(TextBuilder text, T value, DumpOptions options = default)
    {
        if (options.Detailed)
        {
            text.WriteFormatted(value, _detailedFormat ?? _format ?? options.Format);
        }
        else
        {
            text.WriteFormatted(value, _format ?? options.Format);
        }
    }
    
    /// <inheritdoc />
    public void DumpValue(TextBuilder text, T? value, DumpOptions options = default)
    {
        if (value.HasValue)
        {
            if (options.Detailed)
            {
                text.WriteFormatted(value, _detailedFormat ?? _format ?? options.Format);
            }
            else
            {
                text.WriteFormatted(value, _format ?? options.Format);
            }
        }
        else
        {
            Dumper.DumpNull<Nullable<T>>(text, value, options);
        }
    }

    
}