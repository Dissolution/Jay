using System.Numerics;
using Jay.Utilities;

namespace Jay.Randomization;

internal sealed class Xoshiro256StarStarRandomizer : Randomizer
{
    /// <summary>
    /// SplitMix64 Pseudo-Random Number Generator
    /// </summary>
    /// <param name="state">Starting RNG state. This can take any value, including zero, and will be updated to the next state.</param>
    /// <returns>A pseudo-random <see cref="ulong"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong SplitMixNext(ref ulong state)
    {
        ulong z = (state += 0x_9E37_79B9_7F4A_7C15UL);
        z = (z ^ (z >> 30)) * 0x_BF58_476D_1CE4_E5B9UL;
        z = (z ^ (z >> 27)) * 0x_94D0_49BB_1331_11EBUL;
        return z ^ (z >> 31);
    }

    //State #0
    private ulong _s0;

    //State #1
    private ulong _s1;

    //State #2
    private ulong _s2;

    //State #3
    private ulong _s3;

    /// <summary>
    /// Gets the initial starting seed.
    /// </summary>
    public ulong Seed { get; }

    public override bool IsHighResolution { get; }

    public Xoshiro256StarStarRandomizer(bool isHighResolution = false)
        : this(Generate<ulong>(), isHighResolution)
    {
    }

    public Xoshiro256StarStarRandomizer(ulong seed, bool isHighResolution = false)
    {
        this.Seed = seed;
        this.IsHighResolution = isHighResolution;

        /* Notes.
         * The first random sample will be very strongly correlated to the value we give to the 
         * state variables here; such a correlation is undesirable, therefore we significantly 
         * weaken it by hashing the seed's bits using the splitmix64 PRNG.
         *
         * It is required that at least one of the state variables be non-zero;
         * use of splitmix64 satisfies this requirement because it is an equi-distributed generator,
         * thus if it outputs a zero it will next produce a zero after a further 2^64 outputs.
         */
        _s0 = SplitMixNext(ref seed);
        _s1 = SplitMixNext(ref seed);
        _s2 = SplitMixNext(ref seed);
        _s3 = SplitMixNext(ref seed);
    }

