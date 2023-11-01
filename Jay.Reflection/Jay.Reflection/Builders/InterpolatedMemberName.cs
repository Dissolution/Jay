namespace Jay.Reflection.Builders;

[InterpolatedStringHandler]
public ref struct InterpolatedMemberName
{
    private InterpolatedText _interpolatedText;

    public InterpolatedMemberName(int literalLength, int formattedCount)
    {
        _interpolatedText = new(literalLength, formattedCount);
    }

    public void AppendLiteral(string str) => _interpolatedText.AppendLiteral(str);

    public void AppendFormatted<T>(T? value)
    {
        if (value is Type type)
        {
            _interpolatedText.AppendLiteral(type.NameOf());
        }
        else
        {
            _interpolatedText.AppendFormatted<T>(value);
        }
    }

    public string ToStringAndDispose() => _interpolatedText.ToStringAndDispose();

    public void Dispose() => _interpolatedText.Dispose();

    public override string ToString() => _interpolatedText.ToString();
}