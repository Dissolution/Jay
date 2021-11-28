using System.Runtime.CompilerServices;

namespace Jay.Text.Extensions
{
    public static class CharExtensions
    {
        public static text ToReadOnlySpan(ref this char ch)
        {
            unsafe
            {
                return new text(Unsafe.AsPointer(ref ch), 1);
            }
        }
    }
}
