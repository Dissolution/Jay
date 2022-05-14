using InlineIL;
using System.Diagnostics;
using System.Runtime.InteropServices;

using static InlineIL.IL;
// ReSharper disable StaticMemberInGenericType
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace Jay;

public static class Bits
{
       
}

public static class Bits<T> where T : unmanaged
{
    private static readonly int _byteSize;
    private static readonly int _bitSize;
    private static readonly T _one;

    public static int BitSize => _bitSize;
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

    private static ReadOnlySpan<int> ToInts(T value)
    {
        var valueSpan = MemoryMarshal.CreateReadOnlySpan<T>(ref value, 1);
        var intSpan = MemoryMarshal.Cast<T, int>(valueSpan);
        return intSpan;
    }
    private static T FromInts(ReadOnlySpan<int> ints)
    {
        var valueSpan = MemoryMarshal.Cast<int, T>(ints);
        Debug.Assert(valueSpan.Length == 1);
        return valueSpan[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Or(T x, T y)
    {
        var xSpan = ToInts(x);
        var ySpan = ToInts(y);
        var len = xSpan.Length;
        Span<int> output = stackalloc int[len];
        for (var i = 0; i < len; i++)
        {
            output[i] = xSpan[i] | ySpan[i];
        }

        T outT = FromInts(output);
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