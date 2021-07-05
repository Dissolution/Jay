using System.Windows.Input;
using Jay.GhostInput.Native.Enums;
using Jay.GhostInput.Native.Structs;
using InputType = Jay.GhostInput.Native.Enums.InputType;

namespace Jay.GhostInput.Input
{
	internal sealed class KeyboardInputFactory
	{
		#region Keyboard Input
		public INPUT KeyDown(Key key)
		{
			var input = new INPUT {Type = InputType.Keyboard};
			input.Data.Keyboard.KeyCode = (ushort)KeyInterop.VirtualKeyFromKey(key);
			return input;
		}

		public INPUT KeyDown(char c)
		{
			var input = new INPUT { Type = InputType.Keyboard };
			input.Data.Keyboard.KeyCode = 0;
			input.Data.Keyboard.ScanCode = (ScanCode)c;
			input.Data.Keyboard.Flags = KeyboardFlag.UNICODE;

			// Handle extended keys:
			// If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
			// we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
			if ((c & 0xFF00) == 0xE000)
			{
				input.Data.Keyboard.Flags |= KeyboardFlag.EXTENDEDKEY;
			}

			return input;
		}

		public INPUT KeyUp(Key key)
		{
			var input = new INPUT { Type = InputType.Keyboard };
			input.Data.Keyboard.KeyCode = (ushort)KeyInterop.VirtualKeyFromKey(key);
			input.Data.Keyboard.Flags = KeyboardFlag.KEYUP;
			return input;
		}

		public INPUT KeyUp(char c)
		{
			var input = new INPUT { Type = InputType.Keyboard };
			input.Data.Keyboard.KeyCode = 0;
			input.Data.Keyboard.ScanCode = (ScanCode)c;
			input.Data.Keyboard.Flags = KeyboardFlag.KEYUP | KeyboardFlag.UNICODE;

			// Handle extended keys:
			// If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
			// we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
			if ((c & 0xFF00) == 0xE000)
			{
				input.Data.Keyboard.Flags |= KeyboardFlag.EXTENDEDKEY;
			}

			return input;
		}
		#endregion
	}
}
