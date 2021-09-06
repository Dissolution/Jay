using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jay.Exceptions;
using Jay.Text;

namespace Jay.Sandbox.Native
{
    /// <summary>
    /// 
    /// </summary>
    /// <see>DBCS</see>
    [StructLayout(LayoutKind.Explicit, Size = sizeof(byte))]
    public struct CommonLvb : IEquatable<CommonLvb>
    {
        public static bool operator ==(CommonLvb x, CommonLvb y) => x._byte == y._byte;
        public static bool operator !=(CommonLvb x, CommonLvb y) => x._byte != y._byte;
        
        [FieldOffset(0)] 
        private byte _byte;

        public bool LeadingByte
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_byte & 0b00000001) == 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _byte = (byte)((_byte & 0b11111110) | (value ? 1 : 0));
        }
            
        public bool TrailingByte
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_byte & 0b00000010) >= 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _byte = (byte)((_byte & 0b11111101) | ((value ? 1 : 0) << 1));
        }
            
        public bool GridTopHorizontal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_byte & 0b00000100) >= 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _byte = (byte)((_byte & 0b11111011) | ((value ? 1 : 0) << 2));
        }
        public bool GridLeftVertical
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_byte & 0b00001000) >= 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _byte = (byte)((_byte & 0b11110111) | ((value ? 1 : 0) << 3));
        }
        public bool GridRightVertical
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_byte & 0b00010000) >= 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _byte = (byte)((_byte & 0b11101111) | ((value ? 1 : 0) << 4));
        }
        // There is a gap here
        public bool ReverseVideo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_byte & 0b01000000) >= 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _byte = (byte)((_byte & 0b10111111) | ((value ? 1 : 0) << 6));
        }
        public bool Underscore
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_byte & 0b10000000) >= 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _byte = (byte)((_byte & 0b01111111) | ((value ? 1 : 0) << 7));
        }

        /// <inheritdoc />
        public bool Equals(CommonLvb commonLvb)
        {
            return commonLvb._byte == this._byte;
        }
        
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is CommonLvb commonLvb && commonLvb._byte == _byte;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeException.Throw(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(this, (text, clvb) =>
            {
                if (clvb.LeadingByte)
                    text.Append("Leading Byte | ");
                if (clvb.TrailingByte)
                    text.Append("Trailing Byte | ");
                if (clvb.GridTopHorizontal)
                    text.Append("Grid Top Horizontal | ");
                if (clvb.GridLeftVertical)
                    text.Append("Grid Left Vertical | ");
                if (clvb.GridRightVertical)
                    text.Append("Grid Right Vertical | ");
                if (clvb.ReverseVideo)
                    text.Append("Reverse Video | ");
                if (clvb.Underscore)
                    text.Append("Underscore | ");
                text.TrimEnd(" | ");
            });
        }
    }
}