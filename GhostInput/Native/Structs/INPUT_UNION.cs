using System.Runtime.InteropServices;

namespace Jay.GhostInput.Native.Structs
{
	/// <summary>
	/// The unioned structure that contains Mouse, Keyboard, and Hardware input messages.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	internal struct InputUnion
	{
		/// <summary>
		/// The <see cref="MouseInput"/> messages.
		/// </summary>
		[FieldOffset(0)]
		internal MouseInput Mouse;

		/// <summary>
		/// The <see cref="KeyboardInput"/> messages.
		/// </summary>
		[FieldOffset(0)]
		internal KeyboardInput Keyboard;

		/// <summary>
		/// The <see cref="HardwareInput"/> messages.
		/// </summary>
		[FieldOffset(0)]
		internal HardwareInput Hardware;
	}
}
