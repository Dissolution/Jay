using System;
using System.Linq;
using System.Text;

namespace Jay.Text.Unicode
{
	/// <summary>
	/// Utility for transforming <see cref="string"/>s via unicode substitution.
	/// </summary>
    public static class UnicodeTransformer
    {
	    internal static TextBuilder AppendTransform(this TextBuilder builder, ReadOnlySpan<char> text, UnicodeTransformation transformation)
	    {
		    switch (transformation)
		    {
			    case UnicodeTransformation.Bold:
				    return UnicodeBold(builder, text);
			    case UnicodeTransformation.Italic:
				    return UnicodeItalic(builder, text);
			    case UnicodeTransformation.BoldItalic:
				    return UnicodeBoldItalic(builder, text);
			    case UnicodeTransformation.Script:
				    return UnicodeScript(builder, text);
			    case UnicodeTransformation.BoldScript:
				    return UnicodeBoldScript(builder, text);
			    case UnicodeTransformation.Fraktur:
				    return UnicodeFraktur(builder, text);
			    case UnicodeTransformation.DoubleStruck:
				    return UnicodeDoubleStruck(builder, text);
			    case UnicodeTransformation.BoldFraktur:
				    return UnicodeBoldFraktur(builder, text);
			    case UnicodeTransformation.SansSerif:
				    return UnicodeSansSerif(builder, text);
			    case UnicodeTransformation.SansSerifBold:
				    return UnicodeSansSerifBold(builder, text);
			    case UnicodeTransformation.SansSerifItalic:
				    return UnicodeSansSerifItalic(builder, text);
			    case UnicodeTransformation.SansSerifBoldItalic:
				    return UnicodeSansSerifBoldItalic(builder, text);
			    case UnicodeTransformation.Monospace:
				    return UnicodeMonospace(builder, text);
			    case UnicodeTransformation.Underline:
				    return UnicodeUnderline(builder, text);
			    case UnicodeTransformation.None:
			    default:
				    return builder.Append(text);
		    }
	    }

	    private static void Transform(TextBuilder builder, 
	                                  ReadOnlySpan<char> text,
	                                  int upperCaseA,
	                                  int lowerCaseA,
	                                  int zero)
	    {
		    // Process each character in turn
		    foreach (char c in text)
		    {
			    if (c >= 'A' && c <= 'Z')
			    {
				    builder.Write((char) (c + upperCaseA - 'A'));
			    }
			    else if (c >= 'a' && c <= 'z')
			    {
				    builder.Write((char) (c + lowerCaseA - 'a'));
			    }
			    else if (c >= '0' && c <= '9')
			    {
				    builder.Write((char) (c + zero - '0'));
			    }
			    else
			    {
				    builder.Write(c);
			    }
		    }
	    }
	    
	    private static TextBuilder UnicodeBold(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D400, 0x1D41A, 0x1D7CE);
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeItalic(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D434, 0x1D44E, '0');
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeBoldItalic(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D468, 0x1D482, '0');
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeScript(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D49C, 0x1D4B6, '0');
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeBoldScript(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D4D0, 0x1D4EA, '0');
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeFraktur(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D504, 0x1D51E, '0');
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeDoubleStruck(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D538, 0x1D552, 0x1D7D8);
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeBoldFraktur(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D56C, 0x1D586, '0');
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeSansSerif(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D5A0, 0x1D5BA, 0x1D7E2);
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeSansSerifBold(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D5D4, 0x1D5EE, 0x1D7EC);
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeSansSerifItalic(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D608, 0x1D622, '0');
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeSansSerifBoldItalic(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D63C, 0x1D656, '0');
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeMonospace(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    Transform(builder, text, 0x1D670, 0x1D68A, 0x1D7F6);
		    return builder;
	    }
	    
	    private static TextBuilder UnicodeUnderline(TextBuilder builder, ReadOnlySpan<char> text)
	    {
		    // Our underline is just a special character that we append after every normal character
		    const char underline = '\u0332';
		    foreach (var c in text)
		    {
			    builder.Write(c);
			    builder.Write(underline);
		    }
		    return builder;
	    }

/*
		private static string UnicodeCircledNumber(int number)
		{
			//0
			if (number == 0)
				return "⓪";
			//1-20
			if (number >= 1 && number <= 20)
				return char.ConvertFromUtf32(number + 9311); //U+2460
			return number.ToString();
		}

		/// <summary>
		/// Transform this <see cref="string"/> using Unicode Empty Circles.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string TransformUnicodeCircled(string str)
		{
			//Kick back original string if we can't do anything with it
			if (str.IsNullOrEmpty())
				return str;

			//Process
			var sb = new StringBuilder(str.Length * 2);
			var numberParts = str.Split((r, o) => char.IsDigit(o) && !char.IsDigit(r),
				(r, o) => char.IsDigit(r) && !char.IsDigit(o));
			foreach (var part in numberParts)
			{
				//Check for Digits
				if (int.TryParse(part, out int i))
				{
					if (i >= 0 && i <= 20)
					{
						sb.Append(UnicodeCircledNumber(i));
					}
					else
					{
						//Process each digit individually
						foreach (var n in part.Select(c => int.Parse(c.ToString())))
						{
							sb.Append(UnicodeCircledNumber(n));
						}
					}
				}
				//Non-numeric
				else
				{
					//Proces each character
					foreach (var c in part)
					{
						//Upper
						if (char.IsUpper(c))
						{
							sb.Append(char.ConvertFromUtf32(c + 9333)); //U+24B6
						}
						else if (char.IsLower(c))
						{
							sb.Append(char.ConvertFromUtf32(c + 9327));
						}
						else
						{
							sb.Append(c);
						}
					}
				}
			}

			//Finished
			return sb.ToString();
		}

		/// <summary>
		/// Transform this <see cref="string"/> using Unicode Full Width.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string TransformUnicodeFullWidth(string str)
		{
			//Kick back original string if we can't do anything with it
			if (str.IsNullOrEmpty())
				return str;
			//Process
			var sb = new StringBuilder(str.Length * 2);
			foreach (var c in str)
			{
				//Is this character present in the fullwidth set?
				if (c >= '!' && c <= '~')
					sb.Append(char.ConvertFromUtf32(c + 65248));
				else
				{
					//1:1
					sb.Append(c);
				}
			}
			//Finished
			return sb.ToString();
		}
			*/
	}
}
