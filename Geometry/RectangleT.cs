using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jay.Geometry
{
    public static class Rectangle
    {
        public static Rectangle<T> FromLTWH<T>(T left, T top, T width, T height)
            where T : unmanaged
            => new Rectangle<T>(left, top, width, height);
        public static Rectangle<T> FromLTRB<T>(T left, T top, T right, T bottom)
            where T : unmanaged
            => new Rectangle<T>(left, top, NotSafe.Unmanaged.Subtract<T>(right, left), NotSafe.Unmanaged.Subtract<T>(bottom, top));

        public static Rectangle<T> FromPointSize<T>(Point<T> point, Size<T> size)
            where T : unmanaged
            => new Rectangle<T>(point.X, point.Y, size.Width, size.Height);
    }

    [Flags]
    public enum RectanglePosition : int
    {
        Center = 0,
        Left = 1 << 0,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3,
    }
    
    public readonly struct Rectangle<T> : IEquatable<Rectangle<T>>
        where T : unmanaged
    {
        public static implicit operator Rectangle<T>((T left, T right, T width, T height) rect) =>
            new Rectangle<T>(rect.left, rect.right, rect.width, rect.height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Rectangle<T> a, Rectangle<T> b) =>
            EqualityComparer<T>.Default.Equals(a.Left, b.Left) &&
            EqualityComparer<T>.Default.Equals(a.Top, b.Top) &&
            EqualityComparer<T>.Default.Equals(a.Width, b.Width) &&
            EqualityComparer<T>.Default.Equals(a.Height, b.Height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Rectangle<T> a, Rectangle<T> b) =>
            !EqualityComparer<T>.Default.Equals(a.Left, b.Left) ||
            !EqualityComparer<T>.Default.Equals(a.Top, b.Top) ||
            !EqualityComparer<T>.Default.Equals(a.Width, b.Width) ||
            !EqualityComparer<T>.Default.Equals(a.Height, b.Height);

        public readonly T Left;
        public T X
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Left;
        }

        public readonly T Top;
        public T Y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Top;
        }

        public readonly T Width;

        public T Right
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => NotSafe.Unmanaged.Add<T>(Left, Width);
        }
        public readonly T Height;
        public T Bottom
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => NotSafe.Unmanaged.Add<T>(Top, Height);
        }

        public Point<T> Location => new Point<T>(Left, Top);
        public Size<T> Size => new Size<T>(Width, Height);
        
        internal Rectangle(T left, T top, T width, T height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }

        public void Deconstruct(out T left, out T top, out T width, out T height)
        {
            left = this.Left;
            top = this.Top;
            width = this.Width;
            height = this.Height;
        }
        public void Deconstruct(out Point<T> location, out Size<T> size)
        {
            location = this.Location;
            size = this.Size;
        }

        public bool Contains(Point<T> point)
        {
            return Comparer<T>.Default.Compare(point.X, this.Left) >= 0 &&
                   Comparer<T>.Default.Compare(point.X, this.Right) <= 0 &&
                   Comparer<T>.Default.Compare(point.Y, this.Top) >= 0 &&
                   Comparer<T>.Default.Compare(point.Y, this.Bottom) <= 0;
        }

        public Point<T> Position(RectanglePosition position)
        {
            T x;
            if (position.HasFlag<RectanglePosition>(RectanglePosition.Left))
            {
                x = this.Left;
            }
            else if (position.HasFlag<RectanglePosition>(RectanglePosition.Right))
            {
                x = this.Right;
            }
            else
            {
                x = NotSafe.Unmanaged.Divide<T>(NotSafe.Unmanaged.Add<T>(Left, Right), NotSafe.Unmanaged.As<int, T>(2));
            }
            
            T y;
            if (position.HasFlag<RectanglePosition>(RectanglePosition.Left))
            {
                y = this.Left;
            }
            else if (position.HasFlag<RectanglePosition>(RectanglePosition.Right))
            {
                y = this.Right;
            }
            else
            {
                y = NotSafe.Unmanaged.Divide<T>(NotSafe.Unmanaged.Add<T>(Top, Bottom), NotSafe.Unmanaged.As<int, T>(2));
            }

            return new Point<T>(x, y);
        }

        /// <inheritdoc />
        public bool Equals(Rectangle<T> rectangle)
        {
            return EqualityComparer<T>.Default.Equals(rectangle.Left, this.Left) &&
                   EqualityComparer<T>.Default.Equals(rectangle.Top, this.Top) &&
                   EqualityComparer<T>.Default.Equals(rectangle.Width, this.Width) &&
                   EqualityComparer<T>.Default.Equals(rectangle.Height, this.Height);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Rectangle<T> rectangle)
                return Equals(rectangle);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Hasher.Create<T, T, T, T>(Left, Top, Width, Height);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[({Left},{Top})-[{Width}x{Height}]]";
        }
    }
}