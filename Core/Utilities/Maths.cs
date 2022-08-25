using System.Numerics;

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
    /// <param name="value">The input value.</param>
    /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2Ceiling(uint value)
    {
        int result = BitOperations.Log2(value);
        if (BitOperations.PopCount(value) != 1)
        {
            result++;
        }
        return result;
    }

    /// <summary>
    /// Evaluate the binary logarithm of a non-zero Int32, with rounding up of fractional results.
    /// I.e. returns the exponent of the smallest power of two that is greater than or equal to the specified number.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2Ceiling(ulong value)
    {
        int result = BitOperations.Log2(value);
        if (BitOperations.PopCount(value) != 1)
        {
            result++;
        }
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PowerOfTwo(int exponent)
    {
        return 1 << exponent;
    }
}