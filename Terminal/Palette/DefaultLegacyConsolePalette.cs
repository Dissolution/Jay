using System;
using System.Drawing;

namespace Jay.Consolas.Palette
{
	/// <inheritdoc />
	internal sealed class DefaultLegacyConsolePalette : Palette
	{
		public DefaultLegacyConsolePalette()
		{
			Set(ConsoleColor.Black, Color.FromArgb(0,0,0));
			Set(ConsoleColor.DarkBlue, Color.FromArgb(0,0,128));
			Set(ConsoleColor.DarkGreen, Color.FromArgb(0,128,0));
			Set(ConsoleColor.DarkCyan, Color.FromArgb(0,128,128));
			Set(ConsoleColor.DarkRed, Color.FromArgb(128,0,0));
			Set(ConsoleColor.DarkMagenta, Color.FromArgb(128,0,128));
			Set(ConsoleColor.DarkYellow, Color.FromArgb(128,128,0));
			Set(ConsoleColor.Gray, Color.FromArgb(192,192,192));
			Set(ConsoleColor.DarkGray, Color.FromArgb(128,128,128));
			Set(ConsoleColor.Blue, Color.FromArgb(0,0,255));
			Set(ConsoleColor.Green, Color.FromArgb(0,255,0));
			Set(ConsoleColor.Cyan, Color.FromArgb(0,255,255));
			Set(ConsoleColor.Red, Color.FromArgb(255,0,0));
			Set(ConsoleColor.Magenta, Color.FromArgb(255,0,255));
			Set(ConsoleColor.Yellow, Color.FromArgb(255,255,0));
			Set(ConsoleColor.White, Color.FromArgb(255,255,255));
		}
	}
}