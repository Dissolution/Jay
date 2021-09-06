using System;
using System.Text;

namespace Jay.Text
{
    public class TextTransformer
    {
		/// <summary>
		/// Transform this <see cref="string"/> using Unicode Bold.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string TransformUnicodeBold(string str)
		{
			//Kick back original string if we can't do anything with it
			if (str.IsNullOrEmpty())
				return str;
			//Process
			var sb = new StringBuilder(str.Length * 2);
			foreach (var c in str)
			{
				var offset = 0;

				//Upper Case letter
				if (char.IsUpper(c))
				{
					offset = 119743;
				}
				//Lower Case letter
				else if (char.IsLower(c))
				{
					offset = 119737;
				}
				//Digit
				else if (char.IsDigit(c))
				{
					offset = 120734;
				}

				//Apply offset
				var nc = char.ConvertFromUtf32(c + offset);
				sb.Append(nc);
			}
			//Finished
			return sb.ToString();
		}

		/// <summary>
		/// Transform this <see cref="string"/> using Unicode Underline.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string TransformUnicodeUnderline(string str)
		{
			//Kick back original string if we can't do anything with it
			if (str.IsNullOrEmpty())
				return str;

			//Our underline is a special character that gets appended to each character
			const string underline = "\u0332";

			//Process
			var sb = new StringBuilder(str.Length + (str.Length * underline.Length));
			foreach (var c in str)
			{
				sb.Append(c).Append(underline);
			}
			//Finished
			return sb.ToString();
		}

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
			// var numberParts = str.Split((r, o) => char.IsDigit(o) && !char.IsDigit(r),
			// 	(r, o) => char.IsDigit(r) && !char.IsDigit(o));
			throw new NotImplementedException();
			
			// foreach (var part in numberParts)
			// {
			// 	//Check for Digits
			// 	if (int.TryParse(part, out int i))
			// 	{
			// 		if (i >= 0 && i <= 20)
			// 		{
			// 			sb.Append(UnicodeCircledNumber(i));
			// 		}
			// 		else
			// 		{
			// 			//Proces each digit individually
			// 			foreach (var n in part.Select(c => int.Parse(c.ToString())))
			// 			{
			// 				sb.Append(UnicodeCircledNumber(n));
			// 			}
			// 		}
			// 	}
			// 	//Non-numeric
			// 	else
			// 	{
			// 		//Proces each character
			// 		foreach (var c in part)
			// 		{
			// 			//Upper
			// 			if (char.IsUpper(c))
			// 			{
			// 				sb.Append(char.ConvertFromUtf32(c + 9333)); //U+24B6
			// 			}
			// 			else if (char.IsLower(c))
			// 			{
			// 				sb.Append(char.ConvertFromUtf32(c + 9327));
			// 			}
			// 			else
			// 			{
			// 				sb.Append(c);
			// 			}
			// 		}
			// 	}
			// }
			//
			// //Finished
			// return sb.ToString();
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
    }
}