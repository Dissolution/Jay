namespace Jay.Text.Splitting;

/// <summary>
/// An <see cref="IEnumerable{T}">IEnumerable&lt;ReadOnlySpan&lt;char&gt;&gt;</see>
/// </summary>
public readonly ref struct TextSplitEnumerable 
    // : IEnumerable<ReadOnlySpan<char>>, IEnumerable
{
    public readonly ReadOnlySpan<char> InputText;
    public readonly ReadOnlySpan<char> Separator;
    public readonly TextSplitOptions SplitOptions;
    public readonly StringComparison StringComparison;

    public TextSplitEnumerable(
        ReadOnlySpan<char> inputText,
        ReadOnlySpan<char> separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison stringComparison = StringComparison.Ordinal
    )
    {
        InputText = inputText;
        Separator = separator;
        SplitOptions = splitOptions;
        StringComparison = stringComparison;
    }

    /// <summary>
    /// Consume this <see cref="TextSplitEnumerable"/> into a <see cref="IReadOnlyList{T}">IReadOnlyList&lt;string&gt;</see>
    /// </summary>
    public IReadOnlyList<string> ToListOfStrings()
    {
        var e = GetEnumerator();
        var strings = new List<string>();
        while (e.MoveNext())
        {
            strings.Add(e.CurrentString);
        }
        return strings;
    }

    /// <summary>
    /// Consume this <see cref="TextSplitEnumerable"/> into a <see cref="TextSplitList"/>
    /// </summary>
    public TextSplitList ToList()
    {
        List<Range> ranges = new();
        var e = GetEnumerator();
        while (e.MoveNext())
        {
            ranges.Add(e.Range);
        }
        return new TextSplitList(InputText, ranges);
    }


    /// <inheritdoc cref="IEnumerable{T}"/>
    public TextSplitEnumerator GetEnumerator()
    {
        return new TextSplitEnumerator(this);
    }
}