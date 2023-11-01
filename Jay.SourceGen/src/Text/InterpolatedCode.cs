using System.Runtime.CompilerServices;
using Jay.Text.Extensions;

namespace Jay.SourceGen.Text;

/// <summary>
/// An <see cref="System.Runtime.CompilerServices.InterpolatedStringHandlerAttribute">Interpolated String Handler</see>
/// that works with an underlying <see cref="CodeBuilder"/>
/// </summary>
[InterpolatedStringHandler]
// ReSharper disable once StructCanBeMadeReadOnly
public ref struct InterpolatedCode
{
    private readonly CodeBuilder _codeBuilder;

    public InterpolatedCode(int literalLength, int formattedCount, CodeBuilder codeBuilder)
    {
        _codeBuilder = codeBuilder;
    }

    public void AppendLiteral(string str)
    {
        _codeBuilder.IndentAwareWrite(str.AsSpan());
    }

    public void AppendFormatted<T>([AllowNull] T value)
    {
        _codeBuilder.SmartFormat<T>(value);
    }

    public void AppendFormatted<T>([AllowNull] T value, string? format)
    {
        string? str;
    
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format, default);
        }
        else
        {
            Casing casing;
            if (string.IsNullOrEmpty(format))
            {
                casing = Casing.Default;
            }
            else if (TextEqual(format, "l", StringComparison.OrdinalIgnoreCase) || TextEqual(format, "lower", StringComparison.OrdinalIgnoreCase))
            {
                casing = Casing.Lower;
            }
            else if (TextEqual(format, "u", StringComparison.OrdinalIgnoreCase) || TextEqual(format, "upper", StringComparison.OrdinalIgnoreCase))
            {
                casing = Casing.Upper;
            }
            else if (TextEqual(format, "c", StringComparison.OrdinalIgnoreCase) || TextEqual(format, "camel", StringComparison.OrdinalIgnoreCase))
            {
                casing = Casing.Camel;
            }
            else if (TextEqual(format, "p", StringComparison.OrdinalIgnoreCase) || TextEqual(format, "pascal", StringComparison.OrdinalIgnoreCase))
            {
                casing = Casing.Pascal;
            }
            else if (TextEqual(format, "t", StringComparison.OrdinalIgnoreCase) || TextEqual(format, "title", StringComparison.OrdinalIgnoreCase))
            {
                casing = Casing.Title;
            }
            else
            {
                throw new ArgumentException($"Unknown format code: '{format}'", nameof(format));
            }
    
            str = value?.ToString().ToCase(casing);
        }
        
        _codeBuilder.DirectWrite(str);
    }

}