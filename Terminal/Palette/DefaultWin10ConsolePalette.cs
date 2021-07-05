using System;
using System.Drawing;

namespace Jay.Consolas.Palette
{
	/// <inheritdoc />
	internal sealed class DefaultWin10ConsolePalette : Palette
	{
		public DefaultWin10ConsolePalette()
		{
			Set(ConsoleColor.Black, Color.FromArgb(12, 12, 12));
			Set(ConsoleColor.DarkBlue, Color.FromArgb(0, 55, 218));
			Set(ConsoleColor.DarkGreen, Color.FromArgb(19, 161, 14));
			Set(ConsoleColor.DarkCyan, Color.FromArgb(58, 150, 221));
			Set(ConsoleColor.DarkRed, Color.FromArgb(197, 15, 31));
			Set(ConsoleColor.DarkMagenta, Color.FromArgb(136, 23, 152));
			Set(ConsoleColor.DarkYellow, Color.FromArgb(193, 156, 0));
			Set(ConsoleColor.Gray, Color.FromArgb(204, 204, 204));
			Set(ConsoleColor.DarkGray, Color.FromArgb(118, 118, 118));
			Set(ConsoleColor.Blue, Color.FromArgb(59, 120, 255));
			Set(ConsoleColor.Green, Color.FromArgb(22, 198, 12));
			Set(ConsoleColor.Cyan, Color.FromArgb(97, 214, 214));
			Set(ConsoleColor.Red, Color.FromArgb(231, 72, 86));
			Set(ConsoleColor.Magenta, Color.FromArgb(180, 0, 158));
			Set(ConsoleColor.Yellow, Color.FromArgb(249, 241, 165));
			Set(ConsoleColor.White, Color.FromArgb(242, 242, 242));
		}
	}
}