using System;
using System.Collections.Generic;
using System.Drawing;

namespace Jay.Consolas.Palette
{
	/// <summary>
	/// A palette of <see cref="Color"/> values to apply to a <see cref="Console"/>.
	/// </summary>
	public interface IPalette
	{
		/// <summary>
		/// Gets the <see cref="Color"/> to use when writing to the <see cref="Console"/> with the specific <see cref="ConsoleColor"/>.
		/// </summary>
		/// <param name="consoleColor"></param>
		/// <returns></returns>
		Color this[ConsoleColor consoleColor] { get; }

		/// <summary>
		/// Gets the <see cref="ConsoleColor"/> to use when writing to the <see cref="Console"/> with the specified <see cref="Color"/>.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		ConsoleColor this[Color color] { get; }

		Color DefaultForeColor { get; }
		Color DefaultBackColor { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Black.
		/// </summary>
		Color Black { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Dark Blue.
		/// </summary>
		Color DarkBlue { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Dark Green.
		/// </summary>
		Color DarkGreen { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Dark Cyan.
		/// </summary>
		Color DarkCyan { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Dark Red.
		/// </summary>
		Color DarkRed { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Dark Magenta.
		/// </summary>
		Color DarkMagenta { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Dark Yellow.
		/// </summary>
		Color DarkYellow { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Gray.
		/// </summary>
		Color Gray { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Dark Gray.
		/// </summary>
		Color DarkGray { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Blue.
		/// </summary>
		Color Blue { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Green.
		/// </summary>
		Color Green { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Cyan.
		/// </summary>
		Color Cyan { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Red.
		/// </summary>
		Color Red { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Magenta.
		/// </summary>
		Color Magenta { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent Yellow.
		/// </summary>
		Color Yellow { get; }

		/// <summary>
		/// The <see cref="Color"/> to represent White.
		/// </summary>
		Color White { get; }

		bool TryGetColor(ConsoleColor cc, out Color color);
		bool TryGetConsoleColor(Color color, out ConsoleColor cc);
	}
}
