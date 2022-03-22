using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Jay.Reflection;

namespace Jay.Randomization;

public class Randomizer
{
    internal static string BitString
    {
        get
        {
            Span<byte> bytes = stackalloc byte[16];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToHexString(bytes);
        }
    }

    public static IRandomizer Instance { get; } = new Xoshiro256StarStarRandomizer();
    
    static Randomizer()
    {

    }

    public static T Generate<T>()
        where T : unmanaged
    {
        Span<byte> bytes = stackalloc byte[Unmanaged.SizeOf<T>()];
        RandomNumberGenerator.Fill(bytes);
        return MemoryMarshal.Read<T>(bytes);
    }
}