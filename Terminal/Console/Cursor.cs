using System;
using System.Drawing;

namespace Jay.Consolas
{
	/// <summary>
	/// Options related to a <see cref="Console"/>'s cursor.
	/// </summary>
	public sealed class Cursor
	{
		private readonly Console _console;
		
		/// <summary>
		/// Gets or sets the column position of the cursor within the buffer area.
		/// </summary>
		public int Left
		{
			get => System.Console.CursorLeft;
			set => System.Console.CursorLeft = value;
		}

		/// <summary>
		/// Gets or sets the row position of the cursor within the buffer area.
		/// </summary>
		public int Top
		{
			get => System.Console.CursorTop;
			set => System.Console.CursorTop = value;
		}

		/// <summary>
		/// Gets or sets the cursor position within the buffer area.
		/// </summary>
		public Point Position
		{
			get => new Point(System.Console.CursorLeft, System.Console.CursorTop);
			set => System.Console.SetCursorPosition(value.X, value.Y);
		}

		/// <summary>
		/// Gets or sets the height of the cursor within a cell.
		/// </summary>
		public int Size
		{
			get => System.Console.CursorSize;
			set => System.Console.CursorSize = value;
		}

		/// <summary>
		/// Gets or sets whether the cursor is visible.
		/// </summary>
		public bool Visible
		{
			get => System.Console.CursorVisible;
			set => System.Console.CursorVisible = value;
		}

		internal Cursor(Console console)
		{
			_console = console;
		}

		/// <summary>
		/// Sets the position of the cursor.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public Console SetPosition(Point position)
		{
			System.Console.SetCursorPosition(position.X, position.Y);
			return _console;
		}

		/// <summary>
		/// Sets the position of the cursor.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <returns></returns>
		public Console SetPosition(int left, int top)
		{
			System.Console.SetCursorPosition(left, top);
			return _console;
		}

		/// <summary>
		/// Sets the size of the cursor.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public Console SetSize(int size)
		{
			System.Console.CursorSize = size;
			return _console;
		}

		/// <summary>
		/// Sets the cursor's visibility.
		/// </summary>
		/// <param name="visible"></param>
		/// <returns></returns>
		public Console SetVisible(bool visible)
		{
			System.Console.CursorVisible = visible;
			return _console;
		}
	}
}