using System;
using Jay.Text;

namespace Jay;

public static class CharExtensions
{
    /// <summary>
    /// Converts this <see cref="char"/> into a <see cref="ReadOnlySpan{T}"/>
    /// </summary>
    /// <param name="ch"></param>
    /// <returns></returns>
    public static ReadOnlySpan<char> ToReadOnlySpan(ref this char ch)
    {
        // Tested fastest
        unsafe
        {
            return new ReadOnlySpan<char>(Unsafe.AsPointer(in ch), 1);
        }
    }
}