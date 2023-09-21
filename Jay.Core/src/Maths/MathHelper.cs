#if NETCOREAPP3_1_OR_GREATER
using System.Numerics;

#else
using System.Runtime.InteropServices;
#endif

namespace Jay.Maths;

/// <summary>
/// Math and number related utilities
/// </summary>
public static class MathHelper
{
#if !NETCOREAPP3_1_OR_GREATER
    private static ReadOnlySpan<byte> Log2DeBruijn => new byte[32]
    {
        00, 09, 01, 10, 13, 21, 02, 29,
        11, 14, 16, 18, 22, 25, 03, 30,
        08, 12, 20, 28, 15, 17, 24, 07,
        19, 27, 23, 06, 26, 05, 04, 31,
    };

    private static int Log2SoftwareFallback(uint value)
    {
        // No AggressiveInlining due to large method size
        // Has conventional contract 0->0 (Log(0) is undefined)

        // Fill trailing zeros with ones, eg 00010010 becomes 00011111
        value |= value >> 01;
        value |= value >> 02;
        value |= value >> 04;
        value |= value >> 08;
        value |= value >> 16;

        // uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
        return Unsafe.AddByteOffset(
            // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_1100_0100_1010_1100_1101_1101u
            ref MemoryMarshal.GetReference(Log2DeBruijn),
            // uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
            (IntPtr)(value * 0x07C4ACDDu >> 27));
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateLeft(uint value, int offset)
    {
        return value << offset | value >> 32 - offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RotateLeft(ulong value, int offset)
    {
        return value << offset | value >> 64 - offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(uint value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return BitOperations.LeadingZeroCount(value);
#else
        // Unguarded fallback contract is 0->31, BSR contract is 0->undefined
        if (value == 0)
        {
            return 32;
        }

        return 31 ^ Log2SoftwareFallback(value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ulong value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return BitOperations.LeadingZeroCount(value);
#else
        uint hi = (uint)(value >> 32);

        if (hi == 0)
        {
            return 32 + LeadingZeroCount((uint)value);
        }

        return LeadingZeroCount(hi);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ulong value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return BitOperations.PopCount(value);
#else
#if TARGET_32BIT
        return PopCount((uint)value) // lo
            + PopCount((uint)(value >> 32)); // hi
#else
        return softwareFallback(value);

        static int softwareFallback(ulong value)
        {
            const ulong C1 = 0x_55555555_55555555ul;
            const ulong C2 = 0x_33333333_33333333ul;
            const ulong C3 = 0x_0F0F0F0F_0F0F0F0Ful;
            const ulong C4 = 0x_01010101_01010101ul;

            value -= (value >> 1) & C1;
            value = (value & C2) + ((value >> 2) & C2);
            value = (((value + (value >> 4)) & C3) * C4) >> 56;

            return (int)value;
        }
#endif
#endif
    }


    /// <summary>
    /// Half of <paramref name="value"/>, rounded up
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HalfRoundUp(int value)
    {
        return (value >> 1) + (value & 1);
    }

    /// <summary>
    /// Half of <paramref name="value"/>, rounded down
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HalfRoundDown(int value)
    {
        return value >> 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong NextPowerOfTwo(ulong value)
    {
        if (value == 1UL)
            return 1UL;
        return 1UL << 64 - LeadingZeroCount(value - 1UL);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint NextPowerOfTwo(uint value)
    {
        if (value == 1U)
            return 1U;
        return 1U << 32 - LeadingZeroCount(value - 1U);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(uint value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return BitOperations.Log2(value);
#else
        // The 0->0 contract is fulfilled by setting the LSB to 1.
        // Log(1) is 0, and setting the LSB for values > 1 does not change the log2 result.
        value |= 1;

        // Fallback contract is 0->0
        return Log2SoftwareFallback(value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ulong value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return BitOperations.Log2(value);
#else
        value |= 1;

        uint hi = (uint)(value >> 32);

        if (hi == 0)
        {
            return Log2((uint)value);
        }

        return 32 + Log2(hi);
#endif
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
        int result = Log2(value);
        if (PopCount(value) != 1)
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
        int result = Log2(value);
        if (PopCount(value) != 1)
        {
            result++;
        }
        return result;
    }

    /// <summary>
    /// 2 ^ <paramref name="exponent"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PowerOfTwo(int exponent)
    {
        return 1 << exponent;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int value, int min)
    {
        if (value >= min)
        {
            return value;
        }
        return min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int value, int min, int max)
    {
#if NET48 || NETSTANDARD2_0
        if (min > max)
        {
            throw new ArgumentException("Max must not be greater than min", nameof(max));
        }

        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
#else
        return Math.Clamp(value, min, max);
#endif
    }
}