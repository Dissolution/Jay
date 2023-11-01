using System;
using System.Drawing;

namespace Jay.Consolas.Palette
{
	/// <inheritdoc />
	internal sealed class DefaultColorPalette : Palette
	{
		public DefaultColorPalette()
		{
			Set(ConsoleColor.Black, Color.Black);
			Set(ConsoleColor.DarkBlue, Color.DarkBlue);
			Set(ConsoleColor.DarkGreen, Color.DarkGreen);
			Set(ConsoleColor.DarkCyan, Color.DarkCyan);
			Set(ConsoleColor.DarkRed, Color.DarkRed);
			Set(ConsoleColor.DarkMagenta, Color.DarkMagenta);
			Set(ConsoleColor.DarkYellow, Color.Orange);
			Set(ConsoleColor.Gray, Color.Gray);
			Set(ConsoleColor.DarkGray, Color.DarkGray);
			Set(ConsoleColor.Blue, Color.Blue);
			Set(ConsoleColor.Green, Color.Green);
			Set(ConsoleColor.Cyan, Color.Cyan);
			Set(ConsoleColor.Red, Color.Red);
			Set(ConsoleColor.Magenta, Color.Magenta);
			Set(ConsoleColor.Yellow, Color.Yellow);
			Set(ConsoleColor.White, Color.White);
		}
	}
}