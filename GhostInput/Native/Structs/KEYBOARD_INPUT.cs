using System;
using System.Runtime.InteropServices;
using Jay.GhostInput.Native.Enums;

namespace Jay.GhostInput.Native.Structs
{
	/// <summary>
	/// Contains information about a simulated keyboard event.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct KeyboardInput
	{
		/// <summary>
		/// A virtual-key code. The code must be a value in the range 1 to 254. If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0.
		/// </summary>
		internal ushort KeyCode;

		/// <summary>
		/// A hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application.
		/// </summary>
		internal ScanCode ScanCode;

		/// <summary>
		/// Specifies various aspects of a keystroke.
		/// </summary>
		internal KeyboardFlag Flags;

		/// <summary>
		/// The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp.
		/// </summary>
		internal uint Time;

		/// <summary>
		/// An additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information.
		/// </summary>
		internal UIntPtr ExtraInfo;
	}
}
