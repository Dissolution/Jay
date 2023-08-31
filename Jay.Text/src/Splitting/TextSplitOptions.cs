namespace Jay.Text.Splitting;

[Flags]
public enum TextSplitOptions
{
    None = 0,
    /// <summary>
    /// Do not return any empty ranges
    /// </summary>
    RemoveEmptyLines = 1 << 0,
    /// <summary>
    /// Trim each range before returning
    /// </summary>
    TrimLines = 1 << 1,
}