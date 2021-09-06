using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Jay.Reflection.Cloning;

namespace Jay.Geometry
{
    public readonly struct Size<T> : IEquatable<Size<T>>
        where T : unmanaged
    {
        public static implicit operator Size<T>((T Width, T Height) size) => new Size<T>(size.Width, size.Height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Size<T> a, Size<T> b) => EqualityComparer<T>.Default.Equals(a.Width, b.Width) &&
                                                                  EqualityComparer<T>.Default.Equals(a.Height, b.Height);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Size<T> a, Size<T> b) => !EqualityComparer<T>.Default.Equals(a.Width, b.Width) ||
                                                                  !EqualityComparer<T>.Default.Equals(a.Height, b.Height);
        public readonly T Width;
        public readonly T Height;

        public Size(T width, T height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void Deconstruct(out T width, out T height)
        {
            width = this.Width;
            height = this.Height;
        }

        public Size<T> Copy() => new Size<T>(Width, Height);

        /// <inheritdoc />
        public bool Equals(Size<T> size)
        {
            return EqualityComparer<T>.Default.Equals(size.Width, this.Width) &&
                   EqualityComparer<T>.Default.Equals(size.Height, this.Height);
        }
        
        /// <inheritdoc />
        public bool Equals((T Width, T Height) size)
        {
            return EqualityComparer<T>.Default.Equals(size.Width, this.Width) &&
                   EqualityComparer<T>.Default.Equals(size.Height, this.Height);
        }
        
        /// <inheritdoc />
        public bool Equals(T width, T height)
        {
            return EqualityComparer<T>.Default.Equals(width, this.Width) &&
                   EqualityComparer<T>.Default.Equals(height, this.Height);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Size<T> size) 
                return Equals(size);
            if (obj is (T, T))
                return Equals(((T, T)) obj);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Hasher.Create<T,T>(Width, Height);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{Width}x{Height}]";
        }
    }
}