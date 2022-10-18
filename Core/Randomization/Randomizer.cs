using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Jay.Enums;
using Jay.Reflection;

namespace Jay.Randomization;

public abstract class Randomizer : IRandomizer
{
    public static IRandomizer Instance { get; } = new Xoshiro256StarStarRandomizer();

    public static T Generate<T>()
       where T : unmanaged
    {
        Span<byte> bytes = stackalloc byte[Danger.SizeOf<T>()];
        RandomNumberGenerator.Fill(bytes);
        return MemoryMarshal.Read<T>(bytes);
    }

    public abstract bool IsHighResolution { get; }

    protected Randomizer()
    {

    }

    protected static bool TryValidateGenerate<T>(
        T inclusiveMin, 
        T inclusiveMax,
        Func<T> getFullRangeValue,
        out T? singleValue,
        [CallerArgumentExpression(nameof(inclusiveMin))]
        string inclusiveMinName = "",
        [CallerArgumentExpression(nameof(inclusiveMax))]
        string inclusiveMaxName = "")
        where T : IMinMaxValue<T>, IComparable<T>, IEquatable<T>
    {
        int c = inclusiveMin.CompareTo(inclusiveMax);
        if (c > 0)
        {
            throw new ArgumentOutOfRangeException(
                inclusiveMaxName,
                inclusiveMax,
                $"{inclusiveMaxName} must be greater than or equal to {inclusiveMinName}");
        }

        if (c == 0)
        {
            singleValue = inclusiveMin;
            return true;
        }

        if (inclusiveMin.Equals(T.MinValue) && 
            inclusiveMax.Equals(T.MaxValue))
        {
            singleValue = getFullRangeValue();
            return true;
        }

        singleValue = default;
        return false;
    }

    public abstract byte Byte();

