namespace Jay.Terminals.Native;

[Flags]
public enum CommonLVB : byte
{
    LeadingByte       = 0b00000001,
    TrailingByte      = 0b00000010,
    GridTopHorizontal = 0b00000100,
    GridLeftVertical  = 0b00001000,
    GridRightVertical = 0b00010000,
    //Gap               = 0b00100000,
    ReverseVideo      = 0b01000000,
    Underscore        = 0b10000000,
}

// /// <summary>
// /// 
// /// </summary>
// /// <see>DBCS</see>
// [StructLayout(LayoutKind.Explicit, Size = sizeof(byte))]
// public struct CommonLvb : IEquatable<CommonLvb>
// {
//     public static bool operator ==(CommonLvb x, CommonLvb y) => x.Value == y.Value;
//     public static bool operator !=(CommonLvb x, CommonLvb y) => x.Value != y.Value;
//
//     [FieldOffset(0)] 
//     internal byte Value;
//
//     public bool LeadingByte
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (Value & 0b00000001) == 1;
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         set => Value = (byte)((Value & 0b11111110) | (value ? 1 : 0));
//     }
//
//     public bool TrailingByte
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (Value & 0b00000010) >= 1;
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         set => Value = (byte)((Value & 0b11111101) | ((value ? 1 : 0) << 1));
//     }
//
//     public bool GridTopHorizontal
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (Value & 0b00000100) >= 1;
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         set => Value = (byte)((Value & 0b11111011) | ((value ? 1 : 0) << 2));
//     }
//
//     public bool GridLeftVertical
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (Value & 0b00001000) >= 1;
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         set => Value = (byte)((Value & 0b11110111) | ((value ? 1 : 0) << 3));
//     }
//
//     public bool GridRightVertical
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (Value & 0b00010000) >= 1;
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         set => Value = (byte)((Value & 0b11101111) | ((value ? 1 : 0) << 4));
//     }
//
//     // There is a gap here
//     public bool ReverseVideo
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (Value & 0b01000000) >= 1;
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         set => Value = (byte)((Value & 0b10111111) | ((value ? 1 : 0) << 6));
//     }
//
//     public bool Underscore
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => (Value & 0b10000000) >= 1;
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         set => Value = (byte)((Value & 0b01111111) | ((value ? 1 : 0) << 7));
//     }
//
//     /// <inheritdoc />
//     public bool Equals(CommonLvb commonLvb)
//     {
//         return commonLvb.Value == this.Value;
//     }
//
//     /// <inheritdoc />
//     public override bool Equals(object? obj)
//     {
//         return obj is CommonLvb commonLvb && commonLvb.Value == Value;
//     }
//
//     /// <inheritdoc />
//     public override int GetHashCode() => throw new InvalidOperationException();
//
//     /// <inheritdoc />
//     public override string ToString()
//     {
//         StringBuilder text = new();
//         if (LeadingByte)
//             text.Append("Leading Byte | ");
//         if (TrailingByte)
//             text.Append("Trailing Byte | ");
//         if (GridTopHorizontal)
//             text.Append("Grid Top Horizontal | ");
//         if (GridLeftVertical)
//             text.Append("Grid Left Vertical | ");
//         if (GridRightVertical)
//             text.Append("Grid Right Vertical | ");
//         if (ReverseVideo)
//             text.Append("Reverse Video | ");
//         if (Underscore)
//             text.Append("Underscore | ");
//         return text.ToString(0, text.Length - 3);
//     }
// }