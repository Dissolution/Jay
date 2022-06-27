using System.Numerics;
using System.Runtime.InteropServices;
using Jay.Reflection;
using Jay.Text;

namespace Jay;

[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
public struct Bit64 : ISpanFormattable
{
    internal const int FormatLength = 6 + 64 + 1;
    internal const ulong LeftMostBitMask = 1UL << 63;

    public static implicit operator ulong(Bit64 bits) => Danger.DirectCast<Bit64, ulong>(bits);
    public static implicit operator Bit64(ulong data) => Danger.DirectCast<ulong, Bit64>(data);

    public static bool operator ==(Bit64 left, Bit64 right) => Unmanaged.Equals(left, right);
    public static bool operator !=(Bit64 left, Bit64 right) => !Unmanaged.Equals(left, right);

    public static bool operator <(Bit64 left, Bit64 right) => Unmanaged.LessThan(left, right);
    public static bool operator <=(Bit64 left, Bit64 right) => Unmanaged.LessThanOrEqual(left, right);
    public static bool operator >(Bit64 left, Bit64 right) => Unmanaged.GreaterThan(left, right);
    public static bool operator >=(Bit64 left, Bit64 right) => Unmanaged.GreaterThanOrEqual(left, right);

    public static Bit64 operator |(Bit64 left, Bit64 right) => Unmanaged.Or(left, right);
    public static Bit64 operator &(Bit64 left, Bit64 right) => Unmanaged.And(left, right);
    public static Bit64 operator ^(Bit64 left, Bit64 right) => Unmanaged.Xor(left, right);
    public static Bit64 operator !(Bit64 bits) => Unmanaged.Not(bits);
    public static Bit64 operator ~(Bit64 bits) => Unmanaged.Negate(bits);

    
    [FieldOffset(0)]
    private ulong _data;

    public int PopCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => BitOperations.PopCount(_data);
    }

    public bool this[int bit]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var mask = 1UL << bit;
            return (_data & mask) == mask;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var mask = 1UL << bit;
            if (value)
            {
                _data = (_data | mask);
            }
            else
            {
                _data = (_data & ~mask);
            }
        }
    }

    public ulong Data => _data;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Bit64 bits)
            return bits._data == _data;
        if (obj is ulong data)
            return data == _data;
        return false;
    }

    public override int GetHashCode()
    {
        return (int)((_data >> 32) ^ _data);
    }

    /// <inheritdoc />
    public bool TryFormat(Span<char> destination, 
                          out int charsWritten, 
                          ReadOnlySpan<char> format = default, 
                          IFormatProvider? provider = default)
    {
        const string prefix = "Bit64{";
        
        charsWritten = FormatLength;
        if (destination.Length < FormatLength)
        {
            return false;
        }
        TextHelper.CopyTo(prefix, destination);
        destination[^1] = '}';
        destination = destination.Slice(prefix.Length, 64);
        ulong data = _data;
        for (int i = 0; i < destination.Length; i++)
        {
            
            destination[i] = (data & LeftMostBitMask) != 0 ? '1' : '0';
            data <<= 1;
        }
        return true;
    }
    
    /// <inheritdoc />
    public string ToString(string? format, 
                           IFormatProvider? formatProvider = null)
    {
        return ToString();
    }
    
    public override string ToString()
    {
        return string.Create(FormatLength, this, (span, bit64) =>
        {
            bit64.TryFormat(span, out _);
        });
    }

    

   
}