using System;
using System.Runtime.InteropServices;
using Jay.Exceptions;

namespace Jay.Sandbox.Native
{
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Unicode)]
    public struct Coxel : IEquatable<Coxel>
    {
        public static bool operator ==(Coxel a, Coxel b) => NotSafe.Unmanaged.Equal(a, b);
        public static bool operator !=(Coxel a, Coxel b) => NotSafe.Unmanaged.NotEqual(a, b);
        
        public static Coxel Default => new Coxel(' ', CoxelColor.Gray, CoxelColor.Black);
        
        [FieldOffset(0)]
        public CharUnion Char;

        [FieldOffset(2)]
        public CoxelColors Color;

        [FieldOffset(3)] 
        public CommonLvb CommonLvb;

        public Coxel(char unicodeChar, CoxelColor foreColor, CoxelColor backColor)
        {
            this = default;
            this.Char.UnicodeChar = unicodeChar;
            this.Color.Foreground = foreColor;
            this.Color.Background = backColor;
        }

        /// <inheritdoc />
        public bool Equals(Coxel coxel)
        {
            return NotSafe.Unmanaged.Equal(this, coxel);
        }
        
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Coxel coxel && NotSafe.Unmanaged.Equal(this, coxel);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeException.Throw(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Char.ToString();
        }
    }
}