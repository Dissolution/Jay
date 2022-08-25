using InlineIL;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Jay.Reflection;
using static InlineIL.IL;
// ReSharper disable StaticMemberInGenericType
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace Jay;


public ref struct BitRef<T>
    where T : unmanaged, INumber<T>, IShiftOperators<T, T>, IBitwiseOperators<T, T, T>
{
    private ref T _value;

    public readonly int Size;

    public bool this[int bit]
    {
        get => GetBit(bit);
        set => SetBit(bit, value);
    }

    public BitRef(ref T value)
    {
        _value = ref value;
        Size = Danger.SizeOf<T>();
        if (Size > sizeof(ulong))
            throw new NotImplementedException($"Cannot support unmanaged values larger than {sizeof(ulong)}");
    }

    public void SetBit(int bit, bool value)
    {
        if ((uint)bit >= Size)
            throw new ArgumentOutOfRangeException(nameof(bit), bit, $"Bit position {bit} must be [0..{Size})");
        T mask = T.One << bit;
        _value |= mask;
    }

    public bool GetBit(int bit)
    {
        throw new NotImplementedException();
    }
}


public static class Bits<T> where T : unmanaged
{
    private static readonly int _byteSize;
    private static readonly int _bitSize;
    private static readonly T _one;

    public static int BitSize => _bitSize;
    public static int ByteSize => _byteSize;
    public static T Zero => default;
    public static T One => _one;

    static Bits()
    {
        unsafe
        {
            _byteSize = sizeof(T);
            _bitSize = sizeof(T) * 8;
        }
        _one = AsT(1UL);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T AsT(ulong value)
    {
        Emit.Ldarg(nameof(value));
        return Return<T>();
    }

    public static void SetBit(ref T value, int bit)
    {
        if ((uint)bit >= _bitSize)
            throw new ArgumentOutOfRangeException(nameof(bit));
        T mask = LeftShift(One, bit);
        value = Or(value, mask);
    }

    public static void ClearBit(ref T value, int bit)
    {
        if ((uint)bit >= _bitSize)
            throw new ArgumentOutOfRangeException(nameof(bit));
        T mask = Not(LeftShift(One, bit));
        value = And(value, mask);
    }

    public static void ToggleBit(ref T value, int bit)
    {
        if ((uint)bit >= _bitSize)
            throw new ArgumentOutOfRangeException(nameof(bit));
        T mask = LeftShift(One, bit);
        value = Xor(value, mask);
    }

    public static bool IsBitSet(T value, int bit)
    {
        if ((uint)bit >= _bitSize)
            throw new ArgumentOutOfRangeException(nameof(bit));
        return Equals(And(RightShift(value, bit), One), One);
    }

    public static void SetBit(ref T value, int bit, bool yn)
    {
        if ((uint)bit >= _bitSize)
            throw new ArgumentOutOfRangeException(nameof(bit));
        // number = (number & ~(1UL << n)) | (x << n);
        var leftLeft = value;
        var leftRight = Not(LeftShift(One, bit));
        var left = And(leftLeft, leftRight);
        var right = LeftShift((yn ? One : Zero), bit);
        value = Or(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T And(T x, T y)
    {
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.And();
        return IL.Return<T>();
    }

    private static ReadOnlySpan<int> ToInt32Span(in T value)
    {
        var valueSpan = Danger.InToSpan(in value, 1);
        var intSpan = Danger.CastSpan<T, int>(valueSpan);
        return intSpan;
    }
    private static T FromInt32Span(ReadOnlySpan<int> ints)
    {
        var valueSpan = MemoryMarshal.Cast<int, T>(ints);
        Debug.Assert(valueSpan.Length == 1);
        return valueSpan[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Or(T x, T y)
    {
        var xSpan = ToInt32Span(x);
        var ySpan = ToInt32Span(y);
        var len = xSpan.Length;
        Span<int> output = stackalloc int[len];
        for (var i = 0; i < len; i++)
        {
            output[i] = xSpan[i] | ySpan[i];
        }

        T outT = FromInt32Span(output);
        return outT;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Xor(T x, T y)
    {
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Xor();
        return IL.Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Not(T value)
    {
        Emit.Ldarg(nameof(value));
        Emit.Not();
        return IL.Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Neg(T value)
    {
        Emit.Ldarg(nameof(value));
        Emit.Neg();
        return IL.Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LeftShift(T value, int count)
    {
        Emit.Ldarg(nameof(value));
        Emit.Ldarg(nameof(count));
        Emit.Shl();
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T RightShift(T value, int count)
    {
        Emit.Ldarg(nameof(value));
        Emit.Ldarg(nameof(count));
        Emit.Shr();
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(T x, T y)
    {
        Emit.Ldarg(nameof(x));
        Emit.Ldarg(nameof(y));
        Emit.Ceq();
        return Return<bool>();
    }
}