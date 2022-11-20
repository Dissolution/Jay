namespace Jay.Randomization;

public interface IRandomizer
{
    /// <summary>
    /// Get an instance of this <see cref="IRandomizer"/>
    /// </summary>
    abstract static IRandomizer Instance { get; }
    
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

    /// <summary>
    /// Returns a random <see cref="short"/> in a range
    /// </summary>
    /// <param name="inclusiveMin">The inclusive minimum possible value.</param>
    /// <param name="inclusiveMax">The inclusive maximum possible value.</param>
    short Between(short inclusiveMin, short inclusiveMax);

    /// <summary>
    /// Returns a random <see cref="ushort"/> value [0..65_535]
    /// </summary>
    ushort UShort();

    /// <summary>
    /// Returns a random <see cref="ushort"/> in a range
    /// </summary>
    /// <param name="inclusiveMin">The inclusive minimum possible value.</param>
    /// <param name="inclusiveMax">The inclusive maximum possible value.</param>
    ushort Between(ushort inclusiveMin, ushort inclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="int"/> value [-2_147_483_648..2_147_483_647]
    /// </summary>
    int Int();

    /// <summary>
    /// Returns a random <see cref="int"/> in a range
    /// </summary>
    /// <param name="inclusiveMin">The inclusive minimum possible value.</param>
    /// <param name="inclusiveMax">The inclusive maximum possible value.</param>
    int Between(int inclusiveMin, int inclusiveMax);

    /// <summary>
    /// Returns a random <see cref="int"/> from 0 up to (but not including) <paramref name="exclusiveMax"/>
    /// </summary>
    /// <param name="exclusiveMax">The exclusive maximum possible value.</param>
    /// <returns>A positive random <see cref="int"/> value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="exclusiveMax"/> is less than 1</exception>
    int ZeroTo(int exclusiveMax);

    /// <summary>
    /// Returns a random positive <see cref="int"/> value [0..int.MaxValue]
    /// </summary>
    int IntPositive();

    /// <summary>
    /// Returns a random <see cref="uint"/> value [0..4_294_967_295]
    /// </summary>
    uint UInt();

    /// <summary>
    /// Returns a random <see cref="uint"/> in a range
    /// </summary>
    /// <param name="inclusiveMin">The inclusive minimum possible value.</param>
    /// <param name="inclusiveMax">The inclusive maximum possible value.</param>
    uint Between(uint inclusiveMin, uint inclusiveMax);

    /// <summary>
    /// Returns a random <see cref="uint"/> from 0 up to (but not including) <paramref name="exclusiveMax"/>
    /// </summary>
    /// <param name="exclusiveMax">The exclusive maximum possible value.</param>
    /// <returns>A random <see cref="uint"/> value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="exclusiveMax"/> is 0</exception>
    uint ZeroTo(uint exclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="long"/> value [-9_223_372_036_854_775_808..9_223_372_036_854_775_807]
    /// </summary>
    long Long();

    /// <summary>
    /// Returns a random <see cref="long"/> in a range
    /// </summary>
    /// <param name="inclusiveMin">The inclusive minimum possible value.</param>
    /// <param name="inclusiveMax">The inclusive maximum possible value.</param>
    long Between(long inclusiveMin, long inclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="ulong"/> value [0..18_446_744_073_709_551_615]
    /// </summary>
    ulong ULong();

    /// <summary>
    /// Returns a random <see cref="ulong"/> in a range
    /// </summary>
    /// <param name="inclusiveMin">The inclusive minimum possible value.</param>
    /// <param name="inclusiveMax">The inclusive maximum possible value.</param>
    ulong Between(ulong inclusiveMin, ulong inclusiveMax);

    /// <summary>
    /// Returns a random <see cref="ulong"/> from 0 up to (but not including) <paramref name="exclusiveMax"/>
    /// </summary>
    /// <param name="exclusiveMax">The exclusive maximum possible value.</param>
    /// <returns>A random <see cref="ulong"/> value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="exclusiveMax"/> is 0</exception>
    ulong ZeroTo(ulong exclusiveMax);
    
    /// <summary>
    /// Returns a random <see cref="float"/> value
    /// </summary>
    /// <remarks>
    /// <see cref="IsHighResolution"/>
    /// <para>
    /// <c>true</c>: Every possible <see cref="float"/> value can be generated with equal probability for each exact representation.
    /// </para>
    /// <para>
    /// <c>false</c>: Only certain <see cref="float"/> values can be generated
    /// </para>
    /// </remarks>
    float Float();
    
    float Between(float inclusiveMin, float inclusiveMax);

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
    /// <c>true</c>: Every possible <see cref="double"/> value can be generated with equal probability for each exact representation.
    /// </para>
    /// <para>
    /// <c>false</c>: Only certain <see cref="double"/> values can be generated
    /// </para>
    /// </remarks>
    double Double();
    
    double Between(double inclusiveMin, double inclusiveMax);

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
    Guid Guid();
    
    /// <summary>
    /// Returns a random <see cref="TimeSpan"/> value
    /// </summary>
    TimeSpan TimeSpan();

    /// <summary>
    /// Returns a random <see cref="TimeSpan"/> between the inclusive bounds
    /// </summary>
    /// <param name="inclusiveMin">The minimum possible value, inclusive.</param>
    /// <param name="inclusiveMax">The maximum possible value, inclusive.</param>
    TimeSpan Between(TimeSpan inclusiveMin, TimeSpan inclusiveMax);

    /// <summary>
    /// Returns a random <see cref="DateTime"/> value
    /// </summary>
    /// <param name="kind">The <see cref="DateTimeKind"/> of the returned <see cref="DateTime"/>.</param>
    DateTime DateTime(DateTimeKind kind = DateTimeKind.Unspecified);

    /// <summary>
    /// Returns a random <see cref="DateTime"/> between the inclusive bounds
    /// </summary>
    /// <param name="inclusiveMin">The minimum possible value, inclusive.</param>
    /// <param name="inclusiveMax">The maximum possible value, inclusive.</param>
    DateTime Between(DateTime inclusiveMin, DateTime inclusiveMax);

    DateTimeOffset DateTimeOffset();

    DateTimeOffset Between(DateTimeOffset inclusiveMin, DateTimeOffset inclusiveMax);

    TimeOnly TimeOnly();

    TimeOnly Between(TimeOnly inclusiveMin, TimeOnly inclusiveMax);

    DateOnly DateOnly();

    DateOnly Between(DateOnly inclusiveMin, DateOnly inclusiveMax);

    /// <summary>
    /// Returns a random <see cref="char"/> value
    /// </summary>
    char Char();

    char Between(char inclusiveMin, char inclusiveMax);

    /// <summary>
    /// Returns a single random <see cref="char"/> chosen from the <paramref name="text"/>
    /// </summary>
    char Single(string text);

    /// <summary>
    /// Returns a single random <see cref="char"/> chosen from the <paramref name="text"/>
    /// </summary>
    char Single(ReadOnlySpan<char> text);

    string HexString(int length);

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
        where T : unmanaged;

    /// <summary>
    /// Returns a random <typeparamref name="T"/> value
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <c>unmanaged</c> value to return.</typeparam>
    T Unmanaged<T>()
        where T : unmanaged;

    /// <summary>
    /// Fills a <c>Span&lt;byte&gt;</c> with random <see cref="byte"/>s
    /// </summary>
    void Fill(Span<byte> bytes);

    void Fill<T>(Span<T> span)
        where T : unmanaged;

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

    IReadOnlyList<T> MixToList<T>(params T[] values);
    IReadOnlyList<T> MixToList<T>(IList<T> values);
    IReadOnlyList<T> MixToList<T>(IEnumerable<T> values);
}