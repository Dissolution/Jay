using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Jay.Reflection;

namespace Jay.Randomization;

public class Randomizer
{
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

public interface IRandomizer
{
    bool IsHighResolution { get; }

    byte Byte();
    byte Byte(byte incMin, byte incMax);
    sbyte SByte();
    sbyte SByte(sbyte incMin, sbyte incMax);
    short Short();
    short Short(short incMin, short incMax);
    ushort UShort();
    ushort UShort(ushort incMin, ushort incMax);
    int Int();
    int Int(int incMin, int incMax);
    uint UInt();
    uint UInt(uint incMin, uint incMax);
    long Long();
    long Long(long incMin, long incMax);
    ulong ULong();
    ulong ULong(ulong incMin, ulong incMax);

    float Float();
    float Float(float incMin, float incMax, float precision);
    double Double();
    double Double(double incMin, double incMax, double precision);
    decimal Decimal();
    decimal Decimal(decimal incMin, decimal incMax, decimal precision);

    int ZeroTo(int incMax);
    ulong ZeroTo(ulong incMax);

    bool Bool();

    /// <summary>
    /// Generates a random <see cref="Guid"/>
    /// </summary>
    /// <returns>A random <see cref="Guid"/> based upon this <see cref="IRandomizer"/>'s SEED.</returns>
    /// <remarks>
    /// One would use this (rather than <see cref="M:System.Guid.NewGuid()"/>) if you wanted consistent Guids produced with the same SEED.
    /// </remarks>
    Guid Guid()
    {
        Span<byte> bytes = stackalloc byte[16];
        Fill(bytes);
        return new Guid(bytes);
    }

    char Char() => (char)UShort();
    char Char(char incMin, char incMax) => (char)UShort((ushort)incMin, (ushort)incMax);

    TimeSpan TimeSpan()
    {
        return System.TimeSpan.FromTicks(Long());
    }
    TimeSpan TimeSpan(TimeSpan incMin, TimeSpan incMax);

    DateTime DateTime()
    {
        return new DateTime(Long());
    }
    DateTime DateTime(DateTime incMin, DateTime incMax);

    DateTimeOffset DateTimeOffset()
    {
        return new DateTimeOffset(Long(), TimeSpan());
    }
    DateTimeOffset DateTimeOffset(DateTimeOffset incMin, DateTimeOffset incMax);

    void Fill(Span<byte> bytes);

    TEnum Enum<TEnum>(bool incFlags = false)
        where TEnum : struct, Enum;

    TUnmanaged Unmanaged<TUnmanaged>()
        where TUnmanaged : unmanaged
    {
        Span<byte> bytes = stackalloc byte[Reflection.Unmanaged.SizeOf<TUnmanaged>()];
        Fill(bytes);
        return MemoryMarshal.Read<TUnmanaged>(bytes);
    }

    T Single<T>(ReadOnlySpan<T> values)
    {
        int r = ZeroTo(values.Length - 1);
        return values[r];
    }
    T Single<T>(params T[] values)
    {
        int r = ZeroTo(values.Length - 1);
        return values[r];
    }
    T Single<T>(IEnumerable<T> values);

    void Shuffle<T>(Span<T> values);
    void Shuffle<T>(T[] values);
    void Shuffle<T>(IList<T> values);
    IReadOnlyList<T> Shuffled<T>(IEnumerable<T> values);
}