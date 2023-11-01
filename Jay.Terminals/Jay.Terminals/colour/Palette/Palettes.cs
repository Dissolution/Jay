using System.Drawing;
using System.Linq;

namespace Jay.Consolas.Palette
{
	/// <summary>
	/// Storage for predefined <see cref="IPalette"/>s.
	/// </summary>
	/// <remarks>
	/// https://blogs.msdn.microsoft.com/commandline/2017/08/02/updating-the-windows-console-colors/
	/// </remarks>
	public static class Palettes
	{
		/// <summary>
		/// The default <see cref="IPalette"/> loaded from the current <see cref="Console"/>.
		/// </summary>
		public static IPalette Default { get; }

		/// <summary>
		/// The default <see cref="IPalette"/> for the legacy Windows console.
		/// </summary>
		public static IPalette DefaultLegacyConsole { get; }

		/// <summary>
		/// The modified <see cref="IPalette"/> for Windows 10's console.
		/// </summary>
		public static IPalette DefaultWin10Console { get; }

		/// <summary>
		/// The <see cref="IPalette"/> using <see cref="System.Drawing.Color"/>s.
		/// </summary>
		public static IPalette DefaultColor { get; }

		/// <summary>
		/// The Solarized <see cref="IPalette"/>.
		/// </summary>
		public static IPalette Solarized { get; }

		/// <summary>
		/// Get a custom <see cref="IPalette"/>, based upon <see cref="Default"/>, that allows you to use any <see cref="Color"/> you wish by overwriting the 'oldest' color each time.
		/// </summary>
		public static IPalette Custom => new RotatingPalette();

		static Palettes()
		{
			var mapper = new ColorMapper();
			var colors = mapper.GetBufferColors();
			Default = new Palette(colors.ToDictionary(p => p.Key, p => p.Value.GetColor()));
			DefaultLegacyConsole = new DefaultLegacyConsolePalette();
			DefaultWin10Console = new DefaultWin10ConsolePalette();
			DefaultColor = new DefaultColorPalette();
			Solarized = new SolarizedPalette();
		}
	}
}