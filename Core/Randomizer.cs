using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Jay;

partial class Randomizer
{
    public static Randomizer Instance { get; } = new Randomizer();

    /// <summary>
    /// SplitMix64 Pseudo-Random Number Generator
    /// </summary>
    /// <param name="state">
    /// Starting RNG state. This can take any value, including zero, and will be updated to the next state.
    /// </param>
    /// <returns>
    /// A pseudo-random <see cref="ulong"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong SplitMixNext(ref ulong state)
    {
        ulong z = (state += 0x_9E37_79B9_7F4A_7C15UL);
        z = (z ^ (z >> 30)) * 0x_BF58_476D_1CE4_E5B9UL;
        z = (z ^ (z >> 27)) * 0x_94D0_49BB_1331_11EBUL;
        return z ^ (z >> 31);
    }

    public static ulong GetCryptoRandomSeed()
    {
        Span<byte> bytes = stackalloc byte[sizeof(ulong)];
        RandomNumberGenerator.Fill(bytes);
        return BitConverter.ToUInt64(bytes);
    }
}

/// <summary>
/// 
/// </summary>
/// <seealso cref="https://xoshiro.di.unimi.it/xoshiro256starstar.c"/>
public sealed partial class Randomizer
{
    private readonly ulong _seed;

    // State #0
    private ulong _s0;

    // State #1
    private ulong _s1;

    // State #2
    private ulong _s2;

    // State #3
    private ulong _s3;


    public bool IsHighResolution { get; }

    public Randomizer(bool highResolution = false)
        : this(GetCryptoRandomSeed(), highResolution)
    {
    }
    public Randomizer(ulong seed, bool highResolution = false)
    {
        _seed = seed;
        this.IsHighResolution = highResolution;

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


    public T Value<T>()
        where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        Span<byte> bytes = stackalloc byte[size];
        FillBytes(bytes);
        return MemoryMarshal.Read<T>(bytes);
    }


    public void FillBytes(Span<byte> buffer)
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
         while (buffer.Length >= sizeof(ulong))
         {
             // Get 64 random bits, and assign to buffer (at the slice it is currently pointing to)
             Unsafe.WriteUnaligned(
                 ref MemoryMarshal.AsRef<byte>(buffer),
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
             buffer = buffer.Slice(sizeof(ulong));
         }

         // Fill any remaining bytes in buffer (these occur when its length is not a multiple of eight).
         if (!buffer.IsEmpty)
         {
             // Get 64 random bits.
             ulong next = BitOperations.RotateLeft(state1 * 5, 7) * 9;
             unsafe
             {
                 byte* remainingBytes = (byte*)&next;

                 for (int i = 0; i < buffer.Length; i++)
                 {
                     buffer[i] = remainingBytes[i];
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