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
    }
}