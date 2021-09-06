using System;
using System.Runtime.InteropServices;
using Jay.Exceptions;

namespace Jay.Sandbox.Native
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct ShortSize : IEquatable<ShortSize>
    {
        public static bool operator ==(ShortSize a, ShortSize b) => a.Equals(b);
        public static bool operator !=(ShortSize a, ShortSize b) => !a.Equals(b);
        
        public static readonly ShortPoint Empty = new ShortPoint();
        
        [FieldOffset(0)] 
        public short Width;
        [FieldOffset(2)] 
        public short Height;

        public ShortSize(int width, int height)
        {
            this.Width = (short) width;
            this.Height = (short) height;
        }
        
        /// <inheritdoc />
        public bool Equals(ShortSize size)
        {
            return this.Width == size.Width &&
                   this.Height == size.Height;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is ShortSize size && Equals(size);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeException.Throw(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{Width}x{Height}]";
        }
    }
}