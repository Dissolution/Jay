using System;
using System.Collections.Generic;

namespace Jay.Randomization
{
    public enum RandomizerMode
    {
        Fast = 0,
        Precise = 1,
    }
    
    public interface IRandomizer
    {
        RandomizerMode Mode { get; }
        
        bool Bool();
        char Char();
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
        TimeSpan TimeSpan();
        DateTime DateTime(DateTimeKind kind = DateTimeKind.Local);
        DateTimeOffset DateTimeOffset();
        Guid Guid();

        string GuidString() => Guid().ToString("N").ToUpper();
                
        float PercentFloat();
        double PercentDouble();
        decimal PercentDecimal();
        
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
        
        int ZeroTo(int exclusiveMaximum);
        ulong ZeroTo(ulong exclusiveMaximum);

        TEnum Enum<TEnum>() where TEnum : struct, Enum;
        TUnmanaged Unmanaged<TUnmanaged>() where TUnmanaged : unmanaged;

        T Single<T>(params T[] values);
        T Single<T>(IEnumerable<T> values);

        void Fill(Span<byte> bytes);

        IEnumerable<T> Enumerate<T>(params T[] values);
        IEnumerable<T> Enumerate<T>(IEnumerable<T> values);
        
        IReadOnlyList<T> Shuffled<T>(params T[] values);
        IReadOnlyList<T> Shuffled<T>(IEnumerable<T> values);

        void Shuffle<T>(Span<T> values);
        void Shuffle<T>(IList<T> values);
    }
}