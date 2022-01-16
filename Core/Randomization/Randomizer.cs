using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Jay.Reflection;

namespace Jay.Randomization;

public class Randomizer
{
    public static T Generate<T>()
        where T : unmanaged
    {
        Span<byte> bytes = stackalloc byte[Unmanaged.SizeOf<T>()];
        RandomNumberGenerator.Fill(bytes);
        return MemoryMarshal.Read<T>(bytes);
    }
}