    internal static bool ValidateConstraints<T>(T min, T max,
                                                Func<T> getFullRangeValue,
                                                out T? trivial,
                                                [CallerArgumentExpression(nameof(min))]
                                                string minName = "",
                                                [CallerArgumentExpression(nameof(max))]
                                                string maxName = "")
        where T : IMinMaxValue<T>, IComparable<T>, IEquatable<T>
    {
        int c = min.CompareTo(max);
        if (c > 0)
        {
            throw new ArgumentOutOfRangeException(
                maxName,
                max,
                $"{maxName} must be greater than or equal to {minName}");
        }

        if (c == 0)
        {
            trivial = min;
            return true;
        }

        if (min.Equals(T.MinValue) && max.Equals(T.MaxValue))
        {
            trivial = getFullRangeValue();
            return true;
        }

        trivial = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong GenerateUInt64()
    {
        // For improved performance the below loop operates on these stack allocated copies of the heap variables.
        // Note. doing this means that these heavily used variables are located near to other local/stack variables,
        // thus they will very likely be cached in the same CPU cache line.
        ulong s0 = _s0;
        ulong s1 = _s1;
        ulong s2 = _s2;
        ulong s3 = _s3;

        // Generate a further 64 random bits.
        ulong result = BitOperations.RotateLeft(s1 * 5, 7) * 9;

        // Update PRNG state.
        ulong t = s1 << 17;

        s2 ^= s0;
        s3 ^= s1;
        s1 ^= s2;
        s0 ^= s3;

        s2 ^= t;
        s3 = BitOperations.RotateLeft(s3, 45);

        // Update the state variables on the heap.
        _s0 = s0;
        _s1 = s1;
        _s2 = s2;
        _s3 = s3;

        return result;
    }


    public override byte Byte()
    {
        return (byte)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(byte))));
    }

    public override sbyte SByte()
    {
        return (sbyte)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(sbyte))));
    }

    public override short Short()
    {
        return (short)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(short))));
    }

    public override ushort UShort()
    {
        return (ushort)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(ushort))));
    }

    public override int Int()
    {
        return (int)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(int))));
    }


    public override int ZeroTo(int exclusiveMax)
    {
        // Handle special case of a single sample value.
        if (exclusiveMax <= 1)
        {
            return 0;
        }

        /* Generate a double in the interval [0,1) and multiply by exclusiveMaximum.
         * However the use of floating point arithmetic will introduce bias.
         * if (!IsHighResolution)
         * {
         *   return (int)(DoublePercent() * exclusiveMaximum);
         * }
         */

        // Here we sample an integer value within the interval [0, maxValue).
        // Rejection sampling is used in order to produce unbiased samples. 
        // The rejection sampling method used here operates as follows:
        //
        //  1) Calculate N such that  2^(N-1) < maxValue <= 2^N, i.e. N is the minimum number of bits required
        //     to represent maxValue states.
        //  2) Generate an N bit random sample.
        //  3) Reject samples that are >= maxValue, and goto (2) to resample.
        //
        // Repeat until a valid sample is generated.

        // Log2Ceiling(numberOfStates) gives the number of bits required to represent maxValue states.
        int bitCount = Maths.Log2Ceiling((uint)exclusiveMax);

        // Rejection sampling loop.
        // Note. The expected number of samples per generated value is approx. 1.3862,
        // i.e. the number of loops, on average, assuming a random and uniformly distributed maxValue.
        int x;
        do
        {
            x = (int)(GenerateUInt64() >> (64 - bitCount));
        } while (x >= exclusiveMax);

        return x;
    }


    public override uint ZeroTo(uint exclusiveMaximum)
    {
        // Handle special case of a single sample value.
        if (exclusiveMaximum <= 1U)
        {
            return 0U;
        }

        /* Generate a double in the interval [0,1) and multiply by exclusiveMaximum.
         * However the use of floating point arithmetic will introduce bias.
         * if (!IsHighResolution)
         * {
         *     return (uint)(DoublePercent() * exclusiveMaximum);
         * }
         */

        // Here we sample an integer value within the interval [0, maxValue).
        // Rejection sampling is used in order to produce unbiased samples. 
        // The rejection sampling method used here operates as follows:
        //
        //  1) Calculate N such that  2^(N-1) < maxValue <= 2^N, i.e. N is the minimum number of bits required
        //     to represent maxValue states.
        //  2) Generate an N bit random sample.
        //  3) Reject samples that are >= maxValue, and goto (2) to resample.
        //
        // Repeat until a valid sample is generated.

        // Log2Ceiling(numberOfStates) gives the number of bits required to represent maxValue states.
        int bitCount = Maths.Log2Ceiling(exclusiveMaximum);

        // Rejection sampling loop.
        // Note. The expected number of samples per generated value is approx. 1.3862,
        // i.e. the number of loops, on average, assuming a random and uniformly distributed maxValue.
        uint x;
        do
        {
            x = (uint)(GenerateUInt64() >> (64 - bitCount));
        } while (x >= exclusiveMaximum);

        return x;
    }


    public override uint UInt()
    {
        return (uint)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(uint))));
    }

    public override long Long()
    {
        return (long)GenerateUInt64();
    }

    public long ZeroTo(long exclusiveMaximum)
    {
        // Handle special case of a single sample value.
        if (exclusiveMaximum <= 1L)
        {
            return 0L;
        }

        /*// Generate a double in the interval [0,1) and multiply by exclusiveMaximum.
        // However the use of floating point arithmetic will introduce bias.
        if (!IsHighResolution)
        {
            return (long)(DoublePercent() * exclusiveMaximum);
        }*/

        // Here we sample an integer value within the interval [0, maxValue).
        // Rejection sampling is used in order to produce unbiased samples. 
        // The rejection sampling method used here operates as follows:
        //
        //  1) Calculate N such that  2^(N-1) < maxValue <= 2^N, i.e. N is the minimum number of bits required
        //     to represent maxValue states.
        //  2) Generate an N bit random sample.
        //  3) Reject samples that are >= maxValue, and goto (2) to resample.
        //
        // Repeat until a valid sample is generated.

        // Log2Ceiling(numberOfStates) gives the number of bits required to represent maxValue states.
        int bitCount = Maths.Log2Ceiling((ulong)exclusiveMaximum);

        // Rejection sampling loop.
        // Note. The expected number of samples per generated value is approx. 1.3862,
        // i.e. the number of loops, on average, assuming a random and uniformly distributed maxValue.
        long x;
        do
        {
            x = (long)(GenerateUInt64() >> (64 - bitCount));
        } while (x >= exclusiveMaximum);

        return x;
    }


    public override ulong ZeroTo(ulong exclusiveMaximum)
    {
        // Handle special case of a single sample value.
        if (exclusiveMaximum <= 1UL)
        {
            return 0UL;
        }

        /*// Generate a double in the interval [0,1) and multiply by exclusiveMaximum.
        // However the use of floating point arithmetic will introduce bias.
        if (!IsHighResolution)
        {
            return (ulong)(DoublePercent() * exclusiveMaximum);
        }*/

        // Here we sample an integer value within the interval [0, maxValue).
        // Rejection sampling is used in order to produce unbiased samples. 
        // The rejection sampling method used here operates as follows:
        //
        //  1) Calculate N such that  2^(N-1) < maxValue <= 2^N, i.e. N is the minimum number of bits required
        //     to represent maxValue states.
        //  2) Generate an N bit random sample.
        //  3) Reject samples that are >= maxValue, and goto (2) to resample.
        //
        // Repeat until a valid sample is generated.

        // Log2Ceiling(numberOfStates) gives the number of bits required to represent maxValue states.
        int bitCount = Maths.Log2Ceiling(exclusiveMaximum);

        // Rejection sampling loop.
        // Note. The expected number of samples per generated value is approx. 1.3862,
        // i.e. the number of loops, on average, assuming a random and uniformly distributed maxValue.
        ulong x;
        do
        {
            x = GenerateUInt64() >> (64 - bitCount);
        } while (x >= exclusiveMaximum);

        return x;
    }

    public override ulong ULong() => GenerateUInt64();

    public override float Float()
    {
        if (!IsHighResolution)
        {
            const double range = ((double)float.MaxValue - (double)float.MinValue) + (double)float.Epsilon;
            return (float)((double)float.MinValue + (range * DoublePercent()));
        }

        /* Best approach, no crazed values,
         * distributed with respect to the representable intervals on the
         * floating-point number line
         * (removed "uniform" as with respect to a continuous number line it is
         * decidedly non-uniform):
         * Warning: generates positive infinity as well! Choose exponent of 127 to be on the safe side.
        {
            double mantissa = (DoublePercent() * 2.0d) - 1.0d;
            // choose -149 instead of -126 to also generate subnormal floats (*)
            double exponent = 1 << Between(-126, 128));
            return (float)(mantissa * exponent);
        }
        */

        /* Another approach which will give you some crazed values
         * (uniform distribution of bit patterns), 
         * potentially useful for fuzzing:
        {
            Span<byte> buffer = stackalloc byte[sizeof(float)];
            Fill(buffer);
            return Danger.ReadUnaligned<float>(buffer);
        }
        */

        /* An improvement over the previous version is this one,
         * which does not create "crazed" values
         * (neither infinities nor NaN) and is still fast
         * (also distributed with respect to the representable intervals
         * on the floating-point number line):
         */
        {
            var sign = ZeroTo(2);
            var exponent = ZeroTo(0xFF); // do not generate 0xFF (infinities and NaN)
            var mantissa = ZeroTo(0x800001);
            var bits = (sign << 31) + (exponent << 23) + mantissa;
            return Danger.DirectCast<long, float>(bits);
        }
    }

    public override float FloatPercent()
    {
        if (!IsHighResolution)
        {
            // Note. Here we generate a random integer between 0 and 2^24-1 (i.e. 24 binary 1s) and multiply
            // by the fractional unit value 1.0 / 2^24, thus the result has a max value of
            // 1.0 - (1.0 / 2^24). Or 0.99999994 in decimal.
            return (GenerateUInt64() >> 40) * (1.0f / (1U << 24));
        }
        else
        {
            throw new NotImplementedException();
        }
    }


    public override double DoublePercent()
    {
        if (!IsHighResolution)
        {
            // Notes.
            // Here we generate a random integer in the interval [0, 2^53-1]  (i.e. the max value is 53 binary 1s),
            // and multiply by the fractional value 1.0 / 2^53, thus the result has a min value of 0.0 and a max value of
            // 1.0 - (1.0 / 2^53), or 0.99999999999999989 in decimal.
            //
            // I.e. we break the interval [0,1) into 2^53 uniformly distributed discrete values, and thus the interval between
            // two adjacent values is 1.0 / 2^53. This increment is chosen because it is the smallest value at which each
            // distinct value in the full range (from 0.0 to 1.0 exclusive) can be represented directly by a double precision
            // float, and thus no rounding occurs in the representation of these values, which in turn ensures no bias in the
            // random samples.
            return (GenerateUInt64() >> 11) * (1.0d / (1UL << 53));
        }
        else
        {
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
            // RandomizerMode.Speed, however the scheme used in this method is much slower, so is likely of interest
            // in specialist scenarios.
            int exponent = -64;
            ulong significand;
            int shift;

            // Read zeros into the exponent until we hit a one; the rest
            // will go into the significand.
            while ((significand = GenerateUInt64()) == 0UL)
            {
                exponent -= 64;

                // If the exponent falls below -1074 = emin + 1 - p,
                // the exponent of the smallest subnormal, we are
                // guaranteed the result will be rounded to zero.  This
                // case is so unlikely it will happen in realistic
                // terms only if random64 is broken.
                if (exponent < -1074)
                    return 0.0d;
            }

            // There is a 1 somewhere in significand, not necessarily in
            // the most significant position.  If there are leading zeros,
            // shift them into the exponent and refill the less-significant
            // bits of the significand.  Can't predict one way or another
            // whether there are leading zeros: there's a fifty-fifty
            // chance, if random64 is uniformly distributed.
            shift = BitOperations.LeadingZeroCount(significand);
            if (shift != 0)
            {
                exponent -= shift;
                significand <<= shift;
                significand |= (GenerateUInt64() >> (64 - shift));
            }

            // Set the sticky bit, since there is almost surely another 1
            // in the bit stream.  Otherwise, we might round what looks
            // like a tie to even when, almost surely, were we to look
            // further in the bit stream, there would be a 1 breaking the
            // tie.
            significand |= 1;

            // Finally, convert to double (rounding) and scale by
            // 2^exponent.
            //return (double)significand * Math.Pow(2, exponent);
            // 2 ^ x == 1 << x;
            return (double)(significand * (1UL << exponent));
        }
    }

    public override double Double()
    {
        if (!IsHighResolution)
        {
            Span<byte> bytes = stackalloc byte[sizeof(double)];
            Fill(bytes);
            return Danger.ReadUnaligned<double>(bytes);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns a decimal scale, from 0&lt;=x&lt;=28 that is distributed by percentage chance that the scale should be chosen.
    /// </summary>
    private byte GetDecimalScale()
    {
        for (byte scale = 0; scale <= 28; scale++)
        {
            if (DoublePercent() >= 0.1d)
            {
                return scale;
            }
        }

        return 0;
    }

    public override decimal Decimal()
    {
        if (!IsHighResolution)
        {
            // https://stackoverflow.com/a/609529/2871210
            // note: this is NOT UNIFORM

            /* If speed counts, this only has to generate two ulongs
            ulong r = ULong();
            int lo = (int)r;
            bool isNegative = ((r >> sizeof(int)) & 0b1000000000000000_0000000000000000) != 0;
            r = ULong();
            int mid = (int)r;
            int hi = (int)(r >> sizeof(int));
            return new decimal(lo, mid, hi,
                isNegative,
                scale: (byte)ZeroTo(29));
            */

            return new decimal(Int(), Int(), Int(), Bool(), (byte)ZeroTo(29));
        }

        // https://stackoverflow.com/a/610228/2871210
        byte scale = GetDecimalScale();
        int a = (int)(uint.MaxValue * DoublePercent());
        int b = (int)(uint.MaxValue * DoublePercent());
        int c = (int)(uint.MaxValue * DoublePercent());
        bool n = Bool();
        return new decimal(a, b, c, n, scale);
    }

    public override decimal DecimalPercent()
    {
        ulong r = ULong();
        return new decimal(lo: (int)r,
            mid: (int)(r >> sizeof(int)),
            hi: (int)ZeroTo(542101087),
            isNegative: false,
            scale: 28);
    }

    public override bool Bool()
    {
        // Use a high bit since the low bits are linear-feedback shift registers (LFSRs) with low degree.
        // This is slower than the approach of generating and caching 64 bits for future calls, but
        // (A) gives good quality randomness, and (B) is still very fast.
        return (GenerateUInt64() & 0b1000000000000000_0000000000000000_0000000000000000_0000000000000000) != 0;
    }

    public override char Char()
    {
        return (char)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(char))));
    }
    
    public override void Fill(Span<byte> span)
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
            Danger.WriteUnaligned(
                ref Danger.SpanToRef(span),
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

                for (int i = 0; i < span.Length; i++)
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


    public IEnumerable<byte> ProduceBytes(CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            var r = ULong();
            for (var i = 0; i < 8; i++, r >>= 8)
            {
                yield return (byte)r;
            }
        }
    }
}