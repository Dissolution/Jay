using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Randomization
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// In general:
    /// - Uppermost bits are preferred
    ///
    /// </remarks>
    /// <see cref="https://github.com/colgreen/Redzen/blob/master/Redzen/Random/RandomSourceBase.cs"/>
    public abstract class RandomizerBase : IRandomizer
    {
        private const float FloatEpsilon = 1.0f / (1U << 24);
        private const double DoubleEpsilon = 1.0d / (1UL << 53);

        /// <inheritdoc />
        public RandomizerMode Mode { get; }

        protected RandomizerBase(RandomizerMode randomizerMode)
        {
            this.Mode = randomizerMode;
        }

        protected abstract ulong GenerateULong();
        
        /// <inheritdoc />
        public byte Byte()
        {
            return (byte) (GenerateULong() >> (8 * (sizeof(ulong) - sizeof(byte))));
        }

        /// <inheritdoc />
        public sbyte SByte()
        {
            return (sbyte) (GenerateULong() >> (8 * (sizeof(ulong) - sizeof(sbyte))));
        }

        /// <inheritdoc />
        public short Short()
        {
            return (short) (GenerateULong() >> (8 * (sizeof(ulong) - sizeof(short))));
        }

        /// <inheritdoc />
        public ushort UShort()
        {
            return (ushort) (GenerateULong() >> (8 * (sizeof(ulong) - sizeof(ushort))));
        }

        /// <inheritdoc />
        public int Int()
        {
            return (int) (GenerateULong() >> (8 * (sizeof(ulong) - sizeof(int))));
        }

        /// <inheritdoc />
        public uint UInt()
        {
            return (uint) (GenerateULong() >> (8 * (sizeof(ulong) - sizeof(uint))));
        }

        /// <inheritdoc />
        public long Long()
        {
            return (long) (GenerateULong());
        }

        /// <inheritdoc />
        public ulong ULong()
        {
            return GenerateULong();
        }

        /// <inheritdoc />
        public float Float()
        {
            Span<byte> bytes = stackalloc byte[sizeof(float)];
            Fill(bytes);
            return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference(bytes));
        }

        /// <inheritdoc />
        public double Double()
        {
            Span<byte> bytes = stackalloc byte[sizeof(double)];
            Fill(bytes);
            return Unsafe.ReadUnaligned<double>(ref MemoryMarshal.GetReference(bytes));
        }

        /// <inheritdoc />
        public decimal Decimal()
        {
            if (this.Mode == RandomizerMode.Speed)
            {
                ulong r = GenerateULong();
                int lo = (int) r;
                bool isNegative = ((r >> sizeof(int)) & 0b1000000000000000_0000000000000000) != 0;
                r = GenerateULong();
                int mid = (int) r;
                int hi = (int) (r >> sizeof(int));
                return new decimal(lo, mid, hi, 
                                   isNegative, 
                                   scale: (byte) ZeroTo(29));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public TimeSpan TimeSpan()
        {
            return new TimeSpan((long) GenerateULong());
        }

        /// <inheritdoc />
        public DateTime DateTime(DateTimeKind kind = DateTimeKind.Unspecified)
        {
            return new DateTime((long) GenerateULong(), kind);
        }

        /// <inheritdoc />
        public DateTimeOffset DateTimeOffset(TimeSpan? offset = null)
        {
            return new DateTimeOffset((long) GenerateULong(), offset ?? new TimeSpan((long) GenerateULong()));
        }

        /// <inheritdoc />
        public bool Boolean()
        {
            // Use a high bit since the low bits are linear-feedback shift registers (LFSRs) with low degree.
            // This is slower than the approach of generating and caching 64 bits for future calls, but
            // (A) gives good quality randomness, and (B) is still very fast.
            return (GenerateULong() & 0b1000000000000000_0000000000000000_0000000000000000_0000000000000000) != 0;
        }

        /// <inheritdoc />
        public char Character()
        {
            return (char) (GenerateULong() >> (8 * (sizeof(ulong) - sizeof(char))));
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
        public TEnum Enum<TEnum>() where TEnum : unmanaged, Enum
        {
            var members = EnumInfo<TEnum>.Members;
            return members[ZeroTo(members.Count)];
        }

        /// <inheritdoc />
        public T Unmanaged<T>() where T : unmanaged
        {
            Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<T>()];
            Fill(bytes);
            return Unsafe.ReadUnaligned<T>(ref bytes.GetPinnableReference());
        }

        /// <inheritdoc />
        public float PercentFloat()
        {
            if (this.Mode == RandomizerMode.Speed)
            {
                // Note. Here we generate a random integer between 0 and 2^24-1 (i.e. 24 binary 1s) and multiply
                // by the fractional unit value 1.0 / 2^24, thus the result has a max value of
                // 1.0 - (1.0 / 2^24). Or 0.99999994 in decimal.
                return (GenerateULong() >> 40) * FloatEpsilon;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public double PercentDouble()
        {
            if (this.Mode == RandomizerMode.Speed)
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
                return (GenerateULong() >> 11) * DoubleEpsilon;
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
                while ((significand = GenerateULong()) == 0UL)
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
                    significand |= (GenerateULong() >> (64 - shift));
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
                return (double) (significand * (1UL << exponent));
            }
        }

        /// <inheritdoc />
        public decimal PercentDecimal()
        {
            ulong r = GenerateULong();
            return new decimal(lo: (int) r,
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
            if (Mode == RandomizerMode.Speed)
            {
                return (int) (PercentDouble() * exclusiveMaximum);
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
                x = (int)(GenerateULong() >> (64 - bitCount));
            }
            while (x >= exclusiveMaximum);
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
            if (Mode == RandomizerMode.Speed)
            {
                return (uint) (PercentDouble() * exclusiveMaximum);
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
                x = (uint)(GenerateULong() >> (64 - bitCount));
            }
            while (x >= exclusiveMaximum);
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
            if (Mode == RandomizerMode.Speed)
            {
                return (long) (PercentDouble() * exclusiveMaximum);
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
                x = (long)(GenerateULong() >> (64 - bitCount));
            }
            while (x >= exclusiveMaximum);
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
            if (Mode == RandomizerMode.Speed)
            {
                return (ulong) (PercentDouble() * exclusiveMaximum);
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
                x = GenerateULong() >> (64 - bitCount);
            }
            while (x >= exclusiveMaximum);
            return x;
        }

        /// <inheritdoc />
        public int PositiveInt()
        {
            // Generate 64 random bits and shift right to leave the most significant 31 bits.
            // Bit 32 is the sign bit so must be zero to avoid negative results.
            return (int)(GenerateULong() >> 33);
        }

        /// <inheritdoc />
        public char Between(char inclusiveMinimum, char inclusiveMaximum)
        {
            uint range = (uint) inclusiveMaximum - (uint) inclusiveMinimum;
            uint r = ZeroTo(range + 1U);
            return (char) ((uint)inclusiveMinimum + r);
        }

        /// <inheritdoc />
        public byte Between(byte inclusiveMinimum, byte inclusiveMaximum)
        {
            uint range = (uint) inclusiveMaximum - (uint) inclusiveMinimum;
            uint r = ZeroTo(range + 1U);
            return (byte) ((uint)inclusiveMinimum + r);
        }

        /// <inheritdoc />
        public sbyte Between(sbyte inclusiveMinimum, sbyte inclusiveMaximum)
        {
            int range = (int) inclusiveMaximum - (int) inclusiveMinimum;
            int r = ZeroTo(range + 1);
            return (sbyte) ((int) inclusiveMinimum + r);
        }

        /// <inheritdoc />
        public short Between(short inclusiveMinimum, short inclusiveMaximum)
        {
            int range = (int) inclusiveMaximum - (int) inclusiveMinimum;
            int r = ZeroTo(range + 1);
            return (short) ((int) inclusiveMinimum + r);
        }

        /// <inheritdoc />
        public ushort Between(ushort inclusiveMinimum, ushort inclusiveMaximum)
        {
            uint range = (uint) inclusiveMaximum - (uint) inclusiveMinimum;
            uint r = ZeroTo(range + 1U);
            return (ushort) ((uint)inclusiveMinimum + r);
        }

        /// <inheritdoc />
        public int Between(int inclusiveMinimum, int inclusiveMaximum)
        {
            long range = (long) inclusiveMaximum - (long) inclusiveMinimum;
            long r = ZeroTo(range + 1L);
            return (int)((long) inclusiveMinimum + r);
        }

        /// <inheritdoc />
        public uint Between(uint inclusiveMinimum, uint inclusiveMaximum)
        {
            ulong range = (ulong) inclusiveMaximum - (ulong) inclusiveMinimum;
            ulong r = ZeroTo(range + 1UL);
            return (uint) ((ulong)inclusiveMinimum + r);
        }

        /// <inheritdoc />
        public long Between(long inclusiveMinimum, long inclusiveMaximum)
        {
            if (inclusiveMinimum == long.MinValue &&
                inclusiveMaximum == long.MaxValue)
            {
                return (long) GenerateULong();
            }

            ulong range = (ulong) (inclusiveMaximum - inclusiveMinimum);
            ulong r = ZeroTo(range + 1UL);
            return (inclusiveMinimum + (long) r);
        }

        /// <inheritdoc />
        public ulong Between(ulong inclusiveMinimum, ulong inclusiveMaximum)
        {
            if (inclusiveMinimum == ulong.MinValue &&
                inclusiveMaximum == ulong.MaxValue)
            {
                return GenerateULong();
            }

            ulong range = (inclusiveMaximum - inclusiveMinimum);
            ulong r = ZeroTo(range + 1UL);
            return (inclusiveMinimum + r);
        }

        /// <inheritdoc />
        public float Between(float inclusiveMinimum, float inclusiveMaximum)
        {
            double range = ((double) inclusiveMaximum - (double) inclusiveMinimum) + (double)float.Epsilon;
            return (float) ((PercentDouble() * range) + (double) inclusiveMinimum);
        }

        /// <inheritdoc />
        public double Between(double inclusiveMinimum, double inclusiveMaximum)
        {
            double range = (inclusiveMaximum - inclusiveMinimum) + double.Epsilon;
            return (PercentDouble() * range) + inclusiveMinimum;
        }

        /// <inheritdoc />
        public decimal Between(decimal inclusiveMinimum, decimal inclusiveMaximum)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TimeSpan Between(TimeSpan inclusiveMinimum, TimeSpan inclusiveMaximum)
        {
            return new TimeSpan(Between(inclusiveMinimum.Ticks, inclusiveMaximum.Ticks));
        }

        /// <inheritdoc />
        public DateTime Between(DateTime inclusiveMinimum, DateTime inclusiveMaximum)
        {
            return new DateTime(Between(inclusiveMinimum.Ticks, inclusiveMaximum.Ticks), 
                                inclusiveMinimum.Kind == inclusiveMaximum.Kind ? inclusiveMinimum.Kind : DateTimeKind.Unspecified);
        }

        /// <inheritdoc />
        public DateTimeOffset Between(DateTimeOffset inclusiveMinimum, DateTimeOffset inclusiveMaximum)
        {
            return new DateTimeOffset(Between(inclusiveMinimum.DateTime, inclusiveMaximum.DateTime),
                                      Between(inclusiveMinimum.Offset, inclusiveMaximum.Offset));
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
        public abstract void Fill(Span<byte> bytes);

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
                r = GenerateULong();
                for (var i = 0; i < 8; i++, r >>= 8)
                {
                    yield return (byte) r;
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<ulong> ProduceULongs()
        {
            while (true)
            {
                yield return GenerateULong();
            }
        }

        /// <inheritdoc />
        public IEnumerable<T> Enumerate<T>(params T[] values)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerable<T> Enumerate<T>(IEnumerable<T> values)
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
        public T[] Shuffled<T>(ReadOnlySpan<T> values)
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
        public T[] Shuffled<T>(params T[] values)
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
        public IReadOnlyList<T> Shuffled<T>(IEnumerable<T> values)
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
}