using System.Numerics;
using System.Runtime.CompilerServices;

namespace Jay;

public static class Maths
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HalfRoundUp(int value)
    {
        return (value >> 1) + (value & 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HalfRoundDown(int value)
    {
        return (value >> 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong NextPowerOfTwo(ulong value)
    {
        if (value == 1UL)
            return 1UL;
        return 1UL << (64 - BitOperations.LeadingZeroCount(value - 1UL));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint NextPowerOfTwo(uint value)
    {
        if (value == 1U)
            return 1U;
        return 1U << (32 - BitOperations.LeadingZeroCount(value - 1U));
    }

    /// <summary>
    /// Evaluate the binary logarithm of a non-zero Int32, with rounding up of fractional results.
    /// I.e. returns the exponent of the smallest power of two that is greater than or equal to the specified number.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2Ceiling(uint x)
    {
        // Log2(x) gives the required power of two, however this is integer Log2() therefore any fractional
        // part in the result is truncated, i.e., the result may be 1 too low. To compensate we add 1 if x
        // is not an exact power of two.

        // Return (exp + 1) if x is non-zero, and not an exact power of two.
        if (BitOperations.PopCount(x) > 1)
        {
            return BitOperations.Log2(x) + 1;
        }

        return BitOperations.Log2(x);
    }

    /// <summary>
    /// Evaluate the binary logarithm of a non-zero Int32, with rounding up of fractional results.
    /// I.e. returns the exponent of the smallest power of two that is greater than or equal to the specified number.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2Ceiling(ulong x)
    {
        // Log2(x) gives the required power of two, however this is integer Log2() therefore any fractional
        // part in the result is truncated, i.e., the result may be 1 too low. To compensate we add 1 if x
        // is not an exact power of two.

        // Return (exp + 1) if x is non-zero, and not an exact power of two.
        if (BitOperations.PopCount(x) > 1)
        {
            return BitOperations.Log2(x) + 1;
        }

        return BitOperations.Log2(x);
    }
}