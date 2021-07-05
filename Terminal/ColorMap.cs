//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;

//namespace Jay.Consolas
//{
//	internal sealed class ColorMap
//	{
//		private static readonly ColorEqualityComparer _colorComparer = new ColorEqualityComparer();
//		private static readonly ColorMapper _mapper = new ColorMapper();
//		private readonly Dictionary<Color, ConsoleColor> _colorToConsoleColors;
//		private readonly Dictionary<ConsoleColor, Color> _consoleColorToColors;
//		private const int MAX_COLORS = 16;		//Windows limit

//		private readonly Queue<ConsoleColor> _availableConsoleColors;

//		public Color DefaultForeColor { get; private set; }
//		public Color DefaultBackColor { get; private set; }

//		public ColorMap()
//		{
//			_colorToConsoleColors = new Dictionary<Color, ConsoleColor>(_colorComparer);
//			_consoleColorToColors = new Dictionary<ConsoleColor, Color>
//			{
//				[ConsoleColor.Black] = Color.FromArgb(0, 0, 0),
//				[ConsoleColor.DarkGray] = Color.FromArgb(128, 128, 128),
//				[ConsoleColor.Gray] = Color.FromArgb(192, 192, 192),
//				[ConsoleColor.White] = Color.FromArgb(255, 255, 255),
//				[ConsoleColor.Blue] = Color.FromArgb(0, 0, 255),
//				[ConsoleColor.DarkBlue] = Color.FromArgb(0, 0, 128),
//				[ConsoleColor.Green] = Color.FromArgb(0, 255, 0),
//				[ConsoleColor.DarkGreen] = Color.FromArgb(0, 128, 0),
//				[ConsoleColor.Red] = Color.FromArgb(255, 0, 0),
//				[ConsoleColor.DarkRed] = Color.FromArgb(128, 0, 0),
//				[ConsoleColor.Cyan] = Color.FromArgb(0, 255, 255),
//				[ConsoleColor.DarkCyan] = Color.FromArgb(0, 128, 128),
//				[ConsoleColor.Magenta] = Color.FromArgb(255, 0, 255),
//				[ConsoleColor.DarkMagenta] = Color.FromArgb(128, 0, 128),
//				[ConsoleColor.Yellow] = Color.FromArgb(255, 255, 0),
//				[ConsoleColor.DarkYellow] = Color.FromArgb(128, 128, 0)
//			};

//			_availableConsoleColors = new Queue<ConsoleColor>(MAX_COLORS);
//			foreach (var cc in Enums.Data<ConsoleColor>().Members)
//			{
//				if (cc == ConsoleColor.Gray || cc == ConsoleColor.Black)
//					continue;
//				_availableConsoleColors.Enqueue(cc);
//			}

//			this.DefaultForeColor = _consoleColorToColors[ConsoleColor.Gray];
//			this.DefaultBackColor = _consoleColorToColors[ConsoleColor.Black];
//		}

//		public Color GetColor(ConsoleColor consoleColor)
//		{
//			return _consoleColorToColors[consoleColor];
//		}

//		public ConsoleColor GetConsoleColor(Color color)
//		{
//			if (_colorToConsoleColors.TryGetValue(color, out var cc))
//				return cc;
//			//Have to replace one.
//			var toReplace = _availableConsoleColors.Dequeue();
//			_mapper.MapColor(toReplace, color);
//			_colorToConsoleColors[color] = toReplace;
//			//Back in the end of the queue
//			_availableConsoleColors.Enqueue(toReplace);
//			return toReplace;
//		}

//		//public ColorMap Replace(Color oldColor, Color newColor)
//		//{
//		//	if (_colorToConsoleColors.TryGetValue(oldColor, out var consoleColor))
//		//	{
//		//		_colorToConsoleColors.Remove(oldColor);
//		//		_colorToConsoleColors[newColor] = consoleColor;
//		//		_consoleColorToColors[consoleColor] = newColor;
//		//	}

//		//	return this;
//		//}

//		//public bool RequiresUpdate(Color color)
//		//{
//		//	return !_colorToConsoleColors.ContainsKey(color);
//		//}

//		public ColorMap Reset()
//		{
//			var defaults = _consoleColorToColors.ToDictionary(p => p.Key, p => new COLORREF(p.Value));
//			_mapper.SetBatchBufferColors(defaults);
//			_colorToConsoleColors.Clear();
//			System.Console.ResetColor();
//			System.Console.ForegroundColor = GetConsoleColor(DefaultForeColor);
//			System.Console.BackgroundColor = GetConsoleColor(DefaultBackColor);
//			return this;
//		}

