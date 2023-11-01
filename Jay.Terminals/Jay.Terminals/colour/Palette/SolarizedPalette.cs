using System;
using System.Drawing;

namespace Jay.Consolas.Palette
{
	/// <summary>
	/// http://ethanschoonover.com/solarized
	/// </summary>
	internal sealed class SolarizedPalette : Palette
	{
		/// <inheritdoc />
		public SolarizedPalette()
		{
			Set(ConsoleColor.Black, Color.FromArgb(0, 43, 54));
			Set(ConsoleColor.DarkGray, Color.FromArgb(7, 54, 66));
			Set(ConsoleColor.DarkGreen, Color.FromArgb(88, 110, 117));
			Set(ConsoleColor.DarkYellow, Color.FromArgb(101, 123, 131));
			Set(ConsoleColor.DarkBlue, Color.FromArgb(131, 148, 150));
			Set(ConsoleColor.DarkCyan, Color.FromArgb(147, 161, 161));
			Set(ConsoleColor.Gray, Color.FromArgb(238, 232, 213));
			Set(ConsoleColor.White, Color.FromArgb(253, 246, 227));
			Set(ConsoleColor.Yellow, Color.FromArgb(181, 137, 0));
			Set(ConsoleColor.DarkRed, Color.FromArgb(203, 75, 22));
			Set(ConsoleColor.Red, Color.FromArgb(220, 50, 47));
			Set(ConsoleColor.Magenta, Color.FromArgb(211, 54, 130));
			Set(ConsoleColor.DarkMagenta, Color.FromArgb(108, 113, 196));
			Set(ConsoleColor.Blue, Color.FromArgb(38, 139, 210));
			Set(ConsoleColor.Cyan, Color.FromArgb(42, 161, 152));
			Set(ConsoleColor.Green, Color.FromArgb(133, 153, 0));
		}
	}
}