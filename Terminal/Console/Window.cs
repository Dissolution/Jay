using System;
using System.Drawing;

namespace Jay.Consolas
{
	/// <summary>
	/// Options related to a <see cref="Console"/>'s actual window.
	/// </summary>
	public sealed class Window
	{
		private readonly Console _console;
		private readonly IntPtr _handle;

		/// <summary>
		/// Gets or sets the title to display in the <see cref="Console"/> window.
		/// </summary>
		public string Title
		{
			get => System.Console.Title;
			set => System.Console.Title = value;
		}

		/// <summary>
		/// Gets or sets the bounds of the <see cref="Console"/> window.
		/// </summary>
		public Rectangle Bounds
		{
			get
			{
				RECT rect = default;
				if (Native.GetWindowRect(_handle, ref rect))
					return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
				return Rectangle.Empty;
			}
			set => Native.MoveWindow(_handle, value.X, value.Y, value.Width, value.Height, true);
		}

		internal Window(Console console)
		{
			_console = console;
			_handle = Native.GetConsoleWindow();
		}

		/// <summary>
		/// Sets the title to display in the <see cref="Console"/> window.
		/// </summary>
		/// <param name="title"></param>
		/// <returns></returns>
		public Console SetTitle(string title)
		{
			System.Console.Title = title;
			return _console;
		}

		/// <summary>
		/// Manipules the <see cref="Console"/> window with the specified <see cref="Jay.Consolas.Show"/> command.
		/// </summary>
		/// <param name="show"></param>
		/// <returns></returns>
		public Console Show(Show show)
		{
			Native.ShowWindow(_handle, (int) show);
			return _console;
		}

		/// <summary>
		/// Moves the <see cref="Console"/> window to the specified <see cref="Point"/>.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public Console Move(Point position) => Move(position.X, position.Y);

		/// <summary>
		/// Moves the <see cref="Console"/> window to the specified X and Y coordinates.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Console Move(int x, int y)
		{
			var bounds = this.Bounds;
			Native.MoveWindow(_handle, x, y, bounds.Width, bounds.Height, true);
			return _console;
		}

		/// <summary>
		/// Resize the <see cref="Console"/> window to the specified <see cref="Size"/>.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public Console Resize(Size size) => Resize(size.Width, size.Height);

		/// <summary>
		/// Resize the <see cref="Console"/> window to the specified width and height.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public Console Resize(int width, int height)
		{
			if (width <= 0)
				throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
			if (height <= 0)
				throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
			var bounds = this.Bounds;
			Native.MoveWindow(_handle, bounds.X, bounds.Y, width, height, true);
			return _console;
		}

		/// <summary>
		/// Moves and/or resizes the <see cref="Console"/> widow to the specified <see cref="Rectangle"/>.
		/// </summary>
		/// <param name="bounds"></param>
		/// <returns></returns>
		public Console SetBounds(Rectangle bounds)
		{
			Native.MoveWindow(_handle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);
			return _console;
		}
	}
}