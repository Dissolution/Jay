using System;
using System.Windows;
using System.Windows.Input;
using Jay.GhostInput.Native.Enums;
using Jay.GhostInput.Native.Structs;
using InputType = Jay.GhostInput.Native.Enums.InputType;

namespace Jay.GhostInput.Input
{
	internal sealed class MouseInputFactory
	{
		private readonly System.Windows.Rect _bounds;

		public MouseInputFactory()
		{
			var screenHeight = SystemParameters.PrimaryScreenHeight;
			var screenWidth = SystemParameters.PrimaryScreenWidth;
			_bounds = new Rect(0, 0, screenWidth, screenHeight);
		}

		#region Private Methods
		private MouseFlag GetFlag(MouseButton button, MouseButtonState state)
		{
			switch (button)
			{
				case MouseButton.Left:
					return state == MouseButtonState.Pressed ? MouseFlag.LEFTDOWN : MouseFlag.LEFTUP;
				case MouseButton.Middle:
					return state == MouseButtonState.Pressed ? MouseFlag.MIDDLEDOWN : MouseFlag.MIDDLEUP;
				case MouseButton.Right:
					return state == MouseButtonState.Pressed ? MouseFlag.RIGHTDOWN : MouseFlag.RIGHTUP;
				case MouseButton.XButton1:
				case MouseButton.XButton2:
					return state == MouseButtonState.Pressed ? MouseFlag.XDOWN : MouseFlag.XUP;
				default:
					throw new ArgumentException(nameof(button));
			}
		}
		#endregion

		#region Mouse Input
		#region Movement
		public INPUT Move(double x, double y)
		{
			var absoluteX = (65535d * x) / _bounds.Width;
			var absoluteY = (65535d * y) / _bounds.Height;

			var input = new INPUT {Type = InputType.Mouse};
			input.Data.Mouse.Flags = MouseFlag.MOVE | MouseFlag.ABSOLUTE;
			input.Data.Mouse.X = (int)absoluteX;
			input.Data.Mouse.Y = (int)absoluteY;
			return input;
		}
		#endregion
		#region Buttons
		public INPUT ButtonDown(MouseButton button)
		{
			var flag = GetFlag(button, MouseButtonState.Pressed);
			var input = new INPUT{Type = InputType.Mouse};
			input.Data.Mouse.Flags = flag;
			return input;
		}

		public INPUT ButtonUp(MouseButton button)
		{
			var flag = GetFlag(button, MouseButtonState.Released);
			var input = new INPUT {Type = InputType.Mouse};
			input.Data.Mouse.Flags = flag;
			return input;
		}
		#endregion
		#region Scrolling
		public INPUT ScrollVertical(int scrollAmount)
		{
			var input = new INPUT {Type = InputType.Mouse};
			input.Data.Mouse.Flags = MouseFlag.VERTICAL_WHEEL;
			input.Data.Mouse.Data = scrollAmount;
			return input;
		}

		public INPUT ScrollHorizontal(int scrollAmount)
		{
			var input = new INPUT {Type = InputType.Mouse};
			input.Data.Mouse.Flags = MouseFlag.HORIZONTAL_WHEEL;
			input.Data.Mouse.Data = scrollAmount;
			return input;
		}
		#endregion
		#endregion
	}
}
