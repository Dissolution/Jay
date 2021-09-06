using System;
using System.Runtime.InteropServices;
using Jay.Exceptions;

namespace Jay.Sandbox.Native
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct ShortRect : IEquatable<ShortRect>
    {
        public static bool operator ==(ShortRect a, ShortRect b) => a.Equals(b);
        public static bool operator !=(ShortRect a, ShortRect b) => !a.Equals(b);
        
        public static ShortRect FromLTRB(int left, int top, int right, int bottom) => new ShortRect(left, top, right, bottom);
        public static ShortRect FromLTWH(int left, int top, int width, int height) => new ShortRect(left, top, left + width, top + height);
        public static ShortRect FromPointSize(ShortPoint point, ShortSize size) => new ShortRect(point.X, point.Y, point.X + size.Width, point.Y + size.Height);
        
        [FieldOffset(0)]
        public short Left;
        [FieldOffset(2)] 
        public short Top;
        [FieldOffset(4)] 
        public short Right;
        [FieldOffset(6)] 
        public short Bottom;
        
        public ShortPoint Location => new ShortPoint {X = Left, Y = Top};
        public ShortSize Size => new ShortSize(Right - Left, Bottom - Top);
        public int Width => Right - Left;
        public int Height => Bottom - Top;

        internal ShortRect(int left, int top, int right, int bottom)
        {
            this.Left = (short) left;
            this.Top = (short) top;
            this.Right = (short) right;
            this.Bottom = (short) bottom;
        }

        /// <inheritdoc />
        public bool Equals(ShortRect shortRect)
        {
            return shortRect.Left == this.Left &&
                   shortRect.Top == this.Top &&
                   shortRect.Right == this.Right &&
                   shortRect.Bottom == this.Bottom;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is ShortRect smallRect && Equals(smallRect);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeException.Throw(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{Left},{Top}/{Right},{Bottom}]";
        }
    }
}