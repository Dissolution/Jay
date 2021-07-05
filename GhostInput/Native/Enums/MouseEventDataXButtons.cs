using System;

namespace Jay.GhostInput.Native.Enums
{
	[Flags]
	internal enum MouseEventDataXButtons : uint
	{
		/// <summary>
		/// No XButton was used
		/// </summary>
		Nothing = 0x00000000,

		/// <summary>
		/// XBUTTON1 = 0x0001
		/// Set if the first X button is pressed or released.
		/// </summary>
		XBUTTON1 = 0x00000001,

		/// <summary>
		/// XBUTTON1 = 0x0002
		/// Set if the second X button is pressed or released.
		/// </summary>
		XBUTTON2 = 0x00000002
	}
}
