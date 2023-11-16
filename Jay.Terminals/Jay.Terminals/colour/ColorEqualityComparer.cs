using System;
using System.Collections.Generic;
using System.Drawing;
using Jay.Extensions;


namespace Jay.Consolas
{
	/// <summary>
	/// An equality comparer for <see cref="Color"/> values.
	/// </summary>
	public sealed class ColorEqualityComparer : IEqualityComparer<Color>, IEqualityComparer<Color?>
	{
		public bool Equals(Color x, Color y)
		{
			if (x.IsNamedColor && y.IsNamedColor)
				return x.Name == y.Name;

			return x.R == y.R &&
			       x.G == y.G &&
			       x.B == y.B &&
			       x.A == y.A;
		}

		public int GetHashCode(Color color)
		{
			return HashCode.Combine(color.A, color.R, color.G, color.B);
		}

		/// <inheritdoc />
		public bool Equals(Color? x, Color? y)
		{
			if (x is null && y is null)
				return true;
			if (x is null)
				return false;
			if (y is null)
				return false;
			return Equals(x.Value, y.Value);
		}

		/// <inheritdoc />
		public int GetHashCode(Color? color)
		{
			if (!color.TryGetValue(out Color c))
				return 0;
			return GetHashCode(c);
		}

		/// <summary>
		/// Get the distance between two colors in the RGB color space.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static int Distance(Color c1, Color c2)
		{ 
			return (int) Math.Sqrt(
				(c1.R - c2.R) * (c1.R - c2.R) + (c1.G - c2.G) * (c1.G - c2.G) + (c1.B - c2.B)*(c1.B - c2.B)
				);
		}
	}
}