    public byte Between(byte inclusiveMin, byte inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, Byte, out var value))
            return value;
        uint range = ((uint)inclusiveMax - (uint)inclusiveMin) + 1U;
        return (byte)((uint)inclusiveMin + ZeroTo(range));
    }

    public abstract sbyte SByte();

    public sbyte Between(sbyte inclusiveMin, sbyte inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, SByte, out var value))
            return value;
        int range = ((int)inclusiveMax - (int)inclusiveMin) + 1;
        return (sbyte)((int)inclusiveMin + ZeroTo(range));
    }

    public abstract short Short();

    public short Between(short inclusiveMin, short inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, Short, out var value))
            return value;
        int range = ((int)inclusiveMax - (int)inclusiveMin) + 1;
        return (short)((int)inclusiveMin + ZeroTo(range));
    }

    public abstract ushort UShort();

    public ushort Between(ushort inclusiveMin, ushort inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, UShort, out var value))
            return value;
        uint range = ((uint)inclusiveMax - (uint)inclusiveMin) + 1U;
        return (ushort)((uint)inclusiveMin + ZeroTo(range));
    }

    public abstract int Int();

    public int Between(int inclusiveMin, int inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, Int, out var value))
            return value;
        int range = (inclusiveMax - inclusiveMin) + 1;
        return inclusiveMin + ZeroTo(range);
    }

    public abstract int ZeroTo(int exclusiveMax);

    public int IntPositive()
    {
        // Get 31 bits to force positive
        return (int)(ULong() >> 33);
    }

    public abstract uint UInt();

    public uint Between(uint inclusiveMin, uint inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, UInt, out var value))
            return value;
        uint range = (inclusiveMax - inclusiveMin) + 1U;
        return inclusiveMin + ZeroTo(range);

    }

    public abstract uint ZeroTo(uint exclusiveMax);
    public abstract long Long();

    public long Between(long inclusiveMin, long inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, Long, out var value))
            return value;
        ulong range = (ulong)((inclusiveMax - inclusiveMin) + 1L);
        return unchecked(inclusiveMin + (long)ZeroTo(range));
    }

    /// <summary>
    /// Returns a random positive <see cref="long"/> value [0..long.MaxValue]
    /// </summary>
    public long LongPositive()
    {
        // Get 63 bits to force positive
        return (long)(ULong() >> 1);
    }

    public abstract ulong ULong();

    public ulong Between(ulong inclusiveMin, ulong inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, ULong, out var value))
            return value;
        ulong range = (inclusiveMax - inclusiveMin) + 1L;
        return inclusiveMin + ZeroTo(range);

    }
    public abstract ulong ZeroTo(ulong exclusiveMax);

    public abstract float Float();

    public float Between(float inclusiveMin, float inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, Float, out var value))
            return value;
        float range = (inclusiveMax - inclusiveMin) + float.Epsilon;
        float r = FloatPercent() * range;
        return inclusiveMin + r;
    }

    public abstract float FloatPercent();

    public abstract double Double();

    public double Between(double inclusiveMin, double inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, Double, out var value))
            return value;
        double range = (inclusiveMax - inclusiveMin) + double.Epsilon;
        double r = DoublePercent() * range;
        return inclusiveMin + r;
    }

    public abstract double DoublePercent();

    public abstract decimal Decimal();

    public decimal Between(decimal inclusiveMin, decimal inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, Decimal, out var value))
            return value;
        decimal range = (inclusiveMax - inclusiveMin) + 1.0m;
        decimal r = DecimalPercent() * range;
        return inclusiveMin + r;
    }

    public abstract decimal DecimalPercent();

    public abstract bool Bool();

    public Guid Guid()
    {
        Span<byte> buffer = stackalloc byte[16];
        Fill(buffer);
        return new Guid(buffer);
    }

    public TimeSpan TimeSpan()
    {
        return new TimeSpan(ticks: Long());
    }

    public TimeSpan Between(TimeSpan inclusiveMin, TimeSpan inclusiveMax)
    {
        return new TimeSpan(ticks: Between(inclusiveMin.Ticks, inclusiveMax.Ticks));
    }

    public DateTime DateTime(DateTimeKind kind = DateTimeKind.Unspecified)
    {
        return new DateTime(ticks: (long)ULong(), kind: kind);
    }

    public DateTime Between(DateTime inclusiveMin, DateTime inclusiveMax)
    {
        return new DateTime(
            ticks: Between(inclusiveMin.Ticks, inclusiveMax.Ticks),
            kind: inclusiveMin.Kind);
    }

    public DateTimeOffset DateTimeOffset()
    {
        return new DateTimeOffset(ticks: Long(),
            offset: new TimeSpan(ticks: Long()));
    }

    public DateTimeOffset Between(DateTimeOffset inclusiveMin, DateTimeOffset inclusiveMax)
    {
        return new DateTimeOffset(
            dateTime: Between(inclusiveMin.DateTime, inclusiveMax.DateTime),
            offset: Between(inclusiveMin.Offset, inclusiveMax.Offset));
    }

    public TimeOnly TimeOnly()
    {
        return new TimeOnly(ticks: Long());
    }

    public TimeOnly Between(TimeOnly inclusiveMin, TimeOnly inclusiveMax)
    {
        return new TimeOnly(ticks: Between(inclusiveMin.Ticks, inclusiveMax.Ticks));
    }

    public DateOnly DateOnly()
    {
        int year = Between(1, 9999);
        int month = Between(1, 12);
        int day = Between(1, System.DateTime.DaysInMonth(year, month));
        return new DateOnly(year, month, day);
    }

    public DateOnly Between(DateOnly inclusiveMin, DateOnly inclusiveMax)
    {
        return System.DateOnly.FromDayNumber(Between(inclusiveMin.DayNumber, inclusiveMax.DayNumber));
    }

    public abstract char Char();

    public char Between(char inclusiveMin, char inclusiveMax)
    {
        if (TryValidateGenerate(inclusiveMin, inclusiveMax, Char, out var value))
            return value;
        uint range = ((uint)inclusiveMax - (uint)inclusiveMin) + 1U;
        return (char)((uint)inclusiveMin + ZeroTo(range));
    }

    public char Single(string text) => Single<char>((ReadOnlySpan<char>)text);
    public char Single(ReadOnlySpan<char> text) => Single<char>(text);


    public string HexString(int length)
    {
        if (length <= 0) return string.Empty;
        // Each byte will turn into two characters
        Span<byte> bytes = stackalloc byte[Maths.HalfRoundUp(length)];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).Substring(0, length);
    }

    public TEnum Enum<TEnum>()
        where TEnum : struct, Enum
    {
        return Single<TEnum>(EnumInfo.For<TEnum>().Members);
    }

    public T? Nullable<T>(float nullChance)
        where T : unmanaged
    {
        if (FloatPercent() <= nullChance)
            return null;
        return Unmanaged<T>();
    }

    public T Unmanaged<T>()
        where T : unmanaged
    {
        Span<byte> buffer = stackalloc byte[Danger.SizeOf<T>()];
        Fill(buffer);
        return Danger.ReadUnaligned<T>(buffer);
    }

    public abstract void Fill(Span<byte> bytes);

    public void Fill<T>(Span<T> span)
        where T : unmanaged
    {
        Fill(MemoryMarshal.Cast<T, byte>(span));
    }

    public T Single<T>(ReadOnlySpan<T> values)
    {
        return values[ZeroTo(values.Length)];
    }

    public T Single<T>(params T[] values)
    {
        return values[ZeroTo(values.Length)];
    }

    public T Single<T>(IEnumerable<T> values)
    {
        if (values is IList<T> list)
        {
            if (list.Count == 0)
                throw new ArgumentException("There must be at least one value to return", nameof(values));
            return list[ZeroTo(list.Count)];
        }

        if (values is ICollection<T> collection)
        {
            if (collection.Count == 0)
                throw new ArgumentException("There must be at least one value to return", nameof(values));
            var r = ZeroTo(collection.Count);
            using (var e = collection.GetEnumerator())
            {
                do
                {
                    e.MoveNext();
                    r--;
                } while (r >= 0);

                return e.Current;
            }
        }

        T? value = default;
        int count = 0;
        using (var e = values.GetEnumerator())
        {
            while (e.MoveNext())
            {
                count++;
                if (ZeroTo(count) == 0)
                {
                    value = e.Current;
                }
            }
        }

        if (count == 0)
        {
            throw new ArgumentException("There must be at least one value to return", nameof(values));
        }

        return value!;
    }

    /// <remarks>
    /// To initialize an array a of n elements to a randomly shuffled copy of source, both 0-based:
    /// for i from 0 to n − 1 do
    /// j ← random integer such that 0 ≤ j ≤ i
    /// if j ≠ i
    ///     a[i] ← a[j]
    /// a[j] ← source[i]
    /// </remarks>
    public IReadOnlyList<T> MixToList<T>(ReadOnlySpan<T> values)
    {
        var len = values.Length;
        var array = new T[len];
        int r;
        for (var i = 0; i < len; i++)
        {
            r = ZeroTo(i + 1);
            if (r != i)
            {
                array[i] = array[r];
            }
            array[r] = values[i];
        }
        return array;
    }


    public IReadOnlyList<T> MixToList<T>(params T[] values)
    {
        var len = values.Length;
        var array = new T[len];
        int r;
        for (var i = 0; i < len; i++)
        {
            r = ZeroTo(i + 1);
            if (r != i)
            {
                array[i] = array[r];
            }
            array[r] = values[i];
        }
        return array;
    }

    public IReadOnlyList<T> MixToList<T>(IList<T> list)
    {
        var len = list.Count;
        var array = new T[len];
        int r;
        for (var i = 0; i < len; i++)
        {
            r = ZeroTo(i + 1);
            if (r != i)
            {
                array[i] = array[r];
            }
            array[r] = list[i];
        }
        return array;
    }


    /// <remarks>
    /// To initialize an empty array a to a randomly shuffled copy of source whose length is not known:
    /// while source.moreDataAvailable
    ///     j ← random integer such that 0 ≤ j ≤ a.length
    ///     if j = a.length
    ///         a.append(source.next)
    ///     else
    ///         a.append(a[j])
    ///         a[j] ← source.next
    /// </remarks>
    public IReadOnlyList<T> MixToList<T>(IEnumerable<T> values)
    {
        var list = new List<T>();
        using (var e = values.GetEnumerator())
        {
            int r;
            while (e.MoveNext())
            {
                r = ZeroTo(list.Count);
                if (r == list.Count)
                {
                    list.Add(e.Current);
                }
                else
                {
                    list.Add(list[r]);
                    list[r] = e.Current;
                }
            }
            return list;
        }
    }


    /// <remarks>
    /// -- To shuffle an array a of n elements (indices 0..n-1):
    /// for i from n−1 downto 1 do
    /// j ← random integer such that 0 ≤ j ≤ i
    ///     exchange a[j] and a[i]
    /// </remarks>
    public void Mix<T>(Span<T> values)
    {
        T temp;
        int r;
        for (var i = values.Length - 1; i > 0; i--)
        {
            r = ZeroTo(i + 1);
            temp = values[i];
            values[i] = values[r];
            values[r] = temp;
        }
    }


    public void Mix<T>(IList<T> values)
    {
        T temp;
        int r;
        for (var i = values.Count - 1; i > 0; i--)
        {
            r = ZeroTo(i + 1);
            temp = values[i];
            values[i] = values[r];
            values[r] = temp;
        }
    }


    public void Mix<T>(T[] values)
    {
        T temp;
        int r;
        for (var i = values.Length - 1; i > 0; i--)
        {
            r = ZeroTo(i + 1);
            temp = values[i];
            values[i] = values[r];
            values[r] = temp;
        }
    }
}