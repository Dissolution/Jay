namespace Jay.Text;

/// <summary>
/// Textual alignment
/// </summary>
/// <remarks>
/// This has <see cref="FlagsAttribute"/> because
/// <c>Left|Center</c> indicates a left-bias for centering and
/// <c>Right|Center</c> indicates a right-bias.
/// </remarks>
[Flags]
public enum Alignment
{
    /// <summary>
    /// Align text to the left (spaces to the right)
    /// </summary>
    Left = 1 << 0,
    /// <summary>
    /// Align text to the right (spaces to the left)
    /// </summary>
    Right = 1 << 1,
    /// <summary>
    /// Align text in the center (spaces to the sides)
    /// </summary>
    Center = 1 << 2,
}