using System.Numerics;
using System.Runtime.InteropServices;
using Jay.Reflection;

namespace Jay.Randomization;

internal sealed class Xoshiro256StarStarRandomizer : IRandomizer
{
    private const float FloatEpsilon = 1.0f / (1U << 24);
    private const double DoubleEpsilon = 1.0d / (1UL << 53);

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

    /// <inheritdoc />
    public byte Byte()
    {
        return (byte)(ULong() >> (8 * (sizeof(ulong) - sizeof(byte))));
    }

    /// <inheritdoc />
    public sbyte SByte()
    {
        return (sbyte)(ULong() >> (8 * (sizeof(ulong) - sizeof(sbyte))));
    }

    /// <inheritdoc />
    public short Short()
    {
        return (short)(ULong() >> (8 * (sizeof(ulong) - sizeof(short))));
    }

    /// <inheritdoc />
    public ushort UShort()
    {
        return (ushort)(ULong() >> (8 * (sizeof(ulong) - sizeof(ushort))));
    }

    /// <inheritdoc />
    public int Int()
    {
        return (int)(ULong() >> (8 * (sizeof(ulong) - sizeof(int))));
    }

    /// <inheritdoc />
    public uint UInt()
    {
        return (uint)(ULong() >> (8 * (sizeof(ulong) - sizeof(uint))));
    }

    /// <inheritdoc />
    public long Long()
    {
        return (long)(ULong());
    }
    
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
    

    /// <inheritdoc />
    public float Float()
    {
        Span<byte> bytes = stackalloc byte[sizeof(float)];
        Fill(bytes);
        return Danger.ReadUnaligned<float>(in MemoryMarshal.GetReference(bytes));
    }
    /// <inheritdoc />
    public float Float(float inclusiveMinimum, float inclusiveMaximum)
    {
        double range = ((double)inclusiveMaximum - (double)inclusiveMinimum) + (double)float.Epsilon;
        return (float)((DoublePercent() * range) + (double)inclusiveMinimum);
    }
    /// <inheritdoc />
    public float Float(float inclusiveMinimum, float inclusiveMaximum, int precision)
    {
        var incMin = Math.Round(inclusiveMinimum, precision);
        var incMax = Math.Round(inclusiveMaximum, precision);
        double range = (incMax - incMin) + (double)float.Epsilon;
        return (float)Math.Round((DoublePercent() * range) + incMin, precision);
    }

    /// <inheritdoc />
    public double Double()
    {
        Span<byte> bytes = stackalloc byte[sizeof(double)];
        Fill(bytes);
        return Danger.ReadUnaligned<double>(in MemoryMarshal.GetReference(bytes));
    }
    /// <inheritdoc />
    public double Double(double inclusiveMinimum, double inclusiveMaximum)
    {
        double range = (inclusiveMaximum - inclusiveMinimum) + double.Epsilon;
        return (DoublePercent() * range) + inclusiveMinimum;
    }
    /// <inheritdoc />
    public double Double(double inclusiveMinimum, double inclusiveMaximum, int precision)
    {
        var incMin = Math.Round(inclusiveMinimum, precision);
        var incMax = Math.Round(inclusiveMaximum, precision);
        double range = (incMax - incMin) + double.Epsilon;
        return Math.Round((DoublePercent() * range) + incMin, precision);
    }
    
