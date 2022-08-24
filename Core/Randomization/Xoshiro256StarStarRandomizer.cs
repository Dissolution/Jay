using System;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.InteropServices;
using Jay.Enums;
using Jay.Reflection;
using Jay.Validation;
using nint = System.IntPtr;

namespace Jay.Randomization;

internal sealed class Xoshiro256StarStarRandomizer : IRandomizer
{
    // Multiplier to convert a ulong to a float
    private const float FloatEpsilon = 1.0f / (1U << 24);

    // Multiplier to convert a ulong to a double
    private const double DoubleEpsilon = 1.0d / (1UL << 53);

    public static IRandomizer Instance { get; } = new Xoshiro256StarStarRandomizer();

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

    public bool IsHighResolution { get; }

    /// <summary>
    /// Construct a new <see cref="Randomizer"/> with a random seed.
    /// </summary>
    /// <param name="mode">The optional <see cref="RandomizerMode"/>.</param>
    public Xoshiro256StarStarRandomizer(bool isHighResolution = false)
        : this(Randomizer.Generate<ulong>(), isHighResolution)
    {
    }

    /// <summary>
    /// Construct a new <see cref="Randomizer"/> with a given <paramref name="seed"/>.
    /// </summary>
    /// <param name="seed">The initial seed for randomization.</param>
    /// <param name="mode">The optional <see cref="RandomizerMode"/>.</param>
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
         *
         * Use the splitmix64 RNG to hash the seed.
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


