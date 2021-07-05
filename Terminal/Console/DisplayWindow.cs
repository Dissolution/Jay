using System.Drawing;

namespace Jay.Consolas
{
	/// <summary>
	/// Options related to a <see cref="Console"/>'s display (not actual) window.
	/// </summary>
	public sealed class DisplayWindow
	{
		private readonly Console _console;

		/// <summary>
		/// Gets or sets the leftmost position of the <see cref="Console"/> display window area relative to the screen buffer.
		/// </summary>
		public int Left
		{
			get => System.Console.WindowLeft;
			set => System.Console.WindowLeft = value;
		}

		/// <summary>
		/// Gets or sets the top position of the <see cref="Console"/> display window area relative to the screen buffer.
		/// </summary>
		public int Top
		{
			get => System.Console.WindowTop;
			set => System.Console.WindowTop = value;
		}

		/// <summary>
		/// Gets or sets the position of the <see cref="Console"/> display window area relative to the screen buffer.
		/// </summary>
		public Point Position
		{
			get => new Point(System.Console.WindowLeft, System.Console.WindowTop);
			set => System.Console.SetWindowPosition(value.X, value.Y);
		}

		/// <summary>
		/// Gets or sets the width of the <see cref="Console"/> display window.
		/// </summary>
		public int Width
		{
			get => System.Console.WindowWidth;
			set => System.Console.WindowWidth = value;
		}

		/// <summary>
		/// Gets or sets the height of the <see cref="Console"/> display window.
		/// </summary>
		public int Height
		{
			get => System.Console.WindowHeight;
			set => System.Console.WindowHeight = value;
		}

		/// <summary>
		/// Gets or sets the width and height of the <see cref="Console"/> display window.
		/// </summary>
		public Size Size
		{
			get => new Size(System.Console.WindowWidth, System.Console.WindowHeight);
			set => System.Console.SetWindowSize(value.Width, value.Height);
		}

		/// <summary>
		/// Gets the largest possible number of <see cref="Console"/> display window rows, based on the current font, screen resolution, and window size.
		/// </summary>
		public int LargestHeight => System.Console.LargestWindowHeight;

		/// <summary>
		/// Gets the largest possible number of <see cref="Console"/> display window columns, based on the current font, screen resolution, and window size.
		/// </summary>
		public int LargestWidth => System.Console.LargestWindowWidth;

		internal DisplayWindow(Console console)
		{
			_console = console;
		}

		/// <summary>
		/// Sets the position of the <see cref="Console"/> display window relative to the screen buffer.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public Console SetPosition(Point position)
		{
			System.Console.SetWindowPosition(position.X, position.Y);
			return _console;
		}

		/// <summary>
		/// Sets the position of the <see cref="Console"/> display window relative to the screen buffer.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <returns></returns>
		public Console SetPosition(int left, int top)
		{
			System.Console.SetWindowPosition(left, top);
			return _console;
		}

		/// <summary>
		/// Sets the width and height of the <see cref="Console"/> display window.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public Console SetSize(Size size)
		{
			System.Console.SetWindowSize(size.Width, size.Height);
			return _console;
		}

		/// <summary>
		/// Sets the width and height of the <see cref="Console"/> display window.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public Console SetSize(int width, int height)
		{
			System.Console.SetWindowSize(width, height);
			return _console;
		}
	}
}