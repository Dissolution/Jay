using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Randomization
{
    /// <summary>
    /// A source for random values
    /// </summary>
    public interface IRandomizer
    {
        /// <summary>
        /// The mode this <see cref="IRandomizer"/> runs under
        /// </summary>
        RandomizerMode Mode { get; }
        
        byte Byte();
        sbyte SByte();
        short Short();
        ushort UShort();
        int Int();
        uint UInt();
        long Long();
        ulong ULong();
        float Float();
        double Double();
        decimal Decimal();
        decimal Decimal(int scale);
        
        TimeSpan TimeSpan();
        DateTime DateTime(DateTimeKind kind = DateTimeKind.Unspecified);
        DateTimeOffset DateTimeOffset(TimeSpan? offset = null);
        
        bool Boolean();
        char Character();
        Guid Guid();
        nint NInt();
        nuint NUInt();

        TEnum Enum<TEnum>() where TEnum : unmanaged, Enum;
        
        /// <summary>
        /// Generates a random <see langword="unmanaged"/> <typeparamref name="T"/> value
        /// </summary>
        /// <typeparam name="T">The type of unmanaged value to generate</typeparam>
        /// <returns>A random value</returns>
        T Unmanaged<T>() where T : unmanaged;
        
        /// <summary>
        /// Generates a single-precision floating point percentage value (0.0f &lt;= R &lt; 1.0f)
        /// </summary>
        /// <returns>A random <see cref="float"/> between 0 and 1</returns>
        float PercentFloat();
        /// <summary>
        /// Generates a double-precision floating point percentage value (0.0d &lt;= R &lt; 1.0d)
        /// </summary>
        /// <returns>A random <see cref="double"/> between 0 and 1</returns>
        double PercentDouble();
        /// <summary>
        /// Generates a decimal percentage value (0.0m &lt;= R &lt; 1.0m)
        /// </summary>
        /// <returns>A random <see cref="decimal"/> between 0 and 1</returns>
        decimal PercentDecimal();
        
        /// <summary>
        /// Generates a <see cref="uint"/> value from 0 up to (but not including) the <paramref name="exclusiveMaximum"/>
        /// </summary>
        /// <param name="exclusiveMaximum"></param>
        /// <returns></returns>
        uint ZeroTo(uint exclusiveMaximum);
        /// <summary>
        /// Generates a <see cref="int"/> value from 0 up to (but not including) the <paramref name="exclusiveMaximum"/>
        /// </summary>
        /// <param name="exclusiveMaximum"></param>
        /// <returns></returns>
        int ZeroTo(int exclusiveMaximum);
        /// <summary>
        /// Generates a <see cref="ulong"/> value from 0 up to (but not including) the <paramref name="exclusiveMaximum"/>
        /// </summary>
        /// <param name="exclusiveMaximum"></param>
        /// <returns></returns>
        ulong ZeroTo(ulong exclusiveMaximum);

        int PositiveInt();
        
        char Between(char inclusiveMinimum, char inclusiveMaximum);
        byte Between(byte inclusiveMinimum, byte inclusiveMaximum);
        sbyte Between(sbyte inclusiveMinimum, sbyte inclusiveMaximum);
        short Between(short inclusiveMinimum, short inclusiveMaximum);
        ushort Between(ushort inclusiveMinimum, ushort inclusiveMaximum);
        int Between(int inclusiveMinimum, int inclusiveMaximum);
        uint Between(uint inclusiveMinimum, uint inclusiveMaximum);
        long Between(long inclusiveMinimum, long inclusiveMaximum);
        ulong Between(ulong inclusiveMinimum, ulong inclusiveMaximum);
        float Between(float inclusiveMinimum, float inclusiveMaximum);
        double Between(double inclusiveMinimum, double inclusiveMaximum);
        decimal Between(decimal inclusiveMinimum, decimal inclusiveMaximum);
        TimeSpan Between(TimeSpan inclusiveMinimum, TimeSpan inclusiveMaximum);
        DateTime Between(DateTime inclusiveMinimum, DateTime inclusiveMaximum);
        DateTimeOffset Between(DateTimeOffset inclusiveMinimum, DateTimeOffset inclusiveMaximum);
        
        T Single<T>(params T[] values);
        T Single<T>(IEnumerable<T> values);
        T Single<T>(ReadOnlySpan<T> values);

        void Fill(Span<byte> bytes);

        /// <summary>
        /// Produce an infinite series of <typeparamref name="T"/> values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> Produce<T>() where T : unmanaged;
        /// <summary>
        /// Produce an infinite series of random <see cref="byte"/>s
        /// </summary>
        /// <returns></returns>
        IEnumerable<byte> ProduceBytes();
        /// <summary>
        /// Produce an infinite series of random <see cref="ulong"/>s
        /// </summary>
        /// <returns></returns>
        IEnumerable<ulong> ProduceULongs();

        IEnumerable<T> Enumerate<T>(params T[] values);
        IEnumerable<T> Enumerate<T>(IEnumerable<T> values);

        T[] Shuffled<T>(ReadOnlySpan<T> values);
        T[] Shuffled<T>(params T[] values);
        IReadOnlyList<T> Shuffled<T>(IEnumerable<T> values);

        void Shuffle<T>(Span<T> values);
        void Shuffle<T>(IList<T> values);
        void Shuffle<T>(T[] array);
    }
}