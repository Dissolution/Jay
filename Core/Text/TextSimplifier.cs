/*
using Jay.Comparison;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jay.Text
{
	/// <summary>
	/// Utility class for generating simplified <see cref="string"/>s, by stripping all non-ASCII, non-digit, not-letter characters, then converting to uppercase.
	/// </summary>
    public sealed class TextSimplifier : ITextEqualityComparer, ITextComparer
	{
		private const int CHAR_OFFSET = 'a' - 'A';
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool CheckAndTransform(ref char c)
		{
			//0-9 and A-Z go right through
			if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))
				return true;
			//a-z get upper-cased
			if (c >= 'a' && c <= 'z')
			{
				c = (char)(c - CHAR_OFFSET);
				return true;
			}
			//All others do not pass
			return false;
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

        /// <summary>
        /// Simplify the specified <see cref="string"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
		public string Simplify(ReadOnlySpan<char> text)
        {
	        return TextHelper.BuildString(text, (span, sb) =>
	        {
		        for (var i = 0; i < span.Length; i++)
		        {
			        var c = span[i];
			        if (CheckAndTransform(ref c))
				        sb.Append(c);
		        }
	        });
		}

		/// <inheritdoc />
		public bool Equals(string xString, string yString)
		{
			if (xString is null && yString is null)
				return true;
			if (xString is null || yString is null)
				return false;
			int x = 0;
			int y = 0;
			char xChar;
			char yChar;
			do
			{
				xChar = GetNextLegalChar(xString, ref x);
				yChar = GetNextLegalChar(yString, ref y);
				if (xChar != yChar)
					return false;
			} while (xChar != '\0' || yChar != '\0');

			//Both ended at the same time, so they're equal
			return true;
		}

		/// <inheritdoc />
		public int GetHashCode(string str)
		{
			if (str is null) return 0;
			var hash = new Hasher();
			for (var i = 0; i < str.Length; i++)
			{
				var c = str[i];
				if (CheckAndTransform(ref c))
					hash.Add(c);
			}
			//Fin
			return hash.ToHashCode();
		}

		/// <inheritdoc />
		public int Compare(string xString, string yString)
		{
			if (xString is null && yString is null)
				return 0;
			if (xString is null)
				return -1;
			if (yString is null)
				return 1;
			int x = 0;
			int y = 0;
			char xChar;
			char yChar;
			do
			{
				xChar = GetNextLegalChar(xString, ref x);
				yChar = GetNextLegalChar(yString, ref y);
				var c = xChar.CompareTo(yChar);
				if (c != 0)
					return c;
			} while (xChar != '\0' || yChar != '\0');

			//Both ended at the same time, so they're equal
			return 0;
		}

	    /// <inheritdoc />
	    public bool Equals(char x, char y)
	    {
	        CheckAndTransform(ref x);
	        CheckAndTransform(ref y);
	        return x == y;
	    }

	    /// <inheritdoc />
	    public int GetHashCode(char c)
	    {
	        return CheckAndTransform(ref c) ? c.GetHashCode() : 0;
	    }

	    /// <inheritdoc />
	    public int Compare(char x, char y)
	    {
	        if (CheckAndTransform(ref x))
	        {
	            if (CheckAndTransform(ref y))
	            {
	                return x.CompareTo(y);
	            }
	            else
	            {
	                //X is good, Y is bad, y sorts before x
	                return 1;
	            }
	        }
	        else
	        {
	            if (CheckAndTransform(ref y))
	            {
                    //Y is good, X is bad, x sorts before y
	                return -1;
	            }
	            else
	            {
	                //Both are bad
	                return 0;
	            }
	        }
	    }
	}
}
*/
