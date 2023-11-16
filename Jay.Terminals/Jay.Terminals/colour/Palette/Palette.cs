using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Jay.Collections;
using Jay.Enums.Scratch;

using ConsoleColor = System.ConsoleColor;

namespace Jay.Consolas.Palette
{
	public class Palette : IPalette
	{
		protected readonly Dictionary<ConsoleColor, Color> _ccToColor;
		protected readonly Dictionary<Color, ConsoleColor> _colorToCc;

		public Color this[ConsoleColor consoleColor]
		{
			get => _ccToColor[consoleColor];
			set => _ccToColor[consoleColor] = value;
		}

		public ConsoleColor this[Color color]
		{
			get
			{
				if (TryGetConsoleColor(color, out ConsoleColor cc))
					return cc;
				return GetClosestColor(color);
			}
			set => _colorToCc[color] = value;
		}

		/// <inheritdoc />
		public Color DefaultForeColor { get; set; }

		/// <inheritdoc />
		public Color DefaultBackColor { get; set; }

		/// <inheritdoc />
		public Color Black
		{
			get => _ccToColor[ConsoleColor.Black];
			set => _ccToColor[ConsoleColor.Black] = value;
		}

		/// <inheritdoc />
		public Color DarkBlue
		{
			get => _ccToColor[ConsoleColor.DarkBlue];
			set => _ccToColor[ConsoleColor.DarkBlue] = value;
		}

		/// <inheritdoc />
		public Color DarkGreen
		{
			get => _ccToColor[ConsoleColor.DarkGreen];
			set => _ccToColor[ConsoleColor.DarkGreen] = value;
		}

		/// <inheritdoc />
		public Color DarkCyan
		{
			get => _ccToColor[ConsoleColor.DarkCyan];
			set => _ccToColor[ConsoleColor.DarkCyan] = value;
		}

		/// <inheritdoc />
		public Color DarkRed
		{
			get => _ccToColor[ConsoleColor.DarkRed];
			set => _ccToColor[ConsoleColor.DarkRed] = value;
		}

		/// <inheritdoc />
		public Color DarkMagenta
		{
			get => _ccToColor[ConsoleColor.DarkMagenta];
			set => _ccToColor[ConsoleColor.DarkMagenta] = value;
		}

		/// <inheritdoc />
		public Color DarkYellow
		{
			get => _ccToColor[ConsoleColor.DarkYellow];
			set => _ccToColor[ConsoleColor.DarkYellow] = value;
		}

		/// <inheritdoc />
		public Color Gray
		{
			get => _ccToColor[ConsoleColor.Gray];
			set => _ccToColor[ConsoleColor.Gray] = value;
		}

		/// <inheritdoc />
		public Color DarkGray
		{
			get => _ccToColor[ConsoleColor.DarkGray];
			set => _ccToColor[ConsoleColor.DarkGray] = value;
		}

		/// <inheritdoc />
		public Color Blue
		{
			get => _ccToColor[ConsoleColor.Blue];
			set => _ccToColor[ConsoleColor.Blue] = value;
		}

		/// <inheritdoc />
		public Color Green
		{
			get => _ccToColor[ConsoleColor.Green];
			set => _ccToColor[ConsoleColor.Green] = value;
		}

		/// <inheritdoc />
		public Color Cyan
		{
			get => _ccToColor[ConsoleColor.Cyan];
			set => _ccToColor[ConsoleColor.Cyan] = value;
		}

		/// <inheritdoc />
		public Color Red
		{
			get => _ccToColor[ConsoleColor.Red];
			set => _ccToColor[ConsoleColor.Red] = value;
		}

		/// <inheritdoc />
		public Color Magenta
		{
			get => _ccToColor[ConsoleColor.Magenta];
			set => _ccToColor[ConsoleColor.Magenta] = value;
		}

		/// <inheritdoc />
		public Color Yellow
		{
			get => _ccToColor[ConsoleColor.Yellow];
			set => _ccToColor[ConsoleColor.Yellow] = value;
		}

		/// <inheritdoc />
		public Color White
		{
			get => _ccToColor[ConsoleColor.White];
			set => _ccToColor[ConsoleColor.White] = value;
		}


		public Palette()
			: this(Palettes.Default) { }

		public Palette(IPalette originalPalette)
		{
			if (originalPalette is null)
				throw new ArgumentNullException();
			_ccToColor = new Dictionary<ConsoleColor, Color>();
			_colorToCc = new Dictionary<Color, ConsoleColor>(new ColorEqualityComparer());
			foreach (var cc in EnumTypeInfo<ConsoleColor>.Members)
			{
				var color = originalPalette[cc];
				_ccToColor[cc] = color;
				_colorToCc[color] = cc;
			}

			this.DefaultForeColor = originalPalette.DefaultForeColor;
			this.DefaultBackColor = originalPalette.DefaultBackColor;
		}

		public Palette(IDictionary<ConsoleColor, Color> colorMap)
		{
			if (colorMap is null)
				throw new ArgumentNullException();
			_ccToColor = new Dictionary<ConsoleColor, Color>();
			_colorToCc = new Dictionary<Color, ConsoleColor>(new ColorEqualityComparer());
			foreach (var cc in EnumTypeInfo<ConsoleColor>.Members)
			{
				var color = colorMap[cc];
				_ccToColor[cc] = color;
				_colorToCc[color] = cc;
			}

			this.DefaultForeColor = colorMap[ConsoleColor.Gray];
			this.DefaultBackColor = colorMap[ConsoleColor.Black];
		}

		protected virtual ConsoleColor GetClosestColor(Color color)
		{
			//Find closest existing color and use that
			return _colorToCc
				.OrderBy(p => ColorEqualityComparer.Distance(color, p.Key))
				.First().Value;
		}

		protected void Set(ConsoleColor cc, Color color)
		{
			_ccToColor[cc] = color;
			_colorToCc[color] = cc;
		}

		public bool TryGetColor(ConsoleColor cc, out Color color) => _ccToColor.TryGetValue(cc, out color);

		public virtual bool TryGetConsoleColor(Color color, out ConsoleColor cc)
		{
			if (_colorToCc.TryGetValue(color, out cc))
				return true;
			//Find closest existing color and use that
			cc = GetClosestColor(color);
			return true;
		}
	}
}
