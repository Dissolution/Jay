using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Randomization
{
    public sealed class Xoshiro256StarStarRandomizer : RandomizerBase
    {
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

        /// <summary>
        /// Construct a new <see cref="Randomizer"/> with a random seed.
        /// </summary>
        /// <param name="mode">The optional <see cref="RandomizerMode"/>.</param>
        public Xoshiro256StarStarRandomizer(RandomizerMode mode = RandomizerMode.Speed)
            : this(Randomizer.GetCryptoRandomSeed<ulong>(), mode)
        {
        }

        /// <summary>
        /// Construct a new <see cref="Randomizer"/> with a given <paramref name="seed"/>.
        /// </summary>
        /// <param name="seed">The initial seed for randomization.</param>
        /// <param name="mode">The optional <see cref="RandomizerMode"/>.</param>
        public Xoshiro256StarStarRandomizer(ulong seed, RandomizerMode mode = RandomizerMode.Speed)
            : base(mode)
        {
            this.Seed = seed;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override ulong GenerateULong()
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
                    byte* remainingBytes = (byte*) &next;

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
    }
}