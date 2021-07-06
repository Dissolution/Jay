using System.Runtime.CompilerServices;

namespace Jay.UI.Terminal.Native
{
    public static class CoxelColorsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CoxelColors Combine(TerminalColor foreColor, TerminalColor backColor)
        {
            return (CoxelColors) ((int) backColor << 4 | (int) foreColor);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TerminalColor GetForeColor(this CoxelColors attributes)
        {
            // Get last four bits
            return (TerminalColor)((int)attributes & 0b00001111);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetForeColor(this ref CoxelColors attributes,
                                        TerminalColor color)
        {
            // Mask to clear the last four bits
            const int clearMask = 0b11110000;
            // Clear the last four bits and set them
            attributes = (CoxelColors) (((int) attributes & clearMask) | ((int) color));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TerminalColor GetBackColor(this CoxelColors attributes)
        {
            // Get first four bits
            return (TerminalColor)((int) attributes >> 4);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBackColor(this ref CoxelColors attributes,
                                        TerminalColor color)
        {
            // Mask to clear the first four bits
            const int clearMask = 0b00001111;
            // Clear the first four bits and set them
            attributes = (CoxelColors) (((int) attributes & clearMask) | ((int) color << 4));
        }
    }
}