    public byte Byte()
    {
        return (byte)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(byte))));
    }

    public byte Between(byte inclusiveMin, byte inclusiveMax)
    {
        if (ValidateConstraints(inclusiveMin, inclusiveMax, Byte, out var value))
            return value;
        int range = ((int)inclusiveMax - (int)inclusiveMin) + 1;
        return (byte)((int)inclusiveMin + ZeroTo(range));
    }

    public sbyte SByte()
    {
        return (sbyte)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(sbyte))));
    }

    public sbyte Between(sbyte inclusiveMin, sbyte inclusiveMax)
    {
        if (ValidateConstraints(inclusiveMin, inclusiveMax, SByte, out var value))
            return value;
        int range = ((int)inclusiveMax - (int)inclusiveMin) + 1;
        return (sbyte)((int)inclusiveMin + ZeroTo(range));
    }

    public short Short()
    {
        return (short)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(short))));
    }

    public short Between(short inclusiveMin, short inclusiveMax)
    {
        if (ValidateConstraints(inclusiveMin, inclusiveMax, Short, out var value))
            return value;
        int range = ((int)inclusiveMax - (int)inclusiveMin) + 1;
        return (short)((int)inclusiveMin + ZeroTo(range));
    }

    public ushort UShort()
    {
        return (ushort)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(ushort))));
    }

    public ushort Between(ushort inclusiveMin, ushort inclusiveMax)
    {
        if (ValidateConstraints(inclusiveMin, inclusiveMax, UShort, out var value))
            return value;
        int range = ((int)inclusiveMax - (int)inclusiveMin) + 1;
        return (ushort)((int)inclusiveMin + ZeroTo(range));
    }

    public int Int()
    {
        return (int)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(int))));
    }

    public int Between(int inclusiveMin, int inclusiveMax)
    {
        if (ValidateConstraints(inclusiveMin, inclusiveMax, Int, out var value))
            return value;
        int range = (inclusiveMax - inclusiveMin) + 1;
        return inclusiveMax + ZeroTo(range);
    }

    /// <summary>
    /// Returns a random positive <see cref="int"/> value [0..int.MaxValue]
    /// </summary>
    public int IntPositive()
    {
        // Get 31 bits to force positive
        return (int)(GenerateUInt64() >> 33);
    }

    public int ZeroTo(int exclusiveMax)
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


    public uint ZeroTo(uint exclusiveMaximum)
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


    public uint UInt()
    {
        return (uint)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(uint))));
    }

    public uint Between(uint inclusiveMin, uint inclusiveMax)
    {
        if (ValidateConstraints(inclusiveMin, inclusiveMax, UInt, out var value))
            return value;
        long range = ((long)inclusiveMax - (long)inclusiveMin) + 1L;
        return (uint)((long)inclusiveMax + ZeroTo(range));
    }

    public long Long()
    {
        return (long)GenerateUInt64();
    }

    public long Between(long inclusiveMin, long inclusiveMax)
    {
        if (ValidateConstraints(inclusiveMin, inclusiveMax, Long, out var value))
            return value;
        long range = (inclusiveMax - inclusiveMin) + 1L;
        return inclusiveMax + ZeroTo(range);
    }

    /// <summary>
    /// Returns a random positive <see cref="long"/> value [0..long.MaxValue]
    /// </summary>
    public long LongPositive()
    {
        // Get 63 bits to force positive
        return (int)(GenerateUInt64() >> 1);
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


    public ulong ZeroTo(ulong exclusiveMaximum)
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

    public ulong ULong() => GenerateUInt64();

    public ulong Between(ulong inclusiveMin, ulong inclusiveMax)
    {
        if (ValidateConstraints(inclusiveMin, inclusiveMax, ULong, out var value))
            return value;
        ulong range = (inclusiveMax - inclusiveMin) + 1UL;
        return inclusiveMin + ZeroTo(range);
    }


    public nint NInt()
    {
        return (nint)Long();
    }

    public nint Between(nint inclusiveMin, nint inclusiveMax)
    {
        if (ValidateConstraints<nint>(inclusiveMin, inclusiveMax, NInt, out var value))
            return value;
        long range = ((long)inclusiveMax - (long)inclusiveMin) + 1L;
        return (nint)((long)inclusiveMin + ZeroTo(range));
    }

    public nuint NUInt()
    {
        return (nuint)GenerateUInt64();
    }

    public nuint Between(nuint inclusiveMin, nuint inclusiveMax)
    {
        if (ValidateConstraints<nuint>(inclusiveMin, inclusiveMax, NUInt, out var value))
            return value;
        ulong range = ((ulong)inclusiveMax - (ulong)inclusiveMin) + 1UL;
        return (nuint)((ulong)inclusiveMin + ZeroTo(range));
    }

    public float Float()
    {
        if (!IsHighResolution)
        {
            return (float)(ulong.MaxValue * DoublePercent())
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
            double exponent = Math.Pow(2.0d, (double)Between(-126, 128));
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
            var mantissa = ZeroTo(0x800000);
            var bits = (sign << 31) + (exponent << 23) + mantissa;
            return Danger.DirectCast<int, float>(bits);
        }
    }


    public float Between(float inclusiveMin, float inclusiveMax)
    {
        double range = ((double)inclusiveMax - (double)inclusiveMin) + (double)float.Epsilon;
        return (float)((DoublePercent() * range) + (double)inclusiveMin);
    }

    public float Between(float inclusiveMin, float inclusiveMax, int digits)
    {
        var incMin = Math.Round(inclusiveMin, digits);
        var incMax = Math.Round(inclusiveMax, digits);
        double range = (incMax - incMin) + (double)float.Epsilon;
        return (float)Math.Round((DoublePercent() * range) + incMin, digits);
    }

    public float FloatPercent()
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


    public double DoublePercent()
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
            return (double)(significand * Math.Pow(2.0d, (double)exponent)); //1UL << exponent));
        }
    }

    public double Double()
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

    public double Between(double inclusiveMin, double inclusiveMax)
    {
        double range = (inclusiveMax - inclusiveMin) + double.Epsilon;
        return (DoublePercent() * range) + inclusiveMin;
    }

    public double Between(double inclusiveMin, double inclusiveMax, int digits)
    {
        var incMin = Math.Round(inclusiveMin, digits);
        var incMax = Math.Round(inclusiveMax, digits);
        double range = (incMax - incMin) + double.Epsilon;
        return Math.Round((DoublePercent() * range) + incMin, digits);
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

    public decimal Decimal()
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

    public decimal Between(decimal inclusiveMin, decimal inclusiveMax)
    {
        throw new NotImplementedException();
    }


    public decimal DecimalPercent()
    {
        ulong r = ULong();
        return new decimal(lo: (int)r,
            mid: (int)(r >> sizeof(int)),
            hi: ZeroTo(542101087),
            isNegative: false,
            scale: 28);
    }

    public bool Bool()
    {
        // Use a high bit since the low bits are linear-feedback shift registers (LFSRs) with low degree.
        // This is slower than the approach of generating and caching 64 bits for future calls, but
        // (A) gives good quality randomness, and (B) is still very fast.
        return (GenerateUInt64() & 0b1000000000000000_0000000000000000_0000000000000000_0000000000000000) != 0;
    }

    public char Char()
    {
        return (char)(GenerateUInt64() >> (8 * (sizeof(ulong) - sizeof(char))));
    }

    public char Between(char inclusiveMin, char inclusiveMax)
    {
        if (ValidateConstraints(inclusiveMin, inclusiveMax, Char, out var ch))
            return ch;
        int range = ((int)inclusiveMax - (int)inclusiveMin) + 1;
        return (char)((int)inclusiveMin + ZeroTo(range));
    }

    public TEnum Enum<TEnum>()
        where TEnum : struct, Enum
    {
        EnumTypeInfo<TEnum> info = EnumInfo.For<TEnum>();
        if (info.HasFlags)
        {
            // Have to randomize the bits up to flag.highest
            throw new NotImplementedException();
        }
        else
        {
            return Single<TEnum>(info.Members);
        }
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


    public T Single<T>(ReadOnlySpan<T> values)
    {
        return values[ZeroTo(values.Length)];
    }


    public T Single<T>(params T[] values)
    {
        return values[ZeroTo(values.Length)];
    }


    public T Single<T>(IEnumerable<T> values)
    {
        if (values is IList<T> list)
        {
            if (list.Count == 0)
                throw new ArgumentException("There must be at least one value to return", nameof(values));
            return list[ZeroTo(list.Count)];
        }

        if (values is ICollection<T> collection)
        {
            if (collection.Count == 0)
                throw new ArgumentException("There must be at least one value to return", nameof(values));
            var r = ZeroTo(collection.Count);
            using (var e = collection.GetEnumerator())
            {
                do
                {
                    e.MoveNext();
                    r--;
                } while (r >= 0);

                return e.Current;
            }
        }

        T? value = default;
        int count = 0;
        using (var e = values.GetEnumerator())
        {
            while (e.MoveNext())
            {
                count++;
                if (ZeroTo(count) == 0)
                {
                    value = e.Current;
                }
            }
        }

        if (count == 0)
        {
            throw new ArgumentException("There must be at least one value to return", nameof(values));
        }

        return value!;
    }

    public IEnumerable<byte> ProduceBytes(CancellationToken token = default)
    {
        ulong r;
        while (!token.IsCancellationRequested)
        {
            r = ULong();
            for (var i = 0; i < 8; i++, r >>= 8)
            {
                yield return (byte)r;
            }
        }
    }


    public IEnumerable<ulong> ProduceULongs()
    {
        while (true)
        {
            yield return ULong();
        }
    }


    /// <remarks>
    /// To initialize an array a of n elements to a randomly shuffled copy of source, both 0-based:
    /// for i from 0 to n − 1 do
    /// j ← random integer such that 0 ≤ j ≤ i
    /// if j ≠ i
    ///     a[i] ← a[j]
    /// a[j] ← source[i]
    ///
    /// </remarks>
    public IReadOnlyList<T> MixToList<T>(ReadOnlySpan<T> values)
    {
        var len = values.Length;
        var array = new T[len];
        int r;
        for (var i = 0; i < len; i++)
        {
            r = ZeroTo(i + 1);
            if (r != i)
            {
                array[i] = array[r];
            }

            array[r] = values[i];
        }

        return array;
    }


    public IReadOnlyList<T> MixToList<T>(params T[] values)
    {
        var len = values.Length;
        var array = new T[len];
        int r;
        for (var i = 0; i < len; i++)
        {
            r = ZeroTo(i + 1);
            if (r != i)
            {
                array[i] = array[r];
            }

            array[r] = values[i];
        }

        return array;
    }


    /// <remarks>
    /// To initialize an empty array a to a randomly shuffled copy of source whose length is not known:
    /// while source.moreDataAvailable
    ///     j ← random integer such that 0 ≤ j ≤ a.length
    ///     if j = a.length
    ///         a.append(source.next)
    ///     else
    ///         a.append(a[j])
    ///         a[j] ← source.next
    /// </remarks>
    public IReadOnlyList<T> MixToList<T>(IEnumerable<T> values)
    {
        var list = new List<T>();
        using (var e = values.GetEnumerator())
        {
            int r;
            while (e.MoveNext())
            {
                r = ZeroTo(list.Count);
                if (r == list.Count)
                {
                    list.Add(e.Current);
                }
                else
                {
                    list.Add(list[r]);
                    list[r] = e.Current;
                }
            }

            return list;
        }
    }


    /// <remarks>
    /// -- To shuffle an array a of n elements (indices 0..n-1):
    /// for i from n−1 downto 1 do
    /// j ← random integer such that 0 ≤ j ≤ i
    ///     exchange a[j] and a[i]
    /// </remarks>
    public void Mix<T>(Span<T> values)
    {
        T temp;
        int r;
        for (var i = values.Length - 1; i > 0; i--)
        {
            r = ZeroTo(i + 1);
            temp = values[i];
            values[i] = values[r];
            values[r] = temp;
        }
    }


    public void Mix<T>(IList<T> values)
    {
        T temp;
        int r;
        for (var i = values.Count - 1; i > 0; i--)
        {
            r = ZeroTo(i + 1);
            temp = values[i];
            values[i] = values[r];
            values[r] = temp;
        }
    }


    public void Mix<T>(T[] values)
    {
        T temp;
        int r;
        for (var i = values.Length - 1; i > 0; i--)
        {
            r = ZeroTo(i + 1);
            temp = values[i];
            values[i] = values[r];
            values[r] = temp;
        }
    }

    public IEnumerable<T> Mixed<T>(params T[] values)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> Mixed<T>(IEnumerable<T> values)
    {
        throw new NotImplementedException();
    }
}