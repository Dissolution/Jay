using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jay.Exceptions;

namespace Jay.Sandbox.Native
{
    [StructLayout(LayoutKind.Explicit, Size = sizeof(byte))]
    public struct CoxelColors : IEquatable<CoxelColors>
    {
        public static explicit operator CoxelColors(byte b) => NotSafe.As<byte, CoxelColors>(b);
        public static implicit operator CoxelColors((CoxelColor ForeColor, CoxelColor BackColor) colors)
            => new CoxelColors(colors.ForeColor, colors.BackColor);

        public static bool operator ==(CoxelColors x, CoxelColors y) => x._byte == y._byte;
        public static bool operator !=(CoxelColors x, CoxelColors y) => x._byte != y._byte;
        
        public static readonly CoxelColors None = new CoxelColors();

        [FieldOffset(0)] 
        private byte _byte;

        public CoxelColor Foreground
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (CoxelColor) (_byte & 0b00001111);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _byte = (byte) ((_byte & 0b11110000) | (byte) value);
        }
        
        public CoxelColor Background
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (CoxelColor) ((_byte & 0b11110000) >> 4);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _byte = (byte) ((_byte & 0b00001111) | ((int) value << 4));
        }

        public CoxelColors(CoxelColor foreColor, CoxelColor backColor)
        {
            _byte = (byte) ((int) foreColor | ((int) backColor << 4));
        }

        /// <inheritdoc />
        public bool Equals(CoxelColors colors)
        {
            return colors._byte == this._byte;
        }
        
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is CoxelColors colors && colors._byte == this._byte;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeException.Throw(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Foreground}|{Background}";
        }
    }
}