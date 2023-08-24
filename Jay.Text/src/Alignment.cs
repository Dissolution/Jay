namespace Jay.Text;

/// <summary>
/// Textual alignment
/// </summary>
/// <remarks>
/// This is <see cref="FlagsAttribute"/> because Left|Center indicates a left-bias for a centering operation and
/// Right|Center indicates a right-bias
/// </remarks>
[Flags]
public enum Alignment
{
    // None = 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Center = 1 << 2,
    // Left | Center,
    // Right | Center,
}