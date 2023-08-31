namespace Jay.Text.Splitting;

/// <summary>
/// Extensions on <see cref="Span{T}">Span&lt;char&gt;</see> and
/// <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> to create
/// <see cref="TextSplitEnumerable"/>
/// </summary>
public static class SplitExtensions
{
    public static TextSplitEnumerable TextSplit(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return new TextSplitEnumerable(text, separator, splitOptions, stringComparison);
    }

    public static TextSplitEnumerable TextSplit(
        this ReadOnlySpan<char> text,
        string? separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return TextSplit(text, separator.AsSpan(), splitOptions, stringComparison);
    }

    public static TextSplitEnumerable TextSplit(
        this Span<char> text,
        ReadOnlySpan<char> separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return TextSplit((ReadOnlySpan<char>)text, separator, splitOptions, stringComparison);
    }

    public static TextSplitEnumerable TextSplit(
        this Span<char> text,
        string? separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return TextSplit((ReadOnlySpan<char>)text, separator.AsSpan(), splitOptions, stringComparison);
    }

    public static TextSplitEnumerable TextSplit(
        this string? text,
        string? separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return TextSplit(text.AsSpan(), separator.AsSpan(), splitOptions, stringComparison);
    }
    
    public static TextSplitEnumerable TextSplit(
        this string? text,
        ReadOnlySpan<char> separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return TextSplit(text.AsSpan(), separator, splitOptions, stringComparison);
    }
}