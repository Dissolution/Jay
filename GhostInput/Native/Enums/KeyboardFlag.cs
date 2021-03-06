using System;

namespace Jay.GhostInput.Native.Enums
{
	[Flags]
	internal enum KeyboardFlag : uint
	{
		/// <summary>
		/// KEYEVENTF_EXTENDEDKEY = 0x0001
		/// If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
		/// </summary>
		EXTENDEDKEY = 0x0001,

		/// <summary>
		/// KEYEVENTF_KEYUP = 0x0002
		/// If specified, the key is being released. If not specified, the key is being pressed.
		/// </summary>
		KEYUP = 0x0002,

		/// <summary>
		/// KEYEVENTF_UNICODE = 0x0004
		/// If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. 
		/// </summary>
		UNICODE = 0x0004,

		/// <summary>
		/// KEYEVENTF_SCANCODE = 0x0008
		/// If specified, wScan identifies the key and wVk is ignored.
		/// </summary>
		SCANCODE = 0x0008
	}
}
