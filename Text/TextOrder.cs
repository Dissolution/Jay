namespace Jay.Text;

/// <summary>
/// The order text is read/processed
/// </summary>
public enum TextOrder : byte
{
    /// <summary>
    /// From front to back ([0..])
    /// </summary>
    LeftToRight = 0,

    /// <summary>
    /// From back to front ([..^1])
    /// </summary>
    RightToLeft = 1,
}
