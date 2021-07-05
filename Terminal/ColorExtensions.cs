using System;
using System.Collections.Generic;
using System.Drawing;

namespace Jay.Consolas
{
	/// <summary>
	/// Extensions on <see cref="Color"/>s.
	/// </summary>
	public static class ColorExtensions
	{
		private static readonly Dictionary<ConsoleColor, Color> _consoleColors;
		private const double _epsilon = 0.001d;

		static ColorExtensions()
		{
			_consoleColors = new Dictionary<ConsoleColor, Color>
			{
				[ConsoleColor.Black] = Color.FromArgb(0, 0, 0),
				[ConsoleColor.DarkGray] = Color.FromArgb(128, 128, 128),
				[ConsoleColor.Gray] = Color.FromArgb(192, 192, 192),
				[ConsoleColor.White] = Color.FromArgb(255, 255, 255),
				[ConsoleColor.Blue] = Color.FromArgb(0, 0, 255),
				[ConsoleColor.DarkBlue] = Color.FromArgb(0, 0, 128),
				[ConsoleColor.Green] = Color.FromArgb(0, 255, 0),
				[ConsoleColor.DarkGreen] = Color.FromArgb(0, 128, 0),
				[ConsoleColor.Red] = Color.FromArgb(255, 0, 0),
				[ConsoleColor.DarkRed] = Color.FromArgb(128, 0, 0),
				[ConsoleColor.Cyan] = Color.FromArgb(0, 255, 255),
				[ConsoleColor.DarkCyan] = Color.FromArgb(0, 128, 128),
				[ConsoleColor.Magenta] = Color.FromArgb(255, 0, 255),
				[ConsoleColor.DarkMagenta] = Color.FromArgb(128, 0, 128),
				[ConsoleColor.Yellow] = Color.FromArgb(255, 255, 0),
				[ConsoleColor.DarkYellow] = Color.FromArgb(128, 128, 0)
			};
		}

		/// <summary>
		/// Gets the nearest <see cref="ConsoleColor"/> to this <see cref="Color"/>.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static ConsoleColor ToNearestConsoleColor(this Color color)
		{
			ConsoleColor cc = 0;
			var delta = double.MaxValue;
			foreach (var pair in _consoleColors)
			{
				var cColor = pair.Value;
				var diff = Math.Pow(cColor.R - color.R, 2d) +
				           Math.Pow(cColor.G - color.G, 2d) +
				           Math.Pow(cColor.B - color.B, 2d);
				if (diff < _epsilon)
					return pair.Key;
				if (diff < delta)
				{
					delta = diff;
					cc = pair.Key;
				}
			}
			//Fin
			return cc;
		}

		/// <summary>
		/// Gets the nearest <see cref="Color"/> to this <see cref="ConsoleColor"/>.
		/// </summary>
		/// <param name="consoleColor"></param>
		/// <returns></returns>
		public static Color ToNearestColor(this ConsoleColor consoleColor)
		{
			return _consoleColors[consoleColor];
		}
	}
}
