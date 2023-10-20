namespace Jay.Reflection.CodeBuilding;

[InterpolatedStringHandler]
public ref struct InterpolatedCode
{
    private CodeBuilder _codeBuilder;

    public InterpolatedCode()
    {
        _codeBuilder = new();
    }

    public InterpolatedCode(int literalLength, int formattedCount)
    {
        _codeBuilder = new();
    }

    public InterpolatedCode(int literalLength, int formattedCount, CodeBuilder codeBuilder)
    {
        _codeBuilder = codeBuilder;
    }

    public void AppendLiteral(string str)
    {
        _codeBuilder.Write(str);
    }

    public void AppendFormatted<T>([AllowNull] T value)
    {
        _codeBuilder.Write<T>(value);
    }

    public void Dispose()
    {
        _codeBuilder.Dispose();
    }

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }
    
    public override string ToString()
    {
        return _codeBuilder.ToString();
    }
}