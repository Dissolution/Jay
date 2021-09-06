using System;
using System.Runtime.CompilerServices;

namespace Jay
{
    /// <summary>
    /// Extensions on <see cref="System.Range"/>s.
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// Gets the starting and ending indexes for this <see cref="Range"/> over the <paramref name="available"/> length.
        /// </summary>
        /// <param name="range">The <see cref="Range"/> to get the start and end indexes from.</param>
        /// <param name="available">The total available length the <paramref name="range"/> will apply to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="available"/> is less than 0.</exception>
        /// <returns>The starting and ending indexes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int Start, int End) GetStartAndEnd(this Range range, int available)
        {
            if (available < 0)
                throw new ArgumentOutOfRangeException(nameof(available), available, "Available must be 0 or greater.");
            (int offset, int length) = range.GetOffsetAndLength(available);
            return (offset, offset + length);
        }
    }
}