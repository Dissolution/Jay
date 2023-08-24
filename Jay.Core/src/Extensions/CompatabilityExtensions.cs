namespace Jay.Extensions;

public static class CompatabilityExtensions
{
    static CompatabilityExtensions()
    {
        char[] chars = "ABC".ToCharArray();
        var span = chars.AsSpan(Range.All);

        var c = chars.Contains('a');
    }


#if NET48 || NETSTANDARD2_0
    public static Span<T> AsSpan<T>(this T[]? array, Range range)
    {
        (int offset, int length) = range.GetOffsetAndLength(array?.Length ?? 0);
        return array.AsSpan(offset, length);
    }
#endif
}