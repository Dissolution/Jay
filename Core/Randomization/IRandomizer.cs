using System.Runtime.InteropServices;
using Jay.Reflection;

namespace Jay.Randomization;

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
    /// <summary>
    /// Get a random <see cref="float"/> percentage value [0.0f &lt;= (r)f &lt; 1.0f]
    /// </summary>
    float FloatPercent();

    double Double();
    double Double(double incMin, double incMax, double precision);
    /// <summary>
    /// Get a random <see cref="double"/> percentage value [0.00d &lt;= (r)d &lt; 1.00d]
    /// </summary>
    double DoublePercent();

    decimal Decimal();
    decimal Decimal(decimal incMin, decimal incMax, decimal precision);
    /// <summary>
    /// Get a random <see cref="decimal"/> percentage value [0.0mf &lt;= (r)m &lt; 1.0m]
    /// </summary>
    decimal DecimalPercent();


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

    DateTime DateTime(DateTimeKind kind = DateTimeKind.Unspecified)
    {
        return new DateTime(Long(), kind);
    }
    DateTime DateTime(DateTime incMin, DateTime incMax);

    DateTimeOffset DateTimeOffset(TimeSpan? offset)
    {
        return new DateTimeOffset(Long(), offset ?? TimeSpan());
    }
    DateTimeOffset DateTimeOffset(DateTimeOffset incMin, DateTimeOffset incMax);

    void Fill(Span<byte> bytes);

    TEnum Enum<TEnum>(bool incFlags = false)
        where TEnum : Enum;

    TUnmanaged Unmanaged<TUnmanaged>()
        where TUnmanaged : unmanaged
    {
        Span<byte> bytes = stackalloc byte[Danger.SizeOf<TUnmanaged>()];
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

    IEnumerable<T> ToEnumerable<T>(params T[] values);
    IEnumerable<T> ToEnumerable<T>(IEnumerable<T> values);

    IReadOnlyList<T> ToList<T>(params T[] values);
    IReadOnlyList<T> ToList<T>(IEnumerable<T> values);
}