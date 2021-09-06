using System;
using System.Runtime.CompilerServices;

namespace Jay.Comparison
{
	/// <summary>
	/// A utility for simplifying <see cref="string"/>s by stripping all non-ASCII characters, non-digits, non-letters, then uppercasing.
	/// </summary>
	public sealed class SimplifiedTextComparer : TextComparer<SimplifiedTextComparer>
	{
		private const int OFFSET = 'a' - 'A';

		public static string Simplify(string? text)
			=> Simplify((ReadOnlySpan<char>) text);
		
		public static string Simplify(params char[]? chars)
			=> Simplify((ReadOnlySpan<char>) chars);
		
		public static string Simplify(ReadOnlySpan<char> text)
		{
			Span<char> dest = stackalloc char[text.Length];
			int d = 0;
			char c;
			for (var i = 0; i < text.Length; i++)
			{
				c = text[i];
				switch (c)
				{
					case >= '0' and <= '9':
					case >= 'A' and <= 'Z':
					{
						dest[d++] = c;
						break;
					}
					case >= 'a' and <= 'z':
						dest[d++] = (char) (c - OFFSET);
						break;
					default:
						break;
				}
			}
			return new string(dest.Slice(0, d));
		}
	
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool CheckAndTransform(ref char c)
		{
			switch (c)
			{
				case >= '0' and <= '9':
				case >= 'A' and <= 'Z':
					return true;
				case >= 'a' and <= 'z':
					c = (char) (c - OFFSET);
					return true;
				default:
					return false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static char GetNextLegalChar(ReadOnlySpan<char> text, ref int i)
		{
			while (i < text.Length)
			{
				var c = text[i++];
				if (CheckAndTransform(ref c))
					return c;
			}
			return default;
		}

		public override bool Equals(ReadOnlySpan<char> xText, ReadOnlySpan<char> yText)
		{
			int x = 0;
			int y = 0;
			char xChar;
			char yChar;
			do
			{
				xChar = GetNextLegalChar(xText, ref x);
				yChar = GetNextLegalChar(yText, ref y);
				if (xChar != yChar)
					return false;
			} while (xChar != '\0' || yChar != '\0');
			
			//Both ended at the same time, so they're equal
			return true;
		}

		public override int GetHashCode(ReadOnlySpan<char> text)
		{
			var hasher = new Hasher();
			for (var i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (CheckAndTransform(ref c))
					hasher.AddHash((int) c);
			}
			return hasher.ToHashCode();
		}

		public override int Compare(ReadOnlySpan<char> xText, ReadOnlySpan<char> yText)
		{
			int x = 0;
			int y = 0;
			char xChar;
			char yChar;
			do
			{
				xChar = GetNextLegalChar(xText, ref x);
				yChar = GetNextLegalChar(yText, ref y);
				var c = xChar.CompareTo(yChar);
				if (c != 0)
					return c;
			} while (xChar != '\0' || yChar != '\0');

			//Both ended at the same time, so they're equal
			return 0;
		}
	}
}
