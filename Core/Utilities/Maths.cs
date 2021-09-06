using System.Numerics;

namespace Jay
{
    public static class Maths
    {
        /*
         * uint64_t next_pow2(uint64_t x) { 	return x == 1 ? 1 : 1<<(64-__builtin_clzl(x-1)); }
         * And for 32 bit :
         * uint32_t next_pow2(uint32_t x) { 	return x == 1 ? 1 : 1<<(32-__builtin_clz(x-1)); } T
         */

        public static ulong NextPowerOfTwo(ulong value)
        {
            if (value == 1UL)
                return 1UL;
            return 1UL << (64 - BitOperations.LeadingZeroCount(value - 1UL));
        }

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
}