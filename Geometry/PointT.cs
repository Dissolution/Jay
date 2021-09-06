using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Jay.Reflection.Cloning;

namespace Jay.Geometry
{
    public readonly struct Point<T> : IEquatable<Point<T>>
        where T : unmanaged
    {
        public static implicit operator Point<T>((T X, T Y) point) => new Point<T>(point.X, point.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Point<T> a, Point<T> b) => EqualityComparer<T>.Default.Equals(a.X, b.X) &&
                                                                  EqualityComparer<T>.Default.Equals(a.Y, b.Y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Point<T> a, Point<T> b) => !EqualityComparer<T>.Default.Equals(a.X, b.X) ||
                                                                  !EqualityComparer<T>.Default.Equals(a.Y, b.Y);
        public readonly T X;
        public readonly T Y;

        public Point(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }

        public void Deconstruct(out T x, out T y)
        {
            x = this.X;
            y = this.Y;
        }

        public Point<T> Copy() => new Point<T>(X, Y);

        /// <inheritdoc />
        public bool Equals(Point<T> point)
        {
            return EqualityComparer<T>.Default.Equals(point.X, this.X) &&
                   EqualityComparer<T>.Default.Equals(point.Y, this.Y);
        }
        
        /// <inheritdoc />
        public bool Equals((T X, T Y) point)
        {
            return EqualityComparer<T>.Default.Equals(point.X, this.X) &&
                   EqualityComparer<T>.Default.Equals(point.Y, this.Y);
        }
        
        /// <inheritdoc />
        public bool Equals(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, this.X) &&
                   EqualityComparer<T>.Default.Equals(y, this.Y);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Point<T> point) 
                return Equals(point);
            if (obj is (T, T))
                return Equals(((T, T)) obj);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Hasher.Create<T,T>(X, Y);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}