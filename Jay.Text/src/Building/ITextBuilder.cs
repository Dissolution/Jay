namespace Jay.Text.Building;

public interface ITextBuilder<out B> : IBuildingText
    where B : ITextBuilder<B>
{
    B NewLine();
    B NewLines(int count);

    B Append(char ch);
    B Append(scoped ReadOnlySpan<char> text);
    B Append(string? str);
    B Append(params char[]? characters);
    B Append(ref InterpolatedTextWriter interpolatedText);
    B Append<T>(T? value);
    B Append<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null);
    B Append<T>(T? value, string? format, IFormatProvider? provider = null);

    B Align(char ch, int width, Alignment alignment);
    B Align(scoped ReadOnlySpan<char> text, int width, Alignment alignment);
    B Align(string? str, int width, Alignment alignment);

    B Format(NonFormattableString format, params object?[] args);
    B Format(FormattableString formattableString);
    B Format(ref InterpolatedTextWriter interpolatedText);

    B Enumerate<T>(ReadOnlySpan<T> values, Action<B, T> perValue);
    B Enumerate<T>(IEnumerable<T> values, Action<B, T> perValue);
    B Iterate<T>(ReadOnlySpan<T> values, Action<B, T, int> perValueIndex);
    B Iterate<T>(IEnumerable<T> values, Action<B, T, int> perValueIndex);
    B Delimit<T>(Action<B> delimit, ReadOnlySpan<T> values, Action<B, T> perValue);
    B Delimit<T>(Action<B> delimit, IEnumerable<T> values, Action<B, T> perValue);

    B If(bool result, Action<B>? ifTrue, Action<B>? ifFalse = null);
    B Invoke(Action<B> build);
    B GetWritten(Action<B> build, out Span<char> written);
}

public static class TextBuilderExtensions
{
    public static B EnumerateAppend<B, T>(this B textBuilder, IEnumerable<T> enumerable)
        where B : ITextBuilder<B>
        => textBuilder.Enumerate<T>(enumerable, static (tb, value) => tb.Append<T>(value));

    public static B EnumerateAppendLines<B, T>(this B textBuilder, IEnumerable<T> enumerable)
        where B : ITextBuilder<B>
        => textBuilder.Enumerate<T>(enumerable, static (tb, value) => tb.Append<T>(value).NewLine());


    public static B Delimit<B, T>(this B textBuilder, char delimiter, IEnumerable<T> items, Action<B, T> perItem)
        where B : ITextBuilder<B>
        => textBuilder.Delimit(tb => tb.Append(delimiter), items, perItem);

    public static B Delimit<B, T>(this B textBuilder, string? delimiter, IEnumerable<T> items, Action<B, T> perItem)
        where B : ITextBuilder<B>
        => textBuilder.Delimit(tb => tb.Append(delimiter), items, perItem);
}