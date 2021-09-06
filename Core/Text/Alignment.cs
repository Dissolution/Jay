using System;

namespace Jay.Text
{
    /// <summary>
    /// Text Alignment
    /// </summary>
    /// <remarks>
    /// This has <see cref="FlagsAttribute"/> because Left|Center and Right|Center can be hints when the number of characters doesn't evenly align.
    /// </remarks>
    [Flags]
    public enum Alignment
    {
        /// <summary>
        /// Left Aligned
        /// </summary>
        Left = 1 << 0,

        /// <summary>
        /// Center Justified
        /// </summary>
        Center = 1 << 1,

        /// <summary>
        /// Right Aligned
        /// </summary>
        Right =  1<< 2,
    }
}