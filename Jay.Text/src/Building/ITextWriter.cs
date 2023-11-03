namespace Jay.Text.Building;


public interface ITextWriter : IBuildingText
{
    void Write(char ch);
    void Write(scoped ReadOnlySpan<char> text);
    void Write(string? str);
    void Write(params char[]? characters);
    void Write(ref InterpolatedTextWriter interpolatedText);
    void Write<T>(T? value);
    void Write<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null);
    void Write<T>(T? value, string? format, IFormatProvider? provider = null);

    void WriteLine();
}