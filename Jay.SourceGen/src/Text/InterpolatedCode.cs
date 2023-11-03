// ReSharper disable UnusedParameter.Local
// ReSharper disable StructCanBeMadeReadOnly
namespace Jay.SourceGen.Text;

/// <summary>
/// An <see cref="System.Runtime.CompilerServices.InterpolatedStringHandlerAttribute">Interpolated String Handler</see>
/// that works with an underlying <see cref="CodeBuilder"/>
/// </summary>
[InterpolatedStringHandler]
public ref struct InterpolatedCode
{
    private readonly CodeBuilder _codeBuilder;
    
    public InterpolatedCode(int literalLength, int formattedCount, CodeBuilder codeBuilder)
    {
        _codeBuilder = codeBuilder;
    }

    public void AppendLiteral(string str)
    {
        _codeBuilder.WriteIndentAwareText(str.AsSpan());
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> text)
    {
        _codeBuilder.WriteIndentAwareText(text);
    }

    public void AppendFormatted(string? str)
    {
        _codeBuilder.WriteIndentAwareText(str.AsSpan());
    }
    
    public void AppendFormatted<T>([AllowNull] T value)
    {
        _codeBuilder.WriteIndentAwareValue<T>(value);
    }
}