    /// <inheritdoc />
    public decimal Decimal()
    {
        if (!IsHighResolution)
        {
            ulong r = ULong();
            int lo = (int)r;
            bool isNegative = ((r >> sizeof(int)) & 0b1000000000000000_0000000000000000) != 0;
            r = ULong();
            int mid = (int)r;
            int hi = (int)(r >> sizeof(int));
            return new decimal(lo, mid, hi,
                               isNegative,
                               scale: (byte)ZeroTo(29));
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    /// <inheritdoc />
    public decimal Decimal(decimal inclusiveMinimum, decimal inclusiveMaximum)
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public decimal Decimal(decimal inclusiveMinimum, decimal inclusiveMaximum, int precision)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public TimeSpan TimeSpan()
    {
        return new TimeSpan(ticks: (long)ULong());
    }

    /// <inheritdoc />
    public DateTime DateTime(DateTimeKind kind = DateTimeKind.Unspecified)
    {
        return new DateTime(ticks: (long)ULong(), kind: kind);
    }

    /// <inheritdoc />
    public DateTimeOffset DateTimeOffset(TimeSpan? offset = null)
    {
        return new DateTimeOffset(ticks: (long)ULong(),
                                  offset: offset ?? new TimeSpan((long)ULong()));
    }

    /// <inheritdoc />
    public bool Bool()
    {
        // Use a high bit since the low bits are linear-feedback shift registers (LFSRs) with low degree.
        // This is slower than the approach of generating and caching 64 bits for future calls, but
        // (A) gives good quality randomness, and (B) is still very fast.
        return (ULong() & 0b1000000000000000_0000000000000000_0000000000000000_0000000000000000) != 0;
    }

    /// <inheritdoc />
    public char Char()
    {
        return (char)(ULong() >> (8 * (sizeof(ulong) - sizeof(char))));
    }

    /// <inheritdoc />
    public Guid Guid()
    {
        // Do not just use Guid.NewGuid(), that doesn't respect our seed
        Span<byte> bytes = stackalloc byte[16];
        Fill(bytes);
        return new Guid(bytes);
    }

    /// <inheritdoc />
    public TEnum Enum<TEnum>(bool includeFlags = false) 
        where TEnum : Enum
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public T Unmanaged<T>() where T : unmanaged
    {
        Span<byte> bytes = stackalloc byte[Danger.SizeOf<T>()];
        Fill(bytes);
        return Danger.ReadUnaligned<T>(in bytes.GetPinnableReference());
    }

    /// <inheritdoc />
    public float FloatPercent()
    {
        if (!IsHighResolution)
        {
            // Note. Here we generate a random integer between 0 and 2^24-1 (i.e. 24 binary 1s) and multiply
            // by the fractional unit value 1.0 / 2^24, thus the result has a max value of
            // 1.0 - (1.0 / 2^24). Or 0.99999994 in decimal.
            return (ULong() >> 40) * FloatEpsilon;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
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
            return (ULong() >> 11) * DoubleEpsilon;
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
            while ((significand = ULong()) == 0UL)
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
            //return (double)significand * Math.Pow(2, exponent);
            // 2 ^ x == 1 << x;
            return (double)(significand * (1UL << exponent));
        }
    }

    /// <inheritdoc />
    public decimal DecimalPercent()
    {
        ulong r = ULong();
        return new decimal(lo: (int)r,
                           mid: (int)(r >> sizeof(int)),
                           hi: ZeroTo(542101087),
                           isNegative: false,
                           scale: 28);
    }


    public int ZeroTo(int exclusiveMaximum)
    {
        // Handle special case of a single sample value.
        if (exclusiveMaximum <= 1)
        {
            return 0;
        }

        // Generate a double in the interval [0,1) and multiply by exclusiveMaximum.
        // However the use of floating point arithmetic will introduce bias.
        if (!IsHighResolution)
        {
            return (int)(DoublePercent() * exclusiveMaximum);
        }

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
        int bitCount = Maths.Log2Ceiling((uint)exclusiveMaximum);

        // Rejection sampling loop.
        // Note. The expected number of samples per generated value is approx. 1.3862,
        // i.e. the number of loops, on average, assuming a random and uniformly distributed maxValue.
        int x;
        do
        {
            x = (int)(ULong() >> (64 - bitCount));
        } while (x >= exclusiveMaximum);

        return x;
    }

    /// <inheritdoc />
    public uint ZeroTo(uint exclusiveMaximum)
    {
        // Handle special case of a single sample value.
        if (exclusiveMaximum <= 1U)
        {
            return 0U;
        }

        // Generate a double in the interval [0,1) and multiply by exclusiveMaximum.
        // However the use of floating point arithmetic will introduce bias.
        if (!IsHighResolution)
        {
            return (uint)(DoublePercent() * exclusiveMaximum);
        }

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
            x = (uint)(ULong() >> (64 - bitCount));
        } while (x >= exclusiveMaximum);

        return x;
    }

    public long ZeroTo(long exclusiveMaximum)
    {
        // Handle special case of a single sample value.
        if (exclusiveMaximum <= 1L)
        {
            return 0L;
        }

        // Generate a double in the interval [0,1) and multiply by exclusiveMaximum.
        // However the use of floating point arithmetic will introduce bias.
        if (!IsHighResolution)
        {
            return (long)(DoublePercent() * exclusiveMaximum);
        }

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
            x = (long)(ULong() >> (64 - bitCount));
        } while (x >= exclusiveMaximum);

        return x;
    }

