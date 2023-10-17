namespace Jay.Text.Splitting;

/// <summary>
/// Extensions on <see cref="Span{T}">Span&lt;char&gt;</see> and
/// <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> to create
/// <see cref="TextSplitEnumerator"/>
/// </summary>
public static class SplitExtensions
{
    public static IReadOnlyList<Range> RangesToList(this TextSplitEnumerator textSplitEnumerator)
    {
        var ranges = new List<Range>();
        while (textSplitEnumerator.MoveNext())
        {
            ranges.Add(textSplitEnumerator.Range);
        }
        return ranges;
    }
    
    public static TextSplitEnumerator Split(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> separator,
        SplitOptions splitOptions = SplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return new TextSplitEnumerator(text, separator, splitOptions, stringComparison);
    }

    public static TextSplitEnumerator Split(
        this ReadOnlySpan<char> text,
        string? separator,
        SplitOptions splitOptions = SplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return Split(text, separator.AsSpan(), splitOptions, stringComparison);
    }

    public static TextSplitEnumerator Split(
        this Span<char> text,
        ReadOnlySpan<char> separator,
        SplitOptions splitOptions = SplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return Split((ReadOnlySpan<char>)text, separator, splitOptions, stringComparison);
    }

    public static TextSplitEnumerator Split(
        this Span<char> text,
        string? separator,
        SplitOptions splitOptions = SplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return Split((ReadOnlySpan<char>)text, separator.AsSpan(), splitOptions, stringComparison);
    }

    public static TextSplitEnumerator Split(
        this string? text,
        string? separator,
        SplitOptions splitOptions = SplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return Split(text.AsSpan(), separator.AsSpan(), splitOptions, stringComparison);
    }
    
    public static TextSplitEnumerator Split(
        this string? text,
        ReadOnlySpan<char> separator,
        SplitOptions splitOptions = SplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        return Split(text.AsSpan(), separator, splitOptions, stringComparison);
    }
}