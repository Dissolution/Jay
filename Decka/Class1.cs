using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static InlineIL.IL;

namespace Jay.Decka;

[StructLayout(LayoutKind.Explicit, Size = 1)]
public readonly struct Card : IEquatable<Card>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Card left, Card right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ceq();
        return Return<bool>();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Card left, Card right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ceq();
        Emit.Not();
        return Return<bool>();
    }

    [FieldOffset(0)] 
    private readonly byte _value;

    public Face Face
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            const byte mask = 0b01000000;
            return (Face)((_value & mask) >> 6);
        }
    }

    public Rank Rank
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            const byte mask = 0b00111100;
            return (Rank)((_value & mask) >> 2);
        }
    }

    public Suit Suit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            const byte mask = 0b00000011;
            return (Suit)((_value & mask));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Card card)
    {
        Emit.Ldarg(nameof(card));
        Emit.Ldarg(0);      // implicit this for instance method
        Emit.Ceq();
        return Return<bool>();
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Only takes up 2 bits
/// The rightmost bit also determines color (0 - black, 1 - red)
/// </remarks>
public enum Suit : byte
{
    Spade   = 0b00,
    Heart   = 0b01,
    Club    = 0b10,
    Diamond = 0b11,
}

public static class SuitExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBlack(this Suit suit)
    {
        Emit.Ldarg(nameof(suit));
        Emit.Ldc_I4_1();
        Emit.And();
        Emit.Ldc_I4_0();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRed(this Suit suit)
    {
        Emit.Ldarg(nameof(suit));
        Emit.Ldc_I4_1();
        Emit.And();
        Emit.Ldc_I4_1();
        Emit.Ceq();
        return Return<bool>();
    }
}

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Only takes up 4 bits
/// </remarks>
public enum Rank : byte
{
    None  = 0b0000,
    Ace   = 0b0001,
    Two   = 0b0010,
    Three = 0b0011,
    Four  = 0b0100,
    Five  = 0b0101,
    Six   = 0b0110,
    Seven = 0b0111,
    Eight = 0b1000,
    Nine  = 0b1001,
    Ten   = 0b1010,
    Jack  = 0b1011,
    Queen = 0b1100,
    King  = 0b1101,
    Joker = 0b1111,
}

public enum Face : byte
{
    Down = 0b0,
    Up   = 0b1,
}