    /// <inheritdoc />
    public ulong ZeroTo(ulong exclusiveMaximum)
    {
        // Handle special case of a single sample value.
        if (exclusiveMaximum <= 1UL)
        {
            return 0UL;
        }

        // Generate a double in the interval [0,1) and multiply by exclusiveMaximum.
        // However the use of floating point arithmetic will introduce bias.
        if (!IsHighResolution)
        {
            return (ulong)(DoublePercent() * exclusiveMaximum);
        }

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
            x = ULong() >> (64 - bitCount);
        } while (x >= exclusiveMaximum);

        return x;
    }

    /// <inheritdoc />
    public int PositiveInt()
    {
        // Generate 64 random bits and shift right to leave the most significant 31 bits.
        // Bit 32 is the sign bit so must be zero to avoid negative results.
        return (int)(ULong() >> 33);
    }

    /// <inheritdoc />
    public char Char(char inclusiveMinimum, char inclusiveMaximum)
    {
        uint range = (uint)inclusiveMaximum - (uint)inclusiveMinimum;
        uint r = ZeroTo(range + 1U);
        return (char)((uint)inclusiveMinimum + r);
    }

    /// <inheritdoc />
    public byte Byte(byte inclusiveMinimum, byte inclusiveMaximum)
    {
        uint range = (uint)inclusiveMaximum - (uint)inclusiveMinimum;
        uint r = ZeroTo(range + 1U);
        return (byte)((uint)inclusiveMinimum + r);
    }

    /// <inheritdoc />
    public sbyte SByte(sbyte inclusiveMinimum, sbyte inclusiveMaximum)
    {
        int range = (int)inclusiveMaximum - (int)inclusiveMinimum;
        int r = ZeroTo(range + 1);
        return (sbyte)((int)inclusiveMinimum + r);
    }

    /// <inheritdoc />
    public short Short(short inclusiveMinimum, short inclusiveMaximum)
    {
        int range = (int)inclusiveMaximum - (int)inclusiveMinimum;
        int r = ZeroTo(range + 1);
        return (short)((int)inclusiveMinimum + r);
    }

    /// <inheritdoc />
    public ushort UShort(ushort inclusiveMinimum, ushort inclusiveMaximum)
    {
        uint range = (uint)inclusiveMaximum - (uint)inclusiveMinimum;
        uint r = ZeroTo(range + 1U);
        return (ushort)((uint)inclusiveMinimum + r);
    }

    /// <inheritdoc />
    public int Int(int inclusiveMinimum, int inclusiveMaximum)
    {
        long range = (long)inclusiveMaximum - (long)inclusiveMinimum;
        long r = ZeroTo(range + 1L);
        return (int)((long)inclusiveMinimum + r);
    }

    /// <inheritdoc />
    public uint UInt(uint inclusiveMinimum, uint inclusiveMaximum)
    {
        ulong range = (ulong)inclusiveMaximum - (ulong)inclusiveMinimum;
        ulong r = ZeroTo(range + 1UL);
        return (uint)((ulong)inclusiveMinimum + r);
    }

    /// <inheritdoc />
    public long Long(long inclusiveMinimum, long inclusiveMaximum)
    {
        if (inclusiveMinimum == long.MinValue &&
            inclusiveMaximum == long.MaxValue)
        {
            return (long)ULong();
        }

        ulong range = (ulong)(inclusiveMaximum - inclusiveMinimum);
        ulong r = ZeroTo(range + 1UL);
        return (inclusiveMinimum + (long)r);
    }

    /// <inheritdoc />
    public ulong ULong(ulong inclusiveMinimum, ulong inclusiveMaximum)
    {
        if (inclusiveMinimum == ulong.MinValue &&
            inclusiveMaximum == ulong.MaxValue)
        {
            return ULong();
        }

        ulong range = (inclusiveMaximum - inclusiveMinimum);
        ulong r = ZeroTo(range + 1UL);
        return (inclusiveMinimum + r);
    }

    

  

    /// <inheritdoc />
    public TimeSpan TimeSpan(TimeSpan inclusiveMinimum, TimeSpan inclusiveMaximum)
    {
        return new TimeSpan(Long(inclusiveMinimum.Ticks, inclusiveMaximum.Ticks));
    }

    /// <inheritdoc />
    public DateTime DateTime(DateTime inclusiveMinimum, DateTime inclusiveMaximum)
    {
        return new DateTime(Long(inclusiveMinimum.Ticks, inclusiveMaximum.Ticks),
                            inclusiveMinimum.Kind == inclusiveMaximum.Kind ? inclusiveMinimum.Kind : DateTimeKind.Unspecified);
    }

    /// <inheritdoc />
    public DateTimeOffset DateTimeOffset(DateTimeOffset inclusiveMinimum, DateTimeOffset inclusiveMaximum)
    {
        return new DateTimeOffset(DateTime(inclusiveMinimum.DateTime, inclusiveMaximum.DateTime),
                                  TimeSpan(inclusiveMinimum.Offset, inclusiveMaximum.Offset));
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
            Danger.WriteUnaligned(ref MemoryMarshal.GetReference(span),
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
    
    /// <inheritdoc />
    public T Single<T>(ReadOnlySpan<T> values)
    {
        return values[ZeroTo(values.Length)];
    }

    /// <inheritdoc />
    public T Single<T>(params T[] values)
    {
        return values[ZeroTo(values.Length)];
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IEnumerable<T> Produce<T>() where T : unmanaged
    {
        while (true)
        {
            yield return Unmanaged<T>();
        }
    }

    /// <inheritdoc />
    public IEnumerable<byte> ProduceBytes()
    {
        ulong r;
        while (true)
        {
            r = ULong();
            for (var i = 0; i < 8; i++, r >>= 8)
            {
                yield return (byte)r;
            }
        }
    }

    /// <inheritdoc />
    public IEnumerable<ulong> ProduceULongs()
    {
        while (true)
        {
            yield return ULong();
        }
    }

    /// <inheritdoc />
    public IEnumerable<T> ToEnumerable<T>(params T[] values)
    {
        throw new NotImplementedException();
    }
    
    

    /// <inheritdoc />
    public IEnumerable<T> ToEnumerable<T>(IEnumerable<T> values)
    {
        throw new NotImplementedException();
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
    public IReadOnlyList<T> ToList<T>(ReadOnlySpan<T> values)
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

    /// <inheritdoc />
    public IReadOnlyList<T> ToList<T>(params T[] values)
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

    /// <inheritdoc />
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
    public IReadOnlyList<T> ToList<T>(IEnumerable<T> values)
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
    
    

    /// <inheritdoc />
    /// <remarks>
    /// -- To shuffle an array a of n elements (indices 0..n-1):
    /// for i from n−1 downto 1 do
    /// j ← random integer such that 0 ≤ j ≤ i
    ///     exchange a[j] and a[i]
    /// </remarks>
    public void Shuffle<T>(Span<T> values)
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

    /// <inheritdoc />
    public void Shuffle<T>(IList<T> values)
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

    /// <inheritdoc />
    public void Shuffle<T>(T[] values)
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
}