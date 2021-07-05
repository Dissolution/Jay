using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Jay.Randomization
{
    public abstract partial class Randomizer
    {
        public static IRandomizer Instance { get; } = new Xoshiro256StarStarRandomizer();
        
        /// <summary>
        /// Gets a crypto-random <see langword="unmanaged"/> <typeparamref name="T"/> seed.
        /// </summary>
        public static T GetCryptoRandomSeed<T>()
            where T : unmanaged
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                Span<byte> bytes = stackalloc byte[NotSafe.SizeOf<T>()];
                rng.GetBytes(bytes);
                return Unsafe.ReadUnaligned<T>(ref bytes.GetPinnableReference());
            }
        }

        protected readonly Random _random = new Random();
        
        protected virtual ulong GenerateULong()
        {
            Span<byte> bytes = stackalloc byte[sizeof(ulong)];
            _random.NextBytes(bytes);
            return MemoryMarshal.Read<ulong>(bytes);
        }

        public string BitString()
        {
            ulong r = GenerateULong();
            return r.ToString("X");
        }
    }

    // public abstract partial class Randomizer : IRandomizer
    // {
    //    
    //     
    // }
}