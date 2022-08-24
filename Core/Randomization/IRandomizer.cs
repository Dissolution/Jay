using System.Runtime.InteropServices;
using Jay.Reflection;

namespace Jay.Randomization;

public interface IRandomizer
{
    /// <summary>
    /// Get an instance of this <see cref="IRandomizer"/>
    /// </summary>
    static abstract IRandomizer Instance { get; }
    
    /// <summary>
    /// Does this <see cref="IRandomizer"/> produce high-resolution (more-random) results?
    /// </summary>
    bool IsHighResolution { get; }

    
    /// <summary>
    /// Returns a random <see cref="byte"/> value [0..255]
    /// </summary>
    byte Byte();
    
    /// <summary>
    /// Returns a random <see cref="byte"/> in a range
    /// </summary>
    /// <param name="inclusiveMin">The inclusive minimum possible value.</param>
    /// <param name="inclusiveMax">The inclusive maximum possible value.</param>
    byte Between(byte inclusiveMin, byte inclusiveMax);

  
    /// <summary>
    /// Returns a random <see cref="sbyte"/> value [-128..127]
    /// </summary>
    sbyte SByte();
    /// <summary>
    /// Returns a random <see cref="sbyte"/> in a range
    /// </summary>
    /// <param name="inclusiveMin">The inclusive minimum possible value.</param>
    /// <param name="inclusiveMax">The inclusive maximum possible value.</param>
    sbyte Between(sbyte inclusiveMin, sbyte inclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="short"/> value [-32_768..32_767]
    /// </summary>
    short Short();
  
    short Between(short inclusiveMin, short inclusiveMax);

    
    /// <summary>
    /// Returns a random <see cref="ushort"/> value [0..65_535]
    /// </summary>
    ushort UShort();
    
    ushort Between(ushort inclusiveMin, ushort inclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="int"/> value [-2_147_483_648..2_147_483_647]
    /// </summary>
    int Int();
    
    int Between(int inclusiveMin, int inclusiveMax);
    int ZeroTo(int exclusiveMax);
  
    /// <summary>
    /// Returns a random <see cref="uint"/> value [0..4_294_967_295]
    /// </summary>
    uint UInt();
    
    uint Between(uint inclusiveMin, uint inclusiveMax);
    uint ZeroTo(uint exclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="long"/> value [-9_223_372_036_854_775_808..9_223_372_036_854_775_807]
    /// </summary>
    long Long();
    
    long Between(long inclusiveMin, long inclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="ulong"/> value [0..18_446_744_073_709_551_615]
    /// </summary>
    ulong ULong();
    
    ulong Between(ulong inclusiveMin, ulong inclusiveMax);
    ulong ZeroTo(ulong exclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="nint"/> value [<see cref="nint.MinValue"/>..<see cref="nint.MaxValue"/>]
    /// </summary>
    nint NInt();
    
    nint Between(nint inclusiveMin, nint inclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="nuint"/> value [0..<see cref="nuint.MaxValue"/>]
    /// </summary>
    nuint NUInt();
    nuint Between(nuint inclusiveMin, nuint inclusiveMax);
   
    /// <summary>
    /// Returns a random <see cref="float"/> value
    /// </summary>
    /// <remarks>
    /// <see cref="IsHighResolution"/>
    /// <para>
    /// <c>true"/>: Every possible <see cref="float"/> value can be generated with equal probability for each exact representation.
    /// </para>
    /// <para>
    /// <c>false"/>: Only certain <see cref="float"/> values can be generated
    /// </para>
    /// </remarks>
    float Float();
    
    float Between(float inclusiveMin, float inclusiveMax);
    float Between(float inclusiveMin, float inclusiveMax, int digits);

    /// <summary>
    /// Returns a <see cref="float"/> percentage value [0.0f..1.0f)
    /// </summary>
    float FloatPercent();
    
    /// <summary>
    /// Returns a random <see cref="double"/> value
    /// </summary>
    /// <remarks>
    /// <see cref="IsHighResolution"/>
    /// <para>
    /// <c>true"/>: Every possible <see cref="double"/> value can be generated with equal probability for each exact representation.
    /// </para>
    /// <para>
    /// <c>false"/>: Only certain <see cref="double"/> values can be generated
    /// </para>
    /// </remarks>
    double Double();
    
    double Between(double inclusiveMin, double inclusiveMax);
    double Between(double inclusiveMin, double inclusiveMax, int digits);

    /// <summary>
    /// Returns a <see cref="double"/> percentage value [0.0d..1.0d)
    /// </summary>
    double DoublePercent();
    
    /// <summary>
    /// Returns a random <see cref="decimal"/> value
    /// </summary>
    decimal Decimal();
    
    decimal Between(decimal inclusiveMin, decimal inclusiveMax);
 
    /// <summary>
    /// Returns a <see cref="decimal"/> percentage value [0.0m..1.0m)
    /// </summary>
    decimal DecimalPercent();
    
    /// <summary>
    /// Returns a random <see cref="bool"/> value (<c>true"/> or <c>false"/>)
    /// </summary>
    /// <returns></returns>
    bool Bool();

    /// <summary>
    /// Returns a random <see cref="Guid"/>
    /// </summary>
    /// <remarks>
    /// Use this instead of <see cref="M:System.Guid.NewGuid()"/> if you want consistent <see cref="Guid"/>s produced with the same seed.
    /// </remarks>
    Guid Guid()
    {
        Span<byte> bytes = stackalloc byte[16];
        Fill(bytes);
        return new Guid(bytes);
    }

    /// <summary>
    /// Returns a random <see cref="char"/> value
    /// </summary>
    char Char();
    
    char Between(char inclusiveMin, char inclusiveMax);

    /// <summary>
    /// Returns a single random <see cref="char"/> value chosen from the <paramref name="text"/>
    /// </summary>
    char Single(string text) => Single<char>((ReadOnlySpan<char>)text);

    /// <summary>
    /// Returns a random <see cref="TimeSpan"/> value
    /// </summary>
    TimeSpan TimeSpan() => new TimeSpan(ticks: (long)ULong());

    /// <summary>
    /// Returns a random <see cref="TimeSpan"/> between the inclusive bounds
    /// </summary>
    /// <param name="inclusiveMin">The minimum possible value, inclusive.</param>
    /// <param name="inclusiveMax">The maximum possible value, inclusive.</param>
    TimeSpan Between(TimeSpan inclusiveMin, TimeSpan inclusiveMax)
    {
        return new TimeSpan(ticks: Between(inclusiveMin.Ticks, inclusiveMax.Ticks));
    }

    /// <summary>
    /// Returns a random <see cref="DateTime"/> value
    /// </summary>
    /// <param name="kind">The <see cref="DateTimeKind"/> of the returned <see cref="DateTime"/>.</param>
    DateTime DateTime(DateTimeKind kind = DateTimeKind.Unspecified)
    {
        return new DateTime(ticks: (long)ULong(), kind: kind);
    }
    /// <summary>
    /// Returns a random <see cref="DateTime"/> between the inclusive bounds
    /// </summary>
    /// <param name="inclusiveMin">The minimum possible value, inclusive.</param>
    /// <param name="inclusiveMax">The maximum possible value, inclusive.</param>
    DateTime Between(DateTime inclusiveMin, DateTime inclusiveMax)
    {
        return new DateTime(
            ticks: Between(inclusiveMin.Ticks, inclusiveMax.Ticks),
            kind: inclusiveMin.Kind);
    }

    DateTimeOffset DateTimeOffset()
    {
        return new DateTimeOffset(ticks: Long(),
            offset: new TimeSpan(ticks: Long()));
    }

    DateTimeOffset Between(DateTimeOffset inclusiveMin, DateTimeOffset inclusiveMax)
    {
        return new DateTimeOffset(
            dateTime: Between(inclusiveMin.DateTime, inclusiveMax.DateTime),
            offset: Between(inclusiveMin.Offset, inclusiveMax.Offset));
    }

    TimeOnly TimeOnly()
    {
        return new TimeOnly(ticks: Long());
    }

    TimeOnly Between(TimeOnly inclusiveMin, TimeOnly inclusiveMax)
    {
        return new TimeOnly(ticks: Between(inclusiveMin.Ticks, inclusiveMax.Ticks));
    }

    DateOnly DateOnly()
    {
        int year = Between(1, 9999);
        int month = Between(1, 12);
        int day = Between(1, System.DateTime.DaysInMonth(year, month));
        return new DateOnly(year, month, day);
    }

    DateOnly Between(DateOnly inclusiveMin, DateOnly inclusiveMax)
    {
        return System.DateOnly.FromDayNumber(Between(inclusiveMin.DayNumber, inclusiveMax.DayNumber));
    }

    /// <summary>
    /// Returns a random <typeparamref name="TEnum"/> value
    /// </summary>
    /// <typeparam name="TEnum">The type of <c>enum"/> to return.</typeparam>
    /// <returns>
    /// <para>
    /// Non-Flags: Any of the defined values
    /// </para>
    /// <para>
    /// Flags: Any possible combination of flags
    /// </para>
    /// </returns>
    TEnum Enum<TEnum>()
        where TEnum : struct, Enum;

 
    /// <summary>
    /// Returns a random <see cref="Nullable{T}"/> value with a chance to be <c>null</c>
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <c>unmanaged</c> value that might be returned.</typeparam>
    /// <param name="nullChance">The percentage chance that <c>null</c> will be returned.</param>
    T? Nullable<T>(float nullChance)
        where T : unmanaged
    {
        if (FloatPercent() <= nullChance)
            return null;
        return Unmanaged<T>();
    }

    /// <summary>
    /// Returns a random <typeparamref name="T"/> value
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <c>unmanaged</c> value to return.</typeparam>
    T Unmanaged<T>()
        where T : unmanaged
    {
        Span<byte> buffer = stackalloc byte[Danger.SizeOf<T>()];
        Fill(buffer);
        return Danger.ReadUnaligned<T>(buffer);
    }

    /// <summary>
    /// Fills a <c>Span&lt;byte&gt;</c> with random <see cref="byte"/>s
    /// </summary>
    void Fill(Span<byte> bytes);

    void Fill<T>(Span<T> span)
        where T : unmanaged
    {
        Fill(MemoryMarshal.Cast<T, byte>(span));
    }

    /// <summary>
    /// Returns a single random <typeparamref name="T"/> value chosen from all <paramref name="values"/>
    /// </summary>
    T Single<T>(ReadOnlySpan<T> values);
    
    /// <summary>
    /// Returns a single random <typeparamref name="T"/> value chosen from all <paramref name="values"/>
    /// </summary>
    T Single<T>(params T[] values);
    
    /// <summary>
    /// Returns a single random <typeparamref name="T"/> value chosen from all <paramref name="values"/>
    /// </summary>
    T Single<T>(IEnumerable<T> values);


    
    void Mix<T>(Span<T> values);
    void Mix<T>(T[] values);
    void Mix<T>(IList<T> values);

    IEnumerable<T> Mixed<T>(params T[] values);
    IEnumerable<T> Mixed<T>(IEnumerable<T> values);

    IReadOnlyList<T> MixToList<T>(params T[] values);
    IReadOnlyList<T> MixToList<T>(IEnumerable<T> values);
}