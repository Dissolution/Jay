using System.Drawing;

namespace Jay.Consolas
{
	/// <summary>
	/// Options related to a <see cref="Console"/>'s buffer area.
	/// </summary>
	public sealed class Buffer
	{
		private readonly Console _console;

		/// <summary>
		/// Gets or sets the width of the buffer area.
		/// </summary>
		public int Width
		{
			get => System.Console.BufferWidth;
			set => System.Console.BufferWidth = value;
		}

		/// <summary>
		/// Gets or sets the height of the buffer area.
		/// </summary>
		public int Height
		{
			get => System.Console.BufferHeight;
			set => System.Console.BufferHeight = value;
		}

		internal Buffer(Console console)
		{
			_console = console;
		}

		/// <summary>
		/// Sets the width and height of the buffer area to the specified values.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public Console SetSize(Size size)
		{
			System.Console.SetBufferSize(size.Width, size.Height);
			return _console;
		}

		/// <summary>
		/// Sets the width and height of the buffer area to the specified values.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public Console SetSize(int width, int height)
		{
			System.Console.SetBufferSize(width, height);
			return _console;
		}

		/// <summary>
		/// Copies a specified source area of the screen buffer to a specified destination area.
		/// </summary>
		/// <param name="area"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public Console Copy(Rectangle area, Point position)
		{
			System.Console.MoveBufferArea(area.Left, area.Top, area.Width, area.Height, position.X, position.Y);
			return _console;
		}

		/// <summary>
		/// Copies a specified source area of the screen buffer to a specified destination area.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="xPos"></param>
		/// <param name="yPos"></param>
		/// <returns></returns>
		public Console Copy(int left, int top, int width, int height, int xPos, int yPos)
		{
			System.Console.MoveBufferArea(left, top, width, height, xPos, yPos);
			return _console;
		}
	}
}