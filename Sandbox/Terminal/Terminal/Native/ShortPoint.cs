using System;
using System.Runtime.InteropServices;
using Jay.Exceptions;

namespace Jay.Sandbox.Native
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct ShortPoint : IEquatable<ShortPoint>
    {
        public static bool operator ==(ShortPoint a, ShortPoint b) => a.Equals(b);
        public static bool operator !=(ShortPoint a, ShortPoint b) => !a.Equals(b);
        
        public static readonly ShortPoint Empty = new ShortPoint();
        
        [FieldOffset(0)] 
        public short X;
        [FieldOffset(2)] 
        public short Y;

        public ShortPoint(int x, int y)
        {
            this.X = (short) x;
            this.Y = (short) y;
        }
        
        /// <inheritdoc />
        public bool Equals(ShortPoint shortPoint)
        {
            return X == shortPoint.X && Y == shortPoint.Y;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is ShortPoint coord && Equals(coord);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeException.Throw(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}