namespace Jay.Text.Scratch;

public interface IFluentTextBuilder<B> : ITextBuilder
    where B : IFluentTextBuilder<B>
{
    B NewLine();

    B Append(char ch);
    B Append(scoped ReadOnlySpan<char> text);
    B Append(string? str);
    B Append(params char[]? characters);
    B Append(ref InterpolatedTextWriter interpolatedText);
    B Append<T>(T? value);
    B Append<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null);
    B Append<T>(T? value, string? format, IFormatProvider? provider = null);

    Span<char> GetSpan(Action<B> buildText);

    B Align(char ch, int width, Alignment alignment);
    B Align(scoped ReadOnlySpan<char> text, int width, Alignment alignment);
    B Align(string? str, int width, Alignment alignment);

    B Format(string format, params object?[] args);

    B Enumerate<T>(IEnumerable<T> items, Action<B, T> perItem);

    B Delimit<T>(Action<B> delimit, IEnumerable<T> items, Action<B, T> perItem);
    B Delimit<T>(char delimiter, IEnumerable<T> items, Action<B, T> perItem);
    B Delimit<T>(scoped ReadOnlySpan<char> delimiter, IEnumerable<T> items, Action<B, T> perItem);
    B Delimit<T>(string? delimiter, IEnumerable<T> items, Action<B, T> perItem);

    B If(bool result, Action<B>? ifTrue, Action<B>? ifFalse = null);
}