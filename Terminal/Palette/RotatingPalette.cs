using System;
using System.Collections.Generic;
using System.Drawing;

namespace Jay.Consolas.Palette
{
	/// <inheritdoc />
	internal sealed class RotatingPalette : Palette
	{
		private readonly Queue<ConsoleColor> _availableConsoleColors;
		private readonly ColorMapper _colorMapper = new ColorMapper();

		public RotatingPalette()
		{
			_availableConsoleColors = new Queue<ConsoleColor>(16);
			foreach (var cc in Enums<ConsoleColor>.Members)
			{
				_availableConsoleColors.Enqueue(cc);
			}
		}

		/// <inheritdoc />
		protected override ConsoleColor GetClosestColor(Color color)
		{
			//Have to replace a console color
			var cc = _availableConsoleColors.Dequeue();
			//Push back to the end of the queue
			_availableConsoleColors.Enqueue(cc);
			//Push to mapper
			_colorMapper.MapColor(cc, color);
			//Done
			return cc;
		}

		/// <inheritdoc />
		public override bool TryGetConsoleColor(Color color, out ConsoleColor cc)
		{
			if (_colorToCc.TryGetValue(color, out cc))
				return true;
			//Have to replace a console color
			cc = _availableConsoleColors.Dequeue();
			_ccToColor[cc] = color;
			_colorToCc[color] = cc;
			//Push back to the end of the queue
			_availableConsoleColors.Enqueue(cc);
			//Push to mapper
			_colorMapper.MapColor(cc, color);
			//Done
			return true;
		}
	}
}