//		public void SetDefaultColors(Color foreColor, Color backColor)
//		{
//			_mapper.MapColor(ConsoleColor.Gray, foreColor);
//			_mapper.MapColor(ConsoleColor.Black, backColor);
//			this.DefaultForeColor = foreColor;
//			this.DefaultBackColor = backColor;
//		}
//	}

//   // /// <summary>
//   // /// Stores and manages the assignment of System.Drawing.Color objects to ConsoleColor objects.
//   // /// </summary>
//   // public sealed class ColorStore
//   // {
//   //     /// <summary>
//   //     /// A map from System.Drawing.Color to ConsoleColor.
//   //     /// </summary>
//   //     public ConcurrentDictionary<Color, System.ConsoleColor> Colors { get; }
//   //     /// <summary>
//   //     /// A map from ConsoleColor to System.Drawing.Color.
//   //     /// </summary>
//   //     public ConcurrentDictionary<System.ConsoleColor, Color> ConsoleColors { get; }

//   //     /// <summary>
//   //     /// Manages the assignment of System.Drawing.Color objects to ConsoleColor objects.
//   //     /// </summary>
//   //     /// <param name="colorMap">The Dictionary the ColorStore should use to key System.Drawing.Color objects
//   //     /// to ConsoleColor objects.</param>
//   //     /// <param name="consoleColorMap">The Dictionary the ColorStore should use to key ConsoleColor
//   //     /// objects to System.Drawing.Color objects.</param>
//   //     public ColorStore(ConcurrentDictionary<Color, System.ConsoleColor> colorMap, ConcurrentDictionary<System.ConsoleColor, Color> consoleColorMap)
//   //     {
//   //         Colors = colorMap;
//   //         ConsoleColors = consoleColorMap;
//   //     }

//	  //  public ColorStore()
//	  //  {
//			//Colors = new ConcurrentDictionary<Color, System.ConsoleColor>();
//			//ConsoleColors = new ConcurrentDictionary<System.ConsoleColor, Color>();
//		 //   foreach (var cc in new[]
//		 //   {
//			//    CC.Black, CC.DarkGray, CC.Gray, CC.White,
//			//    CC.Blue, CC.DarkBlue,
//			//    CC.Green, CC.DarkGreen,
//			//    CC.Red, CC.DarkRed,
//			//    CC.Cyan, CC.DarkCyan,
//			//    CC.Magenta, CC.DarkMagenta,
//			//    CC.Yellow, CC.DarkYellow,
//		 //   })
//		 //   {
//			//    ConsoleColors.TryAdd(cc, cc);
//		 //   }
//	  //  }

//   //     /// <summary>
//   //     /// Adds a new System.Drawing.Color to the ColorStore.
//   //     /// </summary>
//   //     /// <param name="oldColor">The ConsoleColor to be replaced by the new System.Drawing.Color.</param>
//   //     /// <param name="newColor">The System.Drawing.Color to be added to the ColorStore.</param>
//   //     public void Update(CC oldColor, Color newColor)
//   //     {
//	  //      Colors.TryAdd(newColor, oldColor);
//   //         ConsoleColors[oldColor] = newColor;
//   //     }

//   //     /// <summary>
//   //     /// Replaces one System.Drawing.Color in the ColorStore with another.
//   //     /// </summary>
//   //     /// <param name="oldColor">The color to be replaced.</param>
//   //     /// <param name="newColor">The replacement color.</param>
//   //     /// <returns>The ConsoleColor key of the System.Drawing.Color object that was replaced in the ColorStore.</returns>
//   //     public CC Replace(Color oldColor, Color newColor)
//   //     {
//   //         bool oldColorExistedInColorStore = Colors.TryRemove(oldColor, out var consoleColorKey);

//   //         if (!oldColorExistedInColorStore)
//   //         {
//   //             throw new ArgumentException("An attempt was made to replace a nonexistent color in the ColorStore!");
//   //         }

//   //         Colors.TryAdd(newColor, consoleColorKey);
//   //         ConsoleColors[consoleColorKey] = newColor;

//   //         return consoleColorKey;
//   //     }

//   //     /// <summary>
//   //     /// Notifies the caller as to whether or not the specified System.Drawing.Color needs to be added 
//   //     /// to the ColorStore.
//   //     /// </summary>
//   //     /// <param name="color">The System.Drawing.Color to be checked for membership.</param>
//   //     /// <returns>Returns 'true' if the ColorStore already contains the specified System.Drawing.Color.</returns>
//   //     public bool RequiresUpdate(Color color)
//   //     {
//   //         return !Colors.ContainsKey(color);
//   //     }
//   // }
//}