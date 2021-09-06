using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Sandbox.Fun.Linez
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public readonly struct Color : IEquatable<Color>
    {
        public static bool operator ==(Color x, Color y) => x.Value == y.Value;
        public static bool operator !=(Color x, Color y) => x.Value != y.Value;
        public static Color operator +(Color x, Color y) => x.Add(y);
        public static Color operator -(Color x, Color y) => x.Sub(y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte AddLimit(byte x, byte y)
        {
            int z = (int) x + (int) y;
            if (z > byte.MaxValue)
                return byte.MaxValue;
            return (byte)z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte SubLimit(byte x, byte y)
        {
            int z = (int) x - (int) y;
            if (z < byte.MinValue)
                return byte.MinValue;
            return (byte)z;
        }
        
        internal const int BlueShift = 0;
        internal const int GreenShift = 8;
        internal const int RedShift = 16;
        //internal const int AlphaShift = 24;
        
        [FieldOffset(0)] 
        public readonly byte B;
        [FieldOffset(1)] 
        public readonly byte G;
        [FieldOffset(2)] 
        public readonly byte R;
        // [FieldOffset(3)] 
        // public readonly byte A;

        [FieldOffset(0)]
        internal readonly uint Value;

        public int Sum => B + G + R;

        public Color(byte red, byte green, byte blue)
        {
            //this.Value = (uint) (red << RedShift | green << GreenShift | blue << BlueShift);
            this = NotSafe.As<int, Color>((red << RedShift | green << GreenShift | blue << BlueShift));
        }

        public Color Add(Color color)
        {
            return new Color(AddLimit(R, color.R),
                             AddLimit(G, color.G),
                             AddLimit(B, color.B));
        }
        
        public Color Sub(Color color)
        {
            return new Color(SubLimit(R, color.R),
                             SubLimit(G, color.G),
                             SubLimit(B, color.B));
        }
        
        /// <inheritdoc />
        public bool Equals(Color color)
        {
            return Value == color.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Color color && color.Value == Value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int)Value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({R},{G},{B}";
        }
    }
}