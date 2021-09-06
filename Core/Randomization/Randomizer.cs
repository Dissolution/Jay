using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Jay.Randomization
{
    public static class Randomizer
    {
        public static IRandomizer Instance { get; } = new Xoshiro256StarStarRandomizer();

        public static IRandomizer New() => new Xoshiro256StarStarRandomizer();
        
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
    }
}