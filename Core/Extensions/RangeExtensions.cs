namespace Jay;

/// <summary>
/// Extensions on <see cref="Range"/>s.
/// </summary>
public static class RangeExtensions
{
    /// <summary>
    /// Gets a starting and ending index from a <see cref="Range"/> and a source <paramref name="length"/>.
    /// </summary>
    public static (int Start, int End) GetStartAndEnd(this Range range, int length)
    {
        (int startIndex, int len) = range.GetOffsetAndLength(length);
        int endIndex = startIndex + len;
        if ((uint)endIndex > (uint)length || (uint)startIndex > (uint)endIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }
        return (startIndex, endIndex);
    }
}