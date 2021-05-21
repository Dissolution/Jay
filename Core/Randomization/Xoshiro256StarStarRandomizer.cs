using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Randomization
{
    
    internal partial class Xoshiro256StarStarRandomizer
    {
        //Multiplier to convert a ulong to a float
        protected const float FloatMultiplier = 1.0f / (1U << 24);
        //Multiplier to convert a ulong to a double
        protected const double DoubleMultiplier = 1.0d / ((1UL << 53) + 1);

        
        /// <summary>
        /// SplitMix64 Pseudo-Random Number Generator
        /// </summary>
        /// <param name="state">Starting RNG state. This can take any value, including zero, and will be updated to the next state.</param>
        /// <returns>A pseudo-random <see cref="ulong"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong SplitMixNext(ref ulong state)
        {
            ulong z = (state += 0x9E3779B97F4A7C15UL);
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }

        /// <remarks>RyuJIT will compile this to a single rotate CPU instruction (as of about .NET 4.6.1 and dotnet core 2.0).</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateLeft(ulong x, int k) => (x << k) | (x >> (64 - k));

        /// <summary>
        /// Evaluate the binary logarithm of a non-zero <see cref="uint"/>, with rounding up of fractional results.
        /// I.e. returns the exponent of the smallest power of two that is greater than or equal to the specified number.
        /// </summary>
        /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Log2Ceiling(uint x)
        {
            // Log2(x) gives the required power of two, however this is integer Log2() therefore any fractional
            // part in exp is truncated, i.e. the result may be 1 too low. Thus, if 2^exp == x, then x is an exact 
            // power of two and exp is correct, otherwise exp + 1 gives the correct value.
            int exp = BitOperations.Log2(x);

            // Calc x1 = 2^exp
            int x1 = 1 << exp;

            // Return exp + 1 if x is not an exact power of two.
            return (x == x1) ? exp : exp + 1;
        }

        /// <summary>
        /// Evaluate the binary logarithm of a non-zero <see cref="ulong"/>, with rounding up of fractional results.
        /// I.e. returns the exponent of the smallest power of two that is greater than or equal to the specified number.
        /// </summary>
        /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Log2Ceiling(ulong x)
        {
            // Log2(x) gives the required power of two, however this is integer Log2() therefore any fractional
            // part in exp is truncated, i.e. the result may be 1 too low. Thus, if 2^exp == x, then x is an exact 
            // power of two and exp is correct, otherwise exp + 1 gives the correct value.
            int exp = BitOperations.Log2(x);

            // Calc x1 = 2^exp
            // Return exp + 1 if x is not an exact power of two.
            return (x == (1UL << exp)) ? exp : exp + 1;
        }
    }

    internal partial class Xoshiro256StarStarRandomizer : IRandomizer
    {
        //State #0
        private ulong _s0;
        //State #1
        private ulong _s1;
        //State #2
        private ulong _s2;
        //State #3
        private ulong _s3;

        public RandomizerMode Mode { get; }

        /// <summary>
        /// Gets the initial starting seed.
        /// </summary>
        public ulong Seed { get; }

        
        /// <summary>
        /// Construct a new <see cref="Randomizer"/> with a random seed.
        /// </summary>
        /// <param name="mode">The optional <see cref="RandomizerMode"/>.</param>
        public Xoshiro256StarStarRandomizer(RandomizerMode mode = RandomizerMode.Fast)
            : this(Randomizer.GetCryptoRandomSeed<ulong>(), mode) { }

        /// <summary>
        /// Construct a new <see cref="Randomizer"/> with a given <paramref name="seed"/>.
        /// </summary>
        /// <param name="seed">The initial seed for randomization.</param>
        /// <param name="mode">The optional <see cref="RandomizerMode"/>.</param>
        public Xoshiro256StarStarRandomizer(ulong seed, RandomizerMode mode = RandomizerMode.Fast)
        {
            this.Seed = seed;
            this.Mode = mode;

            /* Notes.
             * The first random sample will be very strongly correlated to the value we give to the 
             * state variables here; such a correlation is undesirable, therefore we significantly 
             * weaken it by hashing the seed's bits using the splitmix64 PRNG.
             *
             * It is required that at least one of the state variables be non-zero;
             * use of splitmix64 satisfies this requirement because it is an equi-distributed generator,
             * thus if it outputs a zero it will next produce a zero after a further 2^64 outputs.
             *
             * Use the splitmix64 RNG to hash the seed.
             */
            _s0 = SplitMixNext(ref seed);
            _s1 = SplitMixNext(ref seed);
            _s2 = SplitMixNext(ref seed);
            _s3 = SplitMixNext(ref seed);
        }
        
        /// <summary>
        /// Generates a <see cref="ulong"/> between 0 and the given <paramref name="inclusiveMax"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ZeroTo(ulong exclusiveMaximum)
        {
            /* A fast approach is:
               * I.e. generate a double precision float in the interval [0,1) and multiply by inclusiveMax.
               * However the use of floating point arithmetic will introduce bias.
               */
            if (Mode == RandomizerMode.Fast)
            {
                return (ulong) (PercentDouble() * exclusiveMaximum);
            }

            /* Rejection sampling is used in order to produce unbiased samples.
             * The rejection sampling method used here operates as follows:
             *
             *  1) Calculate N such that  2^(N-1) < maxValue <= 2^N,
             *     i.e. N is the inclusiveMinimum number of bits required to represent inclusiveMax states.
             *  2) Generate an N bit random sample.
             *  3) Reject samples that are > inclusiveMax, and goto (2) to resample.
             *
             * Repeat until a valid sample is generated.
             */

            // Rejection sampling loop.
            // Note. The expected number of samples per generated value is approx. 1.3862,
            // i.e. the number of loops, on average, assuming a random and uniformly distributed maxValue.
            ulong rand;
            do
            {
                rand = ULong();
                if (rand < exclusiveMaximum)
                    return rand;
            } while (true);
        }
        
         /// <summary>
        /// Generates a random <see cref="bool"/> value.
        /// </summary>
        public bool Bool()
        {
            // Use a high bit since the low bits are linear-feedback shift registers (LFSRs) with low degree.
            // This is slower than the approach of generating and caching 64 bits for future calls, but 
            // (A) gives good quality randomness, and (B) is still very fast.
            return (ULong() & 0b_10000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000) == 0;
        }

        /// <summary>
        /// Generates a random <see cref="byte"/> value.
        /// </summary>
        public byte Byte()
        {
            // Note:
            // Here we shift right to use the 8 most significant bits because these exhibit higher quality randomness than the lower bits.
            return (byte)(ULong() >> 56);
        }

        /// <summary>
        /// Generates a random <see cref="sbyte"/> value.
        /// </summary>
        public sbyte SByte()
        {
            // Note:
            // Here we shift right to use the 8 most significant bits because these exhibit higher quality randomness than the lower bits.
            return (sbyte)(ULong() >> 56);
        }

        /// <summary>
        /// Generates a random <see cref="short"/> value.
        /// </summary>
        public short Short()
        {
            // Note:
            // Here we shift right to use the 16 most significant bits because these exhibit higher quality randomness than the lower bits.
            return (short)(ULong() >> 48);
        }

        /// <summary>
        /// Generates a random <see cref="ushort"/> value.
        /// </summary>
        public ushort UShort()
        {
            // Note:
            // Here we shift right to use the 16 most significant bits because these exhibit higher quality randomness than the lower bits.
            return (ushort)(ULong() >> 48);
        }

        /// <summary>
        /// Generates a random <see cref="char"/> value.
        /// </summary>
        public char Char()
        {
            //char == ushort
            return (char)(ULong() >> 48);
        }

        /// <summary>
        /// Generates a random <see cref="int"/> value.
        /// </summary>
        public int Int()
        {
            // Generate 64 random bits and shift right to leave the most significant 32 bits.
            // Note. Shift right is used instead of a mask because the high significant bits 
            // exhibit higher quality randomness compared to the lower bits.
            return (int)(ULong() >> 32);
        }

        /// <summary>
        /// Generates a <see cref="int"/> value in the interval [0, <see cref="int.MaxValue"/>].
        /// </summary>
        public int PositiveInt()
        {
            // Generate 64 random bits and shift right to leave the most significant 31 bits.
            // Bit 32 is the sign bit so must be zero to avoid negative results.
            // Note. Shift right is used instead of a mask because the high significant bits 
            // exhibit higher quality randomness compared to the lower bits.
            return (int) (ULong() >> 33);
        }

        /// <summary>
        /// Generates a random <see cref="uint"/> value.
        /// </summary>
        public uint UInt() => (uint) ULong();

        /// <summary>
        /// Generates a random <see cref="long"/> value.
        /// </summary>
        public long Long() => (long)ULong();
        
        /// <summary>
        /// Generates a <see cref="ulong"/> value in the interval [<see cref="ulong.MinValue"/>, <see cref="ulong.MaxValue"/>].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ULong()
        {
            // For improved performance the below loop operates on these stack allocated copies of the heap variables.
            // Note. doing this means that these heavily used variables are located near to other local/stack variables,
            // thus they will very likely be cached in the same CPU cache line.
            ulong s0 = _s0;
            ulong s1 = _s1;
            ulong s2 = _s2;
            ulong s3 = _s3;

            // Generate a further 64 random bits.
            ulong result = RotateLeft(s1 * 5, 7) * 9;

            // Update PRNG state.
            ulong t = s1 << 17;
            s2 ^= s0;
            s3 ^= s1;
            s1 ^= s2;
            s0 ^= s3;
            s2 ^= t;
            s3 = RotateLeft(s3, 45);

            // Update the state variables on the heap.
            _s0 = s0;
            _s1 = s1;
            _s2 = s2;
            _s3 = s3;

            return result;
        }

        public TimeSpan TimeSpan() => System.TimeSpan.FromTicks((long)ULong());

        public DateTime DateTime(DateTimeKind kind = DateTimeKind.Local) => new DateTime((long) ULong(), kind);

        public DateTimeOffset DateTimeOffset() => new DateTimeOffset((long) ULong(), System.TimeSpan.Zero);

        public DateTimeOffset DateTimeOffset(TimeSpan offset) => new DateTimeOffset((long) ULong(), offset);

        public Guid Guid()
        {
            Span<byte> bytes = new byte[16];
            Fill(bytes);
            return new Guid(bytes);
        }
        
        /// <summary>
        /// Generates a random <see cref="float"/> in the interval [0f..1f)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float PercentFloat()
        {
            if (Mode == RandomizerMode.Fast)
            {
                // Note. Here we generate a random integer between 0 and 2^24-1 (i.e. 24 binary 1s) and multiply
                // by the fractional unit value 1.0 / 2^24, thus the result has a max value of
                // 1.0 - (1.0 / 2^24). Or 0.99999994 in decimal.
                return (ULong() >> 40) * (1.0f / (1 << 24));
            }

            //TODO: Hacky like PercentDouble for floats?
            return (float) PercentDouble();
        }

        /// <summary>
        /// Generate a random <see cref="double"/> in the interval [0d, 1d)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double PercentDouble()
        {
            if (Mode == RandomizerMode.Fast)
            {
                /*
                 * random_real_53: Pick an integer in {0, 1, ..., 2^53 - 1} uniformly
                 * at random, convert it to double, and divide it by 2^53.  Many
                 * possible outputs are not represented: 2^-54, 1, &c.  There are a
                 * little under 2^62 floating-point values in [0, 1], but only 2^53
                 * possible outputs here.
                 */
                return (double)(ULong() & ((1UL << 53) - 1UL)) / (double)(1UL << 53);
            }

            // Uses an alternative sampling method that is capable of generating all possible values in the
            // interval [0,1] that can be represented by a double precision float. Note however that this method 
            // is significantly slower than NextDouble().

            // Notes.
            // An alternative sampling method from:
            // 
            //    2014, Taylor R Campbell
            //
            //    Uniform random floats:  How to generate a double-precision
            //    floating-point number in [0, 1] uniformly at random given a uniform
            //    random source of bits.
            //
            //    https://mumble.net/~campbell/tmp/random_real.c
            //
            // The basic idea is that we generate a string of binary digits and use them to construct a 
            // base two number of the form:
            //
            //    0.{digits}
            //
            // The digits are generated in blocks of 64 bits. If all 64 bits in a block are zero then a 
            // running exponent value is reduced by 64 and another 64 bits are generated. This process is
            // repeated until a block with non-zero bits is produced, or the exponent value falls below -1074.
            //
            // The final step is to create the IEE754 double precision variable from a 64 bit significand
            // (the most recent and thus significant 64 bits), and the running exponent.
            //
            // This scheme is capable of generating all possible values in the interval [0,1) that can be 
            // represented by a double precision float, and without bias. There are a little under 2^62
            // possible discrete values, and this compares to the 2^53 possible values than can be generated by 
            // NextDouble(), however the scheme used in this method is much slower, so is likely of interest
            // in specialist scenarios.

            int exponent = -64;
            ulong significand;

            // Read zeros into the exponent until we hit a one; the rest
            // will go into the significand.
            while ((significand = ULong()) == 0)
            {
                exponent -= 64;

                // If the exponent falls below -1074 = emin + 1 - p,
                // the exponent of the smallest subnormal, we are
                // guaranteed the result will be rounded to zero.  This
                // case is so unlikely it will happen in realistic
                // terms only if random64 is broken.
                if (exponent < -1074)
                    return 0d;
            }


            // There is a 1 somewhere in significand, not necessarily in
            // the most significant position.  If there are leading zeros,
            // shift them into the exponent and refill the less-significant
            // bits of the significand.  Can't predict one way or another
            // whether there are leading zeros: there's a fifty-fifty
            // chance, if random64 is uniformly distributed.
            int shift = BitOperations.LeadingZeroCount(significand);
            if (shift != 0)
            {
                exponent -= shift;
                significand <<= shift;
                significand |= (ULong() >> (64 - shift));
            }

            // Set the sticky bit, since there is almost surely another 1
            // in the bit stream.  Otherwise, we might round what looks
            // like a tie to even when, almost surely, were we to look
            // further in the bit stream, there would be a 1 breaking the
            // tie.
            significand |= 1;

            // Finally, convert to double (rounding) and scale by
            // 2^exponent.
            return (double)significand * (1L << exponent);
        }
        
        /// <summary>
        /// Generates a random <see cref="byte"/> between the given inclusive <paramref name="minimum"/> and inclusive <paramref name="maximum"/>s.
        /// </summary>
        public byte Between(byte inclusiveMinimum, byte inclusiveMaximum)
        {
            if (inclusiveMinimum > inclusiveMaximum)
                throw new ArgumentOutOfRangeException(nameof(inclusiveMaximum), inclusiveMaximum, "Maximum must be greater than or equal to minimum");
            if (inclusiveMinimum == inclusiveMaximum) 
                return inclusiveMinimum;
            ulong range = ((ulong) inclusiveMaximum - (ulong) inclusiveMinimum) + 1UL;
            ulong under = ZeroTo(range);
            return (byte) (under + (ulong)inclusiveMinimum);
        }
         
        /// <summary>
        /// Generates a random <see cref="byte"/> between the given inclusive <paramref name="minimum"/> and inclusive <paramref name="maximum"/>s.
        /// </summary>
        public sbyte Between(sbyte inclusiveMinimum, sbyte inclusiveMaximum)
        {
            if (inclusiveMinimum > inclusiveMaximum)
                throw new ArgumentOutOfRangeException(nameof(inclusiveMaximum), inclusiveMaximum, "Maximum must be greater than or equal to minimum");
            if (inclusiveMinimum == inclusiveMaximum) 
                return inclusiveMinimum;
            ulong range = (ulong)((long)inclusiveMaximum - (long)inclusiveMinimum) + 1UL;
            ulong under = ZeroTo(range);
            return (sbyte) ((long)under + (long)inclusiveMinimum);
        }

        /// <summary>
        /// Generates a random <see cref="short"/> between the given inclusive <paramref name="minimum"/> and inclusive <paramref name="maximum"/>s.
        /// </summary>
        public short Between(short inclusiveMinimum, short inclusiveMaximum)
        {
            if (inclusiveMinimum > inclusiveMaximum)
                throw new ArgumentOutOfRangeException(nameof(inclusiveMaximum), inclusiveMaximum, "Maximum must be greater than or equal to minimum");
            if (inclusiveMinimum == inclusiveMaximum) 
                return inclusiveMinimum;
            ulong range = (ulong)((long)inclusiveMaximum - (long)inclusiveMinimum) + 1UL;
            ulong under = ZeroTo(range);
            return (short) ((long)under + (long)inclusiveMinimum);
        }

        /// <summary>
        /// Generates a random <see cref="ushort"/> between the given inclusive <paramref name="minimum"/> and inclusive <paramref name="maximum"/>s.
        /// </summary>
        public ushort Between(ushort inclusiveMinimum = ushort.MinValue, ushort inclusiveMaximum = ushort.MaxValue)
        {
            if (inclusiveMinimum > inclusiveMaximum)
                throw new ArgumentOutOfRangeException(nameof(inclusiveMaximum), inclusiveMaximum, "Maximum must be greater than or equal to minimum");
            if (inclusiveMinimum == inclusiveMaximum) 
                return inclusiveMinimum;
            ulong range = ((ulong) inclusiveMaximum - (ulong) inclusiveMinimum) + 1UL;
            ulong under = ZeroTo(range);
            return (ushort) (under + (ulong)inclusiveMinimum);
        }
        
        /// <summary>
        /// Generates a random <see cref="char"/> between the given inclusive <paramref name="minimum"/> and inclusive <paramref name="maximum"/>s.
        /// </summary>
        public char Between(char inclusiveMinimum, char inclusiveMaximum)
        {
            if (inclusiveMinimum > inclusiveMaximum)
                throw new ArgumentOutOfRangeException(nameof(inclusiveMaximum), inclusiveMaximum, "Maximum must be greater than or equal to minimum");
            if (inclusiveMinimum == inclusiveMaximum) 
                return inclusiveMinimum;
            ulong range = ((ulong) inclusiveMaximum - (ulong) inclusiveMinimum) + 1UL;
            ulong under = ZeroTo(range);
            return (char) (under + (ulong)inclusiveMinimum);
        }

        /// <summary>
        /// Generates a random <see cref="int"/> between the given inclusive <paramref name="minimum"/> and inclusive <paramref name="maximum"/>s.
        /// </summary>
        public int Between(int inclusiveMinimum = int.MinValue, int inclusiveMaximum = int.MaxValue)
        {
            if (inclusiveMinimum > inclusiveMaximum)
                throw new ArgumentOutOfRangeException(nameof(inclusiveMaximum), inclusiveMaximum, "Maximum must be greater than or equal to minimum");
            if (inclusiveMinimum == inclusiveMaximum) 
                return inclusiveMinimum;
            ulong range = (ulong)((long)inclusiveMaximum - (long)inclusiveMinimum) + 1UL;
            ulong under = ZeroTo(range);
            return (int) ((long)under + (long)inclusiveMinimum);
        }

        /// <summary>
        /// Generates a random <see cref="uint"/> between the given inclusive <paramref name="minimum"/> and inclusive <paramref name="maximum"/>s.
        /// </summary>
        public uint Between(uint inclusiveMinimum = uint.MinValue, uint inclusiveMaximum = uint.MaxValue)
        {
            if (inclusiveMinimum > inclusiveMaximum)
                throw new ArgumentOutOfRangeException(nameof(inclusiveMaximum), inclusiveMaximum, "Maximum must be greater than or equal to minimum");
            if (inclusiveMinimum == inclusiveMaximum) 
                return inclusiveMinimum;
            ulong range = ((ulong) inclusiveMaximum - (ulong) inclusiveMinimum) + 1UL;
            ulong under = ZeroTo(range);
            return (uint) (under + (ulong)inclusiveMinimum);
        }
        
        /// <summary>
        /// Generates a random <see cref="ulong"/> between the given inclusive <paramref name="minimum"/> and inclusive <paramref name="maximum"/>s.
        /// </summary>
        public long Between(long inclusiveMinimum, long inclusiveMaximum)
        {
            if (inclusiveMinimum > inclusiveMaximum)
                throw new ArgumentOutOfRangeException(nameof(inclusiveMaximum), inclusiveMaximum, "Maximum must be greater than or equal to minimum");
            if (inclusiveMinimum == inclusiveMaximum) 
                return inclusiveMinimum;
            if (inclusiveMinimum == long.MinValue &&
                inclusiveMaximum == long.MaxValue)
                return Long();
            ulong range = (ulong)(inclusiveMaximum - inclusiveMinimum) + 1UL;
            ulong under = ZeroTo(range);
            return ((long)under + inclusiveMinimum);
        }
        
        /// <summary>
        /// Generates a random <see cref="ulong"/> between the given inclusive <paramref name="minimum"/> and inclusive <paramref name="maximum"/>s.
        /// </summary>
        public ulong Between(ulong inclusiveMinimum, ulong inclusiveMaximum)
        {
            if (inclusiveMinimum > inclusiveMaximum)
                throw new ArgumentOutOfRangeException(nameof(inclusiveMaximum), inclusiveMaximum, "Maximum must be greater than or equal to minimum");
            if (inclusiveMinimum == inclusiveMaximum) 
                return inclusiveMinimum;
            if (inclusiveMinimum == ulong.MinValue &&
                inclusiveMaximum == ulong.MaxValue)
                return ULong();
            ulong range = (inclusiveMaximum - inclusiveMinimum) + 1UL;
            ulong under = ZeroTo(range);
            return under + inclusiveMinimum;
        }
        
         public TimeSpan Between(TimeSpan x, TimeSpan y)
        {
            long minTicks;
            long maxTicks;
            if (x <= y)
            {
                minTicks = x.Ticks;
                maxTicks = y.Ticks;
            }
            else
            {
                minTicks = y.Ticks;
                maxTicks = x.Ticks;
            }

            if (minTicks == long.MinValue && maxTicks == long.MaxValue)
            {
                return System.TimeSpan.FromTicks(Long());
            }
            else
            {
                ulong range = (ulong)(maxTicks - minTicks) + 1UL;
                ulong under = ZeroTo(range);
                var ticks = ((long)under + minTicks);
                return System.TimeSpan.FromTicks(ticks);
            }
        }

        public DateTime Between(DateTime x, DateTime y)
        {
            if (x == y)
                return x;

            long minTicks;
            long maxTicks;
            if (x <= y)
            {
                minTicks = x.Ticks;
                maxTicks = y.Ticks;
            }
            else
            {
                minTicks = y.Ticks;
                maxTicks = x.Ticks;
            }

            DateTimeKind kind;
            if (x.Kind == y.Kind)
            {
                kind = x.Kind;
            }
            else
            {
                kind = DateTimeKind.Unspecified;
            }
            
            if (minTicks == long.MinValue && maxTicks == long.MaxValue)
            {
                return new DateTime(Long(), kind);
            }
            else
            {
                ulong range = (ulong)(maxTicks - minTicks) + 1UL;
                ulong under = ZeroTo(range);
                var ticks = ((long)under + minTicks);
                return new DateTime(ticks, kind);
            }
        }
        public DateTimeOffset Between(DateTimeOffset x, DateTimeOffset y)
        {
            return new DateTimeOffset(Between(x.DateTime, y.DateTime),
                                      Between(x.Offset, y.Offset));
        }

        public TUnmanaged Unmanaged<TUnmanaged>() 
            where TUnmanaged : unmanaged
        {
            Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<TUnmanaged>()];
            this.Fill(bytes);
            return MemoryMarshal.Read<TUnmanaged>(bytes);
        }

        public void Fill(Span<byte> span)
        {
             // For improved performance the below loop operates on these stack allocated copies of the heap variables.
            // Note. doing this means that these heavily used variables are located near to other local/stack variables,
            // thus they will very likely be cached in the same CPU cache line.
            ulong state0 = _s0;
            ulong state1 = _s1;
            ulong state2 = _s2;
            ulong state3 = _s3;

            // Allocate bytes in groups of 8 (64 bits at a time), for good performance.
            // Keep looping and updating buffer to point to the remaining/unset bytes, until buffer.Length is too small
            // to use this loop.
            while (span.Length >= sizeof(ulong))
            {
                // Get 64 random bits, and assign to buffer (at the slice it is currently pointing to)
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span),
                                      BitOperations.RotateLeft(state1 * 5, 7) * 9);

                // Update PRNG state.
                ulong t = state1 << 17;
                state2 ^= state0;
                state3 ^= state1;
                state1 ^= state2;
                state0 ^= state3;
                state2 ^= t;
                state3 = BitOperations.RotateLeft(state3, 45);

                // Set buffer to the a slice over the remaining bytes.
                span = span.Slice(sizeof(ulong));
            }

            // Fill any remaining bytes in buffer (these occur when its length is not a multiple of eight).
            if (!span.IsEmpty)
            {
                // Get 64 random bits.
                ulong next = BitOperations.RotateLeft(state1 * 5, 7) * 9;
                unsafe
                {
                    byte* remainingBytes = (byte*)&next;

                    for (int i=0; i < span.Length; i++)
                    {
                        span[i] = remainingBytes[i];
                    }
                }

                // Update PRNG state.
                ulong t = state1 << 17;
                state2 ^= state0;
                state3 ^= state1;
                state1 ^= state2;
                state0 ^= state3;
                state2 ^= t;
                state3 = BitOperations.RotateLeft(state3, 45);
            }

            // Update the state variables on the heap.
            _s0 = state0;
            _s1 = state1;
            _s2 = state2;
            _s3 = state3;
        }

       

        /*
       

        public float Between(float inclusiveMinimum = float.MinValue, float inclusiveMaximum = float.MaxValue)
        {
            if (minimum > maximum)
                throw new ArgumentOutOfRangeException(nameof(maximum), maximum, "Maximum must be greater than or equal to minimum");
            if (EqualityComparer<float>.Default.Equals(minimum, maximum)) return minimum;
            double range = (double) inclusiveMaximum - (double) inclusiveMinimum + (double)float.Epsilon;
            return (float) ((PercentDouble() * range) + (double) minimum);
        }
        public double Between(double inclusiveMinimum = double.MinValue, double inclusiveMaximum = double.MaxValue)
        {
            if (minimum > maximum)
                throw new ArgumentOutOfRangeException(nameof(maximum), maximum, "Maximum must be greater than or equal to minimum");
            if (EqualityComparer<double>.Default.Equals(minimum, maximum)) return minimum;
            double range = inclusiveMaximum - inclusiveMinimum + double.Epsilon;
            return (PercentDouble() * range) + minimum;
        }

    
*/

         float IRandomizer.Float()
         {
             throw new NotImplementedException();
         }

         double IRandomizer.Double()
         {
             throw new NotImplementedException();
         }

         decimal IRandomizer.Decimal()
         {
             throw new NotImplementedException();
         }

       

      

         decimal IRandomizer.PercentDecimal()
         {
             throw new NotImplementedException();
         }

       

         float IRandomizer.Between(float inclusiveMinimum, float inclusiveMaximum)
         {
             throw new NotImplementedException();
         }

         double IRandomizer.Between(double inclusiveMinimum, double inclusiveMaximum)
         {
             throw new NotImplementedException();
         }

         decimal IRandomizer.Between(decimal inclusiveMinimum, decimal inclusiveMaximum)
         {
             throw new NotImplementedException();
         }

      

         int IRandomizer.ZeroTo(int exclusiveMaximum)
         {
             throw new NotImplementedException();
         }

         TEnum IRandomizer.Enum<TEnum>()
         {
             throw new NotImplementedException();
         }

        

         T IRandomizer.Single<T>(params T[] values)
         {
             throw new NotImplementedException();
         }

         T IRandomizer.Single<T>(IEnumerable<T> values)
         {
             throw new NotImplementedException();
         }

        

         IEnumerable<T> IRandomizer.Enumerate<T>(params T[] values)
         {
             throw new NotImplementedException();
         }

         IEnumerable<T> IRandomizer.Enumerate<T>(IEnumerable<T> values)
         {
             throw new NotImplementedException();
         }

         IReadOnlyList<T> IRandomizer.Shuffled<T>(params T[] values)
         {
             throw new NotImplementedException();
         }

         IReadOnlyList<T> IRandomizer.Shuffled<T>(IEnumerable<T> values)
         {
             throw new NotImplementedException();
         }

         void IRandomizer.Shuffle<T>(Span<T> values)
         {
             throw new NotImplementedException();
         }

         void IRandomizer.Shuffle<T>(IList<T> values)
         {
             throw new NotImplementedException();
         }
    }
}