namespace Jay.SourceGen.Extensions;

public static class TextExtensions
{
    public static int NextIndexOf(
        this ReadOnlySpan<char> text,
        ReadOnlySpan<char> searchText,
        int startIndex,
        StringComparison comparison = StringComparison.Ordinal)
    {
        if ((uint)startIndex >= text.Length)
            return -1;

        var sliceIndex = text.Slice(startIndex).IndexOf(searchText, comparison);
        if (sliceIndex == -1)
            return -1;

        return sliceIndex + startIndex;
    }
}