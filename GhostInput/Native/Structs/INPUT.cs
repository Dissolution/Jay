using System.Runtime.InteropServices;
using System.Windows.Input;
using Jay.GhostInput.Native.Enums;
using Jay.Text;
using InputType = Jay.GhostInput.Native.Enums.InputType;

namespace Jay.GhostInput.Native.Structs
{
	[StructLayout(LayoutKind.Sequential)]
	// ReSharper disable once InconsistentNaming
	internal struct INPUT
	{
		/// <summary>
		/// Specifies he type of the input event. This member can be one of the following values:
		/// <see cref="Native.Enums.InputType.Mouse"/> - The event is a mouse event.
		/// <see cref="Native.Enums.InputType.Keyboard"/> - The event is a keyboard event.
		/// <see cref="Native.Enums.InputType.Hardware"/> - The event is a hardware event.
		/// </summary>
		internal InputType Type;

		/// <summary>
		/// The data structure that contains information for synthesizing input events such as keystrokes, mouse movement, and mouse clicks.
		/// </summary>
		internal InputUnion Data;

		/// <summary>
		/// Get the size of the INPUT structure.
		/// </summary>
		internal static int Size => Marshal.SizeOf(typeof(INPUT));

		/// <inheritdoc />
		public override string ToString()
		{
			if (Type == InputType.Hardware)
				return "Hardware";

			using var text = TextBuilder.Rent();

			if (Type == InputType.Keyboard)
			{
				var data = Data.Keyboard;
				if (data.KeyCode != 0)
					text.Append(KeyInterop.KeyFromVirtualKey(data.KeyCode));
				else if (data.ScanCode != default)
					text.Append((char) data.ScanCode);
				text.Append('_');
				if ((data.Flags & KeyboardFlag.KEYUP) != 0)
					text.Append("up");
				else
					text.Append("down");
			}
			else
			{
				var data = Data.Mouse;
				if ((data.Flags & MouseFlag.MOVE) != 0)
				{
					text.Append("move_");
					if ((data.Flags & MouseFlag.ABSOLUTE) != 0)
						text.Append("to_");
					else
						text.Append("by_");
					text.Append(data.X).Append('_').Append(data.Y);
				}
				else if ((data.Flags & MouseFlag.LEFTDOWN) != 0)
					return "left_down";
				else if ((data.Flags & MouseFlag.LEFTUP) != 0)
					return "left_up";
				else if ((data.Flags & MouseFlag.RIGHTDOWN) != 0)
					return "right_down";
				else if ((data.Flags & MouseFlag.RIGHTUP) != 0)
					return "right_up";
			}

			//Fin
			return text.ToString();
		}
	}
}
