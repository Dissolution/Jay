namespace Jay.CodeGen.CSharpCode;

[InterpolatedStringHandler]
public ref struct InterpolatedCode
{
    private CodeBuilder _codeBuilder;
    
    
    public InterpolatedCode(int literalLength, int formattedCount)
    {
        _codeBuilder = new();
    }

    public void AppendLiteral(string literal)
    {
        _codeBuilder.Append(literal);
    }

    public void AppendFormatted<T>(T value)
    {
        _codeBuilder.Append<T>(value);
    }

    public void Dispose()
    {
        this = default;
    }

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override bool Equals(object? obj) => throw new NotSupportedException();
    
    public override int GetHashCode() => throw new NotSupportedException();
    
    public override string ToString()
    {
        return _codeBuilder?.ToString() ?? "";